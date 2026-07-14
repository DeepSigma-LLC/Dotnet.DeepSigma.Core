using System.Buffers.Binary;
using System.Security.Cryptography;

namespace DeepSigma.Core.Cryptography;

/// <summary>
/// Password-based file encryption using AES-256-CBC with encrypt-then-MAC (HMAC-SHA256).
///
/// File format:
///   offset  size  field
///   0       4     magic "FPRT"
///   4       1     format version
///   5       1     KDF id (1 = PBKDF2-HMAC-SHA256)
///   6       4     iteration count (uint32, little-endian) — as used for THIS file
///   10      16    salt
///   26      16    IV
///   42      ..    AES-256-CBC ciphertext (PKCS7)
///   EOF-32  32    HMAC-SHA256 over every preceding byte (header + ciphertext)
///
/// The MAC is verified before any plaintext is produced, so a wrong password or a
/// tampered file fails cleanly instead of emitting garbage.
/// </summary>
public static class FileProtector
{
    private static readonly byte[] Magic = "FPRT"u8.ToArray();

    private const byte FormatVersion = 1;
    private const byte KdfPbkdf2Sha256 = 1;

    private const int SaltSize = 16;
    private const int IvSize = 16;
    private const int EncKeySize = 32;   // AES-256
    private const int MacKeySize = 32;   // HMAC-SHA256
    private const int MacSize = 32;
    private const int HeaderSize = 4 + 1 + 1 + 4 + SaltSize + IvSize;   // 42

    /// <summary>Iteration count used for new files. Raise freely — old files carry their own.</summary>
    public const int DefaultIterations = 600_000;

    /// <summary>Reject absurd iteration counts from an untrusted header (denial-of-service guard).</summary>
    private const int MaxAcceptedIterations = 10_000_000;

    private const int BufferSize = 81_920;

    /// <summary>
    /// Encrypts a file with a password, producing a new file in the format described above.
    /// </summary>
    /// <param name="inputFile">The path to the input file to be encrypted.</param>
    /// <param name="outputFile">The path where the encrypted file will be saved.</param>
    /// <param name="password">The password used for encryption.</param>
    /// <param name="iterations">The number of iterations for the key derivation function.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous encryption operation.</returns>
    public static async Task EncryptFileAsync(
        string inputFile,
        string outputFile,
        string password,
        int iterations = DefaultIterations,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(inputFile);
        ArgumentException.ThrowIfNullOrEmpty(outputFile);
        ArgumentException.ThrowIfNullOrEmpty(password);
        ArgumentOutOfRangeException.ThrowIfLessThan(iterations, 1);

        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] iv = RandomNumberGenerator.GetBytes(IvSize);

        // KDF is CPU-bound and slow by design: keep it off the caller's thread.
        (byte[] encKey, byte[] macKey) = await Task.Run(
            () => DeriveKeys(password, salt, iterations), cancellationToken);

        // Write to a temp file and move on success, so a failure never leaves a
        // half-written file where a valid one used to be.
        string tempFile = outputFile + ".tmp";

        try
        {
            byte[] header = BuildHeader(iterations, salt, iv);

            await using (var fsOut = new FileStream(
                tempFile, FileMode.Create, FileAccess.Write, FileShare.None,
                BufferSize, useAsync: true))
            {
                using var hmac = new HMACSHA256(macKey);
                using var aes = Aes.Create();
                aes.Key = encKey;
                aes.IV = iv;

                // macStream passes bytes through to fsOut unchanged while hashing them,
                // so the MAC covers the header and the ciphertext exactly as written.
                await using (var macStream = new CryptoStream(fsOut, hmac, CryptoStreamMode.Write, leaveOpen: true))
                {
                    await macStream.WriteAsync(header, cancellationToken);

                    using ICryptoTransform encryptor = aes.CreateEncryptor();
                    await using (var aesStream = new CryptoStream(macStream, encryptor, CryptoStreamMode.Write, leaveOpen: true))
                    await using (var fsIn = new FileStream(
                        inputFile, FileMode.Open, FileAccess.Read, FileShare.Read,
                        BufferSize, useAsync: true))
                    {
                        await fsIn.CopyToAsync(aesStream, BufferSize, cancellationToken);
                    }
                    // aesStream disposed -> FlushFinalBlock -> final padded block reaches macStream.
                }
                // macStream disposed -> FlushFinalBlock -> hmac.Hash is now final.

                await fsOut.WriteAsync(hmac.Hash!, cancellationToken);
            }

            File.Move(tempFile, outputFile, overwrite: true);
        }
        catch
        {
            TryDelete(tempFile);
            throw;
        }
        finally
        {
            CryptographicOperations.ZeroMemory(encKey);
            CryptographicOperations.ZeroMemory(macKey);
        }
    }


    /// <summary>
    /// Decrypts a file previously encrypted with EncryptFileAsync.
    /// </summary>
    /// <param name="inputFile">The path to the encrypted input file.</param>
    /// <param name="outputFile">The path where the decrypted file will be saved.</param>
    /// <param name="password">The password used for decryption.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <exception cref="InvalidDataException">Not a FPRT file, or an unsupported version.</exception>
    /// <exception cref="CryptographicException">Wrong password, or the file has been altered.</exception>
    public static async Task DecryptFileAsync(
        string inputFile,
        string outputFile,
        string password,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(inputFile);
        ArgumentException.ThrowIfNullOrEmpty(outputFile);
        ArgumentException.ThrowIfNullOrEmpty(password);

        await using var fsIn = new FileStream(
            inputFile, FileMode.Open, FileAccess.Read, FileShare.Read,
            BufferSize, useAsync: true);

        if (fsIn.Length < HeaderSize + MacSize)
            throw new InvalidDataException("File is too small to be a valid protected file.");

        byte[] header = new byte[HeaderSize];
        await fsIn.ReadExactlyAsync(header, cancellationToken);

        (int iterations, byte[] salt, byte[] iv) = ParseHeader(header);

        long cipherLength = fsIn.Length - HeaderSize - MacSize;

        (byte[] encKey, byte[] macKey) = await Task.Run(
            () => DeriveKeys(password, salt, iterations), cancellationToken);

        try
        {
            // Pass 1: verify the MAC over header + ciphertext BEFORE decrypting anything.
            byte[] expected = await ComputeMacAsync(fsIn, macKey, cipherLength, header, cancellationToken);

            byte[] actual = new byte[MacSize];
            fsIn.Position = HeaderSize + cipherLength;
            await fsIn.ReadExactlyAsync(actual, cancellationToken);

            if (!CryptographicOperations.FixedTimeEquals(expected, actual))
                throw new CryptographicException(
                    "Authentication failed. The password is wrong, or the file has been modified.");

            // Pass 2: authenticated — now it is safe to decrypt.
            string tempFile = outputFile + ".tmp";
            try
            {
                fsIn.Position = HeaderSize;

                using var aes = Aes.Create();
                aes.Key = encKey;
                aes.IV = iv;

                await using (var fsOut = new FileStream(
                    tempFile, FileMode.Create, FileAccess.Write, FileShare.None,
                    BufferSize, useAsync: true))
                {
                    using ICryptoTransform decryptor = aes.CreateDecryptor();
                    await using (var aesStream = new CryptoStream(fsOut, decryptor, CryptoStreamMode.Write, leaveOpen: true))
                    {
                        await CopyExactlyAsync(fsIn, aesStream, cipherLength, cancellationToken);
                    }
                }

                File.Move(tempFile, outputFile, overwrite: true);
            }
            catch
            {
                TryDelete(tempFile);
                throw;
            }
        }
        finally
        {
            CryptographicOperations.ZeroMemory(encKey);
            CryptographicOperations.ZeroMemory(macKey);
        }
    }

    // ---------------------------------------------------------------- helpers

    private static (byte[] encKey, byte[] macKey) DeriveKeys(string password, byte[] salt, int iterations)
    {
        byte[] derived = Rfc2898DeriveBytes.Pbkdf2(
            password, salt, iterations, HashAlgorithmName.SHA256, EncKeySize + MacKeySize);

        try
        {
            byte[] encKey = derived[..EncKeySize];
            byte[] macKey = derived[EncKeySize..];
            return (encKey, macKey);
        }
        finally
        {
            CryptographicOperations.ZeroMemory(derived);
        }
    }

    private static byte[] BuildHeader(int iterations, byte[] salt, byte[] iv)
    {
        byte[] header = new byte[HeaderSize];
        Span<byte> span = header;

        Magic.CopyTo(span);
        span[4] = FormatVersion;
        span[5] = KdfPbkdf2Sha256;
        BinaryPrimitives.WriteUInt32LittleEndian(span[6..10], (uint)iterations);
        salt.CopyTo(span[10..26]);
        iv.CopyTo(span[26..42]);

        return header;
    }

    private static (int iterations, byte[] salt, byte[] iv) ParseHeader(byte[] header)
    {
        ReadOnlySpan<byte> span = header;

        if (!span[..4].SequenceEqual(Magic))
            throw new InvalidDataException("Not a protected file (bad magic bytes).");

        byte version = span[4];
        if (version != FormatVersion)
            throw new InvalidDataException($"Unsupported format version {version}; this build understands {FormatVersion}.");

        byte kdf = span[5];
        if (kdf != KdfPbkdf2Sha256)
            throw new InvalidDataException($"Unsupported KDF id {kdf}.");

        uint iterations = BinaryPrimitives.ReadUInt32LittleEndian(span[6..10]);
        if (iterations is 0 or > MaxAcceptedIterations)
            throw new InvalidDataException($"Header declares an unreasonable iteration count ({iterations}).");

        return ((int)iterations, span[10..26].ToArray(), span[26..42].ToArray());
    }

    private static async Task<byte[]> ComputeMacAsync(
        FileStream fsIn, byte[] macKey, long cipherLength, byte[] header, CancellationToken ct)
    {
        using var hmac = new HMACSHA256(macKey);

        hmac.TransformBlock(header, 0, header.Length, null, 0);

        fsIn.Position = HeaderSize;

        byte[] buffer = new byte[BufferSize];
        long remaining = cipherLength;

        while (remaining > 0)
        {
            int want = (int)Math.Min(buffer.Length, remaining);
            int read = await fsIn.ReadAsync(buffer.AsMemory(0, want), ct);
            if (read == 0)
                throw new InvalidDataException("File ended unexpectedly.");

            hmac.TransformBlock(buffer, 0, read, null, 0);
            remaining -= read;
        }

        hmac.TransformFinalBlock([], 0, 0);
        return hmac.Hash!;
    }

    private static async Task CopyExactlyAsync(
        Stream source, Stream destination, long count, CancellationToken ct)
    {
        byte[] buffer = new byte[BufferSize];
        long remaining = count;

        while (remaining > 0)
        {
            int want = (int)Math.Min(buffer.Length, remaining);
            int read = await source.ReadAsync(buffer.AsMemory(0, want), ct);
            if (read == 0)
                throw new InvalidDataException("File ended unexpectedly.");

            await destination.WriteAsync(buffer.AsMemory(0, read), ct);
            remaining -= read;
        }
    }

    private static void TryDelete(string path)
    {
        try { if (File.Exists(path)) File.Delete(path); }
        catch { /* best effort */ }
    }
}