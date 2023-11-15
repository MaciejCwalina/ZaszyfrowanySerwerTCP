namespace EncryptedChatExample {
    internal class UserPacket {

        public void SerializeUser(BinaryWriter binaryWriter, User user) {
            binaryWriter.Write(user.name);
            binaryWriter.Write(user.messageContent);
            binaryWriter.Write(user.publicKey);
        }

        public void SendKey(BinaryWriter binaryWriter, User user) {
            binaryWriter.Write(user.publicKey);
        }

        public EPacketFlag GetPacketFlag(BinaryReader binaryReader) {
            return (EPacketFlag)binaryReader.ReadByte();
        }

        public Byte[] GetPublicKey(BinaryReader binaryReader) {
            return binaryReader.ReadBytes(294);
        }

        public User DeserializeUser(BinaryReader binaryReader) {
            String name = binaryReader.ReadString();
            Byte[] message = binaryReader.ReadBytes(256);
            Byte[] publicKey = binaryReader.ReadBytes(294);
            return new User {
                name = name,
                messageContent = message,
                publicKey = publicKey
            };
        }
    }
}
