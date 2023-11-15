using System.Security.Cryptography;
internal class RSAWrapper {
    private RSACryptoServiceProvider _rsa;
    public RSAWrapper() {
        this._rsa = new RSACryptoServiceProvider();
    }

    public void LoadFromFile(String path) {
        this._rsa.ImportFromPem(File.ReadAllText(path));
    }

    public Byte[] Encrypt(Byte[] toEncrypt) {
        return this._rsa.Encrypt(toEncrypt, false);
    }

    public Byte[] Decrypt(Byte[] toDecrypt) {
        return this._rsa.Decrypt(toDecrypt, false);
    }

    public Byte[] GetPublicKey() {
        return this._rsa.ExportSubjectPublicKeyInfo();
    }

    public void ImportPublicKey(Byte[] publicKey) {
        this._rsa.ImportSubjectPublicKeyInfo(publicKey, out int _);
    }
}