using System.Net.Sockets;

namespace EncryptedChatServer {
    internal class User {
        public TcpClient tcpClient;
        public String name = String.Empty;
        public Byte[] messageContent = new byte[256];
        public Byte[] publicKey = new Byte[294];

        public Boolean Connected() {
            try {
                if (this.tcpClient.Client.Poll(0, SelectMode.SelectRead)) {
                    byte[] checkConn = new byte[1];

                    if (this.tcpClient.Client.Receive(checkConn, SocketFlags.Peek) == 0) {
                        return false;
                    }
                }

                return true;
            } catch (Exception) {
                return false;
            }
        }
    }
}
