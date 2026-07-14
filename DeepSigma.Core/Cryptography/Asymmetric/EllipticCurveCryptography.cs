using DeepSigma.Core.Encode;
using System.Security.Cryptography;

namespace DeepSigma.Core.Cryptography.Asymmetric;

/// <summary>
/// Handles elliptic curve cryptography (ECC) operations such as signing and verifying data.
/// </summary>
public static class EllipticCurveCryptography
{
    // Tune this: higher = slower for you AND for an attacker guessing the password.
    private const int Pbkdf2Iterations = 600_000;

    /// <summary>
    /// Generates a new ECDsa key pair using the NIST P-256 curve.
    /// </summary>
    /// <returns></returns>
    public static ECDsa CreateKeyPair(ECCurve? curve = null) => ECDsa.Create(curve ?? ECCurve.NamedCurves.nistP256);

    /// <summary>
    /// Signs the given data using the provided ECDsa private key.
    /// </summary>
    /// <param name="data">The data to sign.</param>
    /// <param name="privateKey">The ECDsa private key to use for signing.</param>
    /// <param name="hashAlgorithmName">The hash algorithm to use for signing. Default is SHA256.</param>
    /// <param name="dSASignatureFormat">The signature format to use. Default is DER.</param>
    /// <returns>The encoded signature.</returns>
    public static byte[] Sign(string data, ECDsa privateKey, DSASignatureFormat? dSASignatureFormat = null, HashAlgorithmName? hashAlgorithmName = null)
    {
        byte[] dataBytes = System.Text.Encoding.UTF8.GetBytes(data);
        return Sign(dataBytes, privateKey, dSASignatureFormat ?? DSASignatureFormat.Rfc3279DerSequence, hashAlgorithmName);
    }

    /// <summary>
    /// Signs the given data using the provided ECDsa private key.
    /// </summary>
    /// <param name="data">The data to sign.</param>
    /// <param name="privateKey">The ECDsa private key to use for signing.</param>
    /// <param name="hashAlgorithmName">The hash algorithm to use for signing. Default is SHA256.</param>
    /// <param name="dSASignatureFormat">The signature format to use. Default is Rfc3279DerSequence.</param>
    /// <returns>The encoded signature.</returns>
    public static byte[] Sign(byte[] data, ECDsa privateKey, DSASignatureFormat? dSASignatureFormat = null, HashAlgorithmName? hashAlgorithmName = null)
    {
        ArgumentNullException.ThrowIfNull(privateKey);
        ArgumentNullException.ThrowIfNull(data);
        return privateKey.SignData(data, hashAlgorithmName ?? HashAlgorithmName.SHA256, dSASignatureFormat ?? DSASignatureFormat.Rfc3279DerSequence);
    }

    /// <summary>
    /// Verifies the given signature for the data using the provided ECDsa public key.
    /// </summary>
    /// <param name="data">The data to verify.</param>
    /// <param name="signature">The signature to verify.</param>
    /// <param name="publicKey">The ECDsa public key to use for verification.</param>
    /// <param name="hashAlgorithm">The hash algorithm to use for verification.</param>
    /// <returns></returns>
    public static bool Verify(byte[] data, byte[] signature, ECDsa publicKey, HashAlgorithmName? hashAlgorithm = null)
    {
        ArgumentNullException.ThrowIfNull(publicKey);
        ArgumentNullException.ThrowIfNull(data);
        ArgumentNullException.ThrowIfNull(signature);

        try
        {
            return publicKey.VerifyData(data, signature, hashAlgorithm ?? HashAlgorithmName.SHA256);
        }
        catch (CryptographicException)
        {
            return false;
        }
    }


    /// <summary>
    /// Exports the private key from the given ECDsa instance, encrypting it with the provided password.
    /// </summary>
    /// <param name="key">The ECDsa instance containing the private key.</param>
    /// <param name="password">The password to use for encrypting the private key.</param>
    /// <returns>The encrypted private key as a byte array.</returns>
    /// <exception cref="ArgumentException">Thrown when the password is empty.</exception>
    public static byte[] ExportPrivateKey(ECDsa key, ReadOnlySpan<char> password)
    {
        ArgumentNullException.ThrowIfNull(key);
        if (password.IsEmpty)
            throw new ArgumentException("Password must not be empty.", nameof(password));

        return key.ExportEncryptedPkcs8PrivateKey(
            password,
            new PbeParameters(PbeEncryptionAlgorithm.Aes256Cbc, HashAlgorithmName.SHA256, Pbkdf2Iterations));
    }

    /// <summary>
    /// Exports the public key from the given ECDsa instance.
    /// </summary>
    /// <param name="publicKey">The ECDsa instance containing the public key.</param>
    /// <returns>The public key as a byte array.</returns>
    public static byte[] ExportPublicKey(ECDsa publicKey)
    {
        return publicKey.ExportSubjectPublicKeyInfo();
    }

    /// <summary>
    /// Imports a private key from the given encrypted PKCS#8 byte array, decrypting it with the provided password.
    /// </summary>
    /// <param name="pkcs8">The encrypted PKCS#8 byte array containing the private key.</param>
    /// <param name="password">The password to use for decrypting the private key.</param>
    /// <returns>The ECDsa instance containing the private key.</returns>
    public static ECDsa ImportPrivateKey(byte[] pkcs8, ReadOnlySpan<char> password)
    {
        ArgumentNullException.ThrowIfNull(pkcs8);
        ECDsa key = ECDsa.Create();
        try 
        { 
            key.ImportEncryptedPkcs8PrivateKey(password, pkcs8, out _); 
            return key; 
        }
        catch (CryptographicException)
        { 
            key.Dispose(); 
            throw; 
        }
    }

    /// <summary>
    /// Imports a public key from the given SPKI byte array.
    /// </summary>
    /// <param name="spki">The SPKI byte array containing the public key.</param>
    /// <returns>The ECDsa instance containing the public key.</returns>
    public static ECDsa ImportPublicKey(byte[] spki)
    {
        ArgumentNullException.ThrowIfNull(spki);
        ECDsa key = ECDsa.Create();
        try
        { 
            key.ImportSubjectPublicKeyInfo(spki, out _); 
            return key; 
        }
        catch(CryptographicException)
        { 
            key.Dispose(); 
            throw; 
        }
    }

    /// <summary>
    /// Exports the private key as password-encrypted PEM. Safe to write to disk.
    /// </summary>
    /// <param name="key">The ECDsa instance containing the private key.</param>
    /// <param name="password">The password used to encrypt the private key.</param>
    /// <returns>A PEM-encoded, encrypted PKCS#8 private key.</returns>
    /// <exception cref="ArgumentException">Thrown when the password is empty.</exception>
    public static string ExportPrivateKeyPem(ECDsa key, ReadOnlySpan<char> password)
    {
        ArgumentNullException.ThrowIfNull(key);
        if (password.IsEmpty)
            throw new ArgumentException("Password must not be empty.", nameof(password));

        return key.ExportEncryptedPkcs8PrivateKeyPem(
            password,
            new PbeParameters(PbeEncryptionAlgorithm.Aes256Cbc, HashAlgorithmName.SHA256, Pbkdf2Iterations));
    }

    /// <summary>
    /// Imports a password-encrypted PEM private key. Caller owns the returned ECDsa instance.
    /// </summary>
    /// <param name="pem">The PEM-encoded, encrypted PKCS#8 private key.</param>
    /// <param name="password">The password used to decrypt the private key.</param>
    /// <returns>An ECDsa instance containing the private key.</returns>
    public static ECDsa ImportPrivateKeyPem(string pem, ReadOnlySpan<char> password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pem);

        ECDsa key = ECDsa.Create();
        try
        {
            key.ImportFromEncryptedPem(pem, password);
            return key;
        }
        catch (CryptographicException)
        {
            key.Dispose();
            throw;
        }
    }

    /// <summary>
    /// Exports the public key as PEM. Not secret — safe to share.
    /// </summary>
    public static string ExportPublicKeyPem(ECDsa key)
    {
        ArgumentNullException.ThrowIfNull(key);
        return key.ExportSubjectPublicKeyInfoPem();
    }

    /// <summary>
    /// Imports a PEM public key. Caller owns the returned ECDsa instance.
    /// </summary>
    public static ECDsa ImportPublicKeyPem(string pem)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pem);

        ECDsa key = ECDsa.Create();
        try
        {
            key.ImportFromPem(pem);
            return key;
        }
        catch (CryptographicException)
        {
            key.Dispose();
            throw;
        }
    }
}
