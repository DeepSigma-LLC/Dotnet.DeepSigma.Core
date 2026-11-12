using Xunit;
using System.Security.Cryptography;
using DeepSigma.Core.Cryptography.Symmetric;
using DeepSigma.Core.Cryptography.Asymmetric;
using DeepSigma.Core.Cryptography;

namespace DeepSigma.Core.Tests.Tests;

public class Crypto_Tests
{
    [Fact]
    public void AESTest()
    {
        byte[] key = AesCryptography.GenerateAESKey();
        string message = "This is a secret message.";
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(message);
        byte[] encrypted = AesCryptography.AESEncrypt(bytes, key);
        byte[] decrypted = AesCryptography.AESDecrypt(encrypted, key);
        string encrypted_text = System.Text.Encoding.UTF8.GetString(encrypted);
        
        Assert.Equal(message, System.Text.Encoding.UTF8.GetString(decrypted));
        Assert.NotEqual(message, encrypted_text);
    }

    [Fact]
    public void RSATest()
    {
        RSA keys = RsaCryptography.GenerateRSAKeys(2048);
        string message = "This is a secret message.";
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(message);
        byte[] encrypted = RsaCryptography.RSAEncrypt(bytes, keys);
        byte[] decrypted = RsaCryptography.RSADecrypt(encrypted, keys);
        string decrypted_text = System.Text.Encoding.UTF8.GetString(decrypted);

        Assert.Equal(message, decrypted_text);
        string encrypted_string = System.Text.Encoding.UTF8.GetString(encrypted);
        Assert.NotEqual(message, encrypted_string);
    }

    [Fact]
    public void EllipticCurveSignitureShouldBeTrue()
    {
        ECDsa wrong_key = EllipticCurveCryptography.CreateKeyPair();
        ECDsa keys = EllipticCurveCryptography.CreateKeyPair();
        string message = "This is a secret message.";
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(message);

        byte[] signature = EllipticCurveCryptography.Sign(bytes, keys);
        bool valid_sig = EllipticCurveCryptography.Verify(bytes, signature, keys);
        bool invalid_sig = EllipticCurveCryptography.Verify(bytes, signature, wrong_key);

        Assert.True(valid_sig);
        Assert.False(invalid_sig);
    }
}
