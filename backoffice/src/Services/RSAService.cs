using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

public class RSAEncryptionService : IDisposable
{
    private readonly RSA _rsa;

    public RSAEncryptionService(string certificatePath, string password = null)
    {
        _rsa = RSA.Create();

        if (certificatePath.EndsWith("RSAcertificates\\private.key")) // Private key
        {
            var cert = new X509Certificate2(certificatePath, password, X509KeyStorageFlags.Exportable);
            _rsa.ImportRSAPrivateKey(cert.PrivateKey.ExportPkcs8PrivateKey(), out _);
        }
        else if (certificatePath.EndsWith("RSAcertificates\\public.pem")) // Public key
        {
            string pemContent = System.IO.File.ReadAllText(certificatePath);
            _rsa.ImportFromPem(pemContent.ToCharArray());
        }
        else
        {
            throw new ArgumentException("Unsupported certificate format. Use .pem for public or .pfx for private.");
        }
    }

    public byte[] Encrypt(string plaintext)
    {
        byte[] dataToEncrypt = Encoding.UTF8.GetBytes(plaintext);
        return _rsa.Encrypt(dataToEncrypt, RSAEncryptionPadding.Pkcs1);
    }

    public string Decrypt(byte[] encryptedData)
    {
        byte[] decryptedBytes = _rsa.Decrypt(encryptedData, RSAEncryptionPadding.Pkcs1);
        return Encoding.UTF8.GetString(decryptedBytes);
    }

    public void Dispose()
    {
        _rsa.Dispose();
    }
}
