using System.Security.Cryptography;

internal class RSAWrapper {
    private RSA _rsa;
    public RSAWrapper() {
        this._rsa = RSA.Create();
    }

    public void LoadFromFile(String path) {
        this._rsa.ImportFromPem(File.ReadAllText(path));
    }

    public Byte[] Encrypt(Byte[] toEncrypt) {
        return this._rsa.Encrypt(toEncrypt, RSAEncryptionPadding.OaepSHA256);
    }

    public Byte[] Decrypt(Byte[] toDecrypt) {
        return this._rsa.Decrypt(toDecrypt, RSAEncryptionPadding.OaepSHA256);
    }
}
