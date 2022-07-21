using Freelance.Services.Interfaces;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Freelance.Services
{
    public class CertificateService : ICertificateService
    {
        public X509Certificate2 LoadCertificate(string subject)
        {
            var userStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            X509Certificate2 cert = null;

            userStore.Open(OpenFlags.OpenExistingOnly);
            if (userStore.IsOpen)
            {
                var collection = userStore.Certificates.Find(X509FindType.FindBySubjectName, subject, false);
                if (collection.Count > 0)
                {
                    cert = collection[0];
                }
                userStore.Close();
            }
            
            if(cert == null)
            {
                cert = GenerateCertificate(subject);
                SaveCertificate(cert);
                return cert;
            }

            var certEpirationDate = Convert.ToDateTime(cert.GetExpirationDateString());

            if (certEpirationDate < DateTime.UtcNow)
            {
                RemoveCertificate(cert);

                cert = GenerateCertificate(subject);
                SaveCertificate(cert);
            }

            return cert;
        }

        public string GetPublicKey(X509Certificate2 cert)
        {
            return BuildPublicKeyPem(cert);
        }

        public string TryDecrypt(X509Certificate2 cert, string input)
        {
            using (var rsa = cert.GetRSAPrivateKey())
            {
                try
                {
                    var data = rsa.Decrypt(Convert.FromBase64String(input), RSAEncryptionPadding.Pkcs1);
                    return Encoding.UTF8.GetString(data);
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        private X509Certificate2 GenerateCertificate(string subject)
        {

            var random = new SecureRandom();
            var certificateGenerator = new X509V3CertificateGenerator();

            var serialNumber = BigIntegers.CreateRandomInRange(BigInteger.One, BigInteger.ValueOf(Int64.MaxValue), random);
            certificateGenerator.SetSerialNumber(serialNumber);

            certificateGenerator.SetIssuerDN(new X509Name($"C=NL, O=SomeCompany, CN={subject}"));
            certificateGenerator.SetSubjectDN(new X509Name($"C=NL, O=SomeCompany, CN={subject}"));
            certificateGenerator.SetNotBefore(DateTime.UtcNow);
            certificateGenerator.SetNotAfter(DateTime.UtcNow.AddSeconds(15));

            const int strength = 2048;
            var keyGenerationParameters = new KeyGenerationParameters(random, strength);
            var keyPairGenerator = new RsaKeyPairGenerator();
            keyPairGenerator.Init(keyGenerationParameters);

            var subjectKeyPair = keyPairGenerator.GenerateKeyPair();
            certificateGenerator.SetPublicKey(subjectKeyPair.Public);

            var issuerKeyPair = subjectKeyPair;
            const string signatureAlgorithm = "SHA256WithRSA";
            var signatureFactory = new Asn1SignatureFactory(signatureAlgorithm, issuerKeyPair.Private);
            var bouncyCert = certificateGenerator.Generate(signatureFactory);

            X509Certificate2 certificate;

            Pkcs12Store store = new Pkcs12StoreBuilder().Build();
            store.SetKeyEntry($"{subject}_key", new AsymmetricKeyEntry(subjectKeyPair.Private), new[] { new X509CertificateEntry(bouncyCert) });
            string exportpw = Guid.NewGuid().ToString("x");

            using (var ms = new MemoryStream())
            {
                store.Save(ms, exportpw.ToCharArray(), random);
                certificate = new X509Certificate2(ms.ToArray(), exportpw, X509KeyStorageFlags.Exportable);
            }

            return certificate;
        }

        private void SaveCertificate(X509Certificate2 certificate)
        {
            var userStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            userStore.Open(OpenFlags.ReadWrite);
            userStore.Add(certificate);
            userStore.Close();
        }

        private void RemoveCertificate(X509Certificate2 certificate)
        {
            var userStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            userStore.Open(OpenFlags.ReadWrite);
            userStore.Remove(certificate);
            userStore.Close();
        }

        private static string BuildPublicKeyPem(X509Certificate2 cert)
        {
            byte[] algOid;

            switch (cert.GetKeyAlgorithm())
            {
                case "1.2.840.113549.1.1.1":
                    algOid = new byte[] { 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01 };
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(cert), $"Need an OID lookup for {cert.GetKeyAlgorithm()}");
            }

            byte[] algParams = cert.GetKeyAlgorithmParameters();
            byte[] publicKey = WrapAsBitString(cert.GetPublicKey());

            byte[] algId = BuildSimpleDerSequence(algOid, algParams);
            byte[] spki = BuildSimpleDerSequence(algId, publicKey);

            return PemEncode(spki, "PUBLIC KEY");
        }

        private static string PemEncode(byte[] berData, string pemLabel)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("-----BEGIN ");
            builder.Append(pemLabel);
            builder.AppendLine("-----");
            builder.AppendLine(Convert.ToBase64String(berData, Base64FormattingOptions.InsertLineBreaks));
            builder.Append("-----END ");
            builder.Append(pemLabel);
            builder.AppendLine("-----");

            return builder.ToString();
        }

        private static byte[] BuildSimpleDerSequence(params byte[][] values)
        {
            int totalLength = values.Sum(v => v.Length);
            byte[] len = EncodeDerLength(totalLength);
            int offset = 1;

            byte[] seq = new byte[totalLength + len.Length + 1];
            seq[0] = 0x30;

            Buffer.BlockCopy(len, 0, seq, offset, len.Length);
            offset += len.Length;

            foreach (byte[] value in values)
            {
                Buffer.BlockCopy(value, 0, seq, offset, value.Length);
                offset += value.Length;
            }

            return seq;
        }

        private static byte[] WrapAsBitString(byte[] value)
        {
            byte[] len = EncodeDerLength(value.Length + 1);
            byte[] bitString = new byte[value.Length + len.Length + 2];
            bitString[0] = 0x03;
            Buffer.BlockCopy(len, 0, bitString, 1, len.Length);
            bitString[len.Length + 1] = 0x00;
            Buffer.BlockCopy(value, 0, bitString, len.Length + 2, value.Length);
            return bitString;
        }

        private static byte[] EncodeDerLength(int length)
        {
            if (length <= 0x7F)
            {
                return new byte[] { (byte)length };
            }

            if (length <= 0xFF)
            {
                return new byte[] { 0x81, (byte)length };
            }

            if (length <= 0xFFFF)
            {
                return new byte[]
                {
            0x82,
            (byte)(length >> 8),
            (byte)length,
                };
            }

            if (length <= 0xFFFFFF)
            {
                return new byte[]
                {
            0x83,
            (byte)(length >> 16),
            (byte)(length >> 8),
            (byte)length,
                };
            }

            return new byte[]
            {
        0x84,
        (byte)(length >> 24),
        (byte)(length >> 16),
        (byte)(length >> 8),
        (byte)length,
            };
        }
    }
}