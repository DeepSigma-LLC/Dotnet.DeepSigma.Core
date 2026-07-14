using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace DeepSigma.Core.Cryptography.Asymmetric;

/// <summary>
/// Represents common elliptic curves used in cryptography, including NIST curves and Curve25519/Ed25519.
/// </summary>
public enum ECCurvesCommon
{
    /// <summary>
    /// NIST P-256 curve (also known as secp256r1 or prime256v1). 
    /// Most commonly used curve for ECDSA and widely supported across platforms.
    /// Developed by the NSA, so some organizations prefer to avoid it due to potential concerns about backdoors.
    /// </summary>
    NistP256,
    /// <summary>
    /// NIST P-384 curve (also known as secp384r1).
    /// Provides a higher level of security compared to P-256, suitable for applications requiring stronger security.
    /// Similar to 192-bit RSA in terms of security level, often used in government and military applications.
    /// Developed by the NSA, so some organizations prefer to avoid it due to potential concerns about backdoors.
    /// </summary>
    NistP384,
    /// <summary>
    /// NIST P-521 curve (also known as secp521r1).
    /// Provides a higher level of security compared to P-256 and P-384.
    /// Often used in applications requiring long-term security, such as government and military communications.
    /// Developed by the NSA, so some organizations prefer to avoid it due to potential concerns about backdoors.
    /// </summary>
    NistP521,
    /// <summary>
    /// SECG secp256k1 curve, commonly used in cryptocurrencies like Bitcoin and Ethereum.
    /// Not recommended for general-purpose cryptography due to its specific design and widespread use in blockchain applications.
    /// </summary>
    secp256k1,
    /// <summary>
    /// Curve25519, designed for high performance and security, often used for key exchange (X25519).
    /// Commonly used in modern cryptographic protocols like Signal and TLS 1.3 as an alternative to traditional NIST elliptic curves.
    /// </summary>
    Curve25519,
    /// <summary>
    /// Ed25519, a high-security digital signature algorithm based on Curve25519.
    /// </summary>
    Ed25519
}

/// <summary>
/// Provides extension methods for the ECCurvesCommon enum to convert to ECCurve instances.
/// </summary>
public static class ECCurvesCommonExtensions
{
    /// <summary>
    /// Converts the specified ECCurvesCommon value to its corresponding ECCurve instance.
    /// </summary>
    /// <param name="curve">The ECCurvesCommon value to convert.</param>
    /// <returns>The corresponding ECCurve instance.</returns>
    public static ECCurve ToECCurve(this ECCurvesCommon curve) => curve switch
    {
        ECCurvesCommon.NistP256 => ECCurve.NamedCurves.nistP256,
        ECCurvesCommon.NistP384 => ECCurve.NamedCurves.nistP384,
        ECCurvesCommon.NistP521 => ECCurve.NamedCurves.nistP521,
        ECCurvesCommon.secp256k1 => ECCurve.CreateFromFriendlyName("secp256k1"),
        // Note: Curve25519 and Ed25519 are not directly supported by System.Security.Cryptography.ECCurve.
        // They require specialized libraries or implementations for proper usage.
        _ => throw new ArgumentOutOfRangeException(nameof(curve), curve, "Unsupported elliptic curve.")
    };
}
