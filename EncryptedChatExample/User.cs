namespace EncryptedChatExample {
    internal class User {
        public String name = String.Empty;
        public Byte[] messageContent = new byte[256];
        public Byte[] publicKey = new Byte[294];
    }
}
