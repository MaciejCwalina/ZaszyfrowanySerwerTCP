namespace EncryptedChatServer {
    internal enum EPacketFlag : byte {
        KEY,
        MESSAGE,
        USERDISCONNECTED
    }
}
