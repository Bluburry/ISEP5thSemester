/* eslint-disable prettier/prettier */
import * as fs from 'fs';
import * as forge from 'node-forge';

export class RSADecryptionService {
  private privateKey: forge.pki.PrivateKey;

  constructor() {
    // Use double backslashes or forward slashes for the file path
    const privateKeyPath = "RSAcertificates/private.key";
    // Alternatively, you can use forward slashes
    // const privateKeyPath = "C:/Users/alfre/source/repos/projeto_integrador_lapr5/RSAcertificates/private.pem";
    try {
      if (!fs.existsSync(privateKeyPath)) {
        throw new Error(`File not found: ${privateKeyPath}`);
      }

      const privateKeyPem = fs.readFileSync(privateKeyPath, 'utf8');
      this.privateKey = forge.pki.privateKeyFromPem(privateKeyPem);
    } catch (error) {
      console.error('Error reading private key file:', error.message);
      throw error;
    }
  }

  public decrypt(encryptedData: string): string {
    const encryptedBytes = forge.util.decode64(encryptedData);
    const decryptedBytes = this.privateKey.decrypt(encryptedBytes, 'RSAES-PKCS1-V1_5');
    return forge.util.decodeUtf8(decryptedBytes);
  }
}
