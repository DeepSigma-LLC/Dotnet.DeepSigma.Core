using DeepSigma.Core.Encode;
using System.Security.Cryptography;

namespace DeepSigma.Core.Cryptography.Asymmetric;

/// <summary>
/// Handles RSA cryptography operations such as signing and verifying data.
/// RSA (Rivest–Shamir–Adleman) is a widely used asymmetric cryptographic algorithm that uses a pair of keys: a public key for encryption and a private key for decryption.
/// It is commonly used for secure data transmission, digital signatures, and key exchange.
/// RSA can only encrypt data up to a certain size limit, which is determined by the key size and padding scheme. 
/// For larger data, symmetric encryption (e.g., AES) is typically used in conjunction with RSA for secure key exchange.
/// </summary>
/// <remarks>
/// Note: RSA must be disposed of after use to free up resources. Use the 'using' statement or manually call Dispose() when done with an RSA instance.
/// </remarks>
public static class RsaCryptography
{
    // Tune this: higher = slower for you AND for an attacker guessing the password.
    private const int Pbkdf2Iterations = 600_000;

    /// <summary>
    /// Generates a new RSA key pair.
    /// </summary>
    /// <param name="keySize">The size of the RSA key to generate. Default is 3072 bits.</param>
    /// <returns>A new RSA instance with the generated key pair.</returns>
    public static RSA GenerateRSAKeys(int keySize = 3072)
    {
        return RSA.Create(keySize);
    }

    /// <summary>
    /// Exports the public key as SubjectPublicKeyInfo (SPKI). Not secret — safe to share.
    /// </summary>
    public static byte[] ExportPublicKey(RSA rsa)
    {
        ArgumentNullException.ThrowIfNull(rsa);
        return rsa.ExportSubjectPublicKeyInfo();
    }

    /// <summary>
    /// Imports a public key previously exported with <see cref="ExportPublicKey"/>.
    /// Caller owns the returned RSA instance.
    /// </summary>
    /// <param name="spki">The SubjectPublicKeyInfo (SPKI) byte array representing the public key.</param>
    public static RSA ImportPublicKey(byte[] spki)
    {
        ArgumentNullException.ThrowIfNull(spki);

        RSA rsa = RSA.Create();
        try
        {
            rsa.ImportSubjectPublicKeyInfo(spki, out _);
            return rsa;
        }
        catch
        {
            rsa.Dispose();
            throw;
        }
    }

    /// <summary>
    /// Exports the private key as encrypted PKCS#8. Safe to write to disk.
    /// </summary>
    public static byte[] ExportPrivateKey(RSA rsa, ReadOnlySpan<char> password)
    {
        ArgumentNullException.ThrowIfNull(rsa);
        ArgumentException.ThrowIfNullOrEmpty(password.ToString(), nameof(password));

        if (password.IsEmpty)
            throw new ArgumentException("Password must not be empty.", nameof(password));

        return rsa.ExportEncryptedPkcs8PrivateKey(
            password,
            new PbeParameters(
                PbeEncryptionAlgorithm.Aes256Cbc,
                HashAlgorithmName.SHA256,
                Pbkdf2Iterations));
    }

    /// <summary>
    /// Imports a private key previously exported with <see cref="ExportPrivateKey"/>.
    /// Caller owns the returned RSA instance.
    /// </summary>
    public static RSA ImportPrivateKey(byte[] pkcs8, ReadOnlySpan<char> password)
    {
        ArgumentNullException.ThrowIfNull(pkcs8);
        ArgumentException.ThrowIfNullOrWhiteSpace(password.ToString(), nameof(password));

        RSA rsa = RSA.Create();
        try
        {
            rsa.ImportEncryptedPkcs8PrivateKey(password, pkcs8, out _);
            return rsa;
        }
        catch
        {
            rsa.Dispose();
            throw;
        }
    }

    /// <summary>
    /// Exports the public key as Privacy-Enhanced Mail (PEM). Not secret — safe to share.
    /// </summary>
    public static string ExportPublicKeyPem(RSA rsa)
    {
        ArgumentNullException.ThrowIfNull(rsa);
        return rsa.ExportSubjectPublicKeyInfoPem();
    }

    /// <summary>
    /// Imports a Privacy-Enhanced Mail (PEM) public key. Caller owns the returned RSA instance.
    /// </summary>
    public static RSA ImportPublicKeyPem(string pem)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pem);

        RSA rsa = RSA.Create();
        try
        {
            rsa.ImportFromPem(pem);
            return rsa;
        }
        catch
        {
            rsa.Dispose();
            throw;
        }
    }

    /// <summary>
    /// Exports the private key as password-encrypted Privacy-Enhanced Mail (PEM). Safe to write to disk.
    /// </summary>
    public static string ExportPrivateKeyPem(RSA rsa, ReadOnlySpan<char> password)
    {
        ArgumentNullException.ThrowIfNull(rsa);
        ArgumentException.ThrowIfNullOrWhiteSpace(password.ToString(), nameof(password));


        if (password.IsEmpty)
            throw new ArgumentException("Password must not be empty.", nameof(password));

        return rsa.ExportEncryptedPkcs8PrivateKeyPem(
            password,
            new PbeParameters(
                PbeEncryptionAlgorithm.Aes256Cbc,
                HashAlgorithmName.SHA256,
                Pbkdf2Iterations));
    }

    /// <summary>
    /// Imports a password-encrypted Privacy-Enhanced Mail (PEM) private key. Caller owns the returned RSA instance.
    /// </summary>
    public static RSA ImportPrivateKeyPem(string pem, ReadOnlySpan<char> password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pem);
        ArgumentException.ThrowIfNullOrWhiteSpace(password.ToString(), nameof(password));

        RSA rsa = RSA.Create();
        try
        {
            rsa.ImportFromEncryptedPem(pem, password);
            return rsa;
        }
        catch
        {
            rsa.Dispose();
            throw;
        }
    }


    /// <summary>
    /// Encrypts the given plain text using the provided RSA public key.
    /// </summary>
    /// <param name="plainBytes">The plain bytes to encrypt.</param>
    /// <param name="rsa">The RSA instance to use for encryption. Must contain a public key.</param>
    /// <returns>The encrypted bytes.</returns>
    public static byte[] RSAEncrypt(byte[] plainBytes, RSA rsa)
    {
        ArgumentNullException.ThrowIfNull(rsa);
        byte[] encryptedBytes = rsa.Encrypt(plainBytes, RSAEncryptionPadding.OaepSHA256);
        return encryptedBytes;
    }

    /// <summary>
    /// Decrypts the given encrypted text using the provided RSA private key.
    /// </summary>
    /// <param name="encryptedBytes">The encrypted bytes to decrypt.</param>
    /// <param name="rsa">The RSA instance to use for decryption. Must contain a private key.</param>
    /// <returns>The decrypted bytes.</returns>
    public static byte[] RSADecrypt(byte[] encryptedBytes, RSA rsa)
    {
        ArgumentNullException.ThrowIfNull(rsa);
        byte[] decryptedBytes = rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.OaepSHA256);
        return decryptedBytes;
    }

    /// <summary>
    /// Signs the given data using the provided RSA private key.
    /// </summary>
    /// <param name="data">The data to sign.</param>
    /// <param name="rsa">The RSA instance to use for signing. Must contain a private key.</param>
    /// <returns>The signature of the data.</returns>
    public static byte[] Sign(byte[] data, RSA rsa)
    {
        ArgumentNullException.ThrowIfNull(rsa);
        return rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pss);
    }

    /// <summary>
    /// Verifies the signature of the given data using the provided RSA public key.
    /// Returns false for any invalid signature, including malformed input.
    /// </summary>
    /// <param name="data">The data whose signature needs to be verified.</param>
    /// <param name="rsa">The RSA instance to use for verification. Must contain a public key.</param>
    /// <param name="signature">The signature to verify.</param>
    public static bool Verify(byte[] data, byte[] signature, RSA rsa)
    {
        ArgumentNullException.ThrowIfNull(rsa);
        ArgumentNullException.ThrowIfNull(data);
        ArgumentNullException.ThrowIfNull(signature);

        try
        {
            return rsa.VerifyData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pss);
        }
        catch (CryptographicException)
        {
            return false;
        }
    }

}
