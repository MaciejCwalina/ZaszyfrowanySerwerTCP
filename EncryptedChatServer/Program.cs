using EncryptedChatServer;
using System.Net;
using System.Net.Sockets;

class Program {
    private static List<User> _connections = new List<User>();
    private static async Task Main(String[] args) {
        TcpListener listener;
        if (args.Length == 1) {
            listener = new TcpListener(IPAddress.Any, int.Parse(args[0]));
        } else {
            listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 9999);
        }


        listener.Start();
        await Console.Out.WriteLineAsync($"Starting Server");
        while (true) {
            for (int i = 0; i < _connections.Count; i++) {
                if (!_connections[i].Connected()) {
                    await Console.Out.WriteLineAsync("Removing client");
                    User disconnectedUser = _connections[i];
                    _connections.RemoveAt(i);
                    foreach (User connection in _connections) {
                        BinaryWriter binaryWriter = new BinaryWriter(connection.tcpClient.GetStream());
                        binaryWriter.Write((Byte)EPacketFlag.USERDISCONNECTED);
                        UserPacket disconnectedUserPacket = new UserPacket();
                        disconnectedUserPacket.SerializeUser(binaryWriter, disconnectedUser);
                    }
                }
            }


            if (listener.Pending()) {
                await Console.Out.WriteLineAsync("Got connecting");
                TcpClient client = await listener.AcceptTcpClientAsync();
                NetworkStream clientStream = client.GetStream();
                BinaryReader binaryReader = new BinaryReader(clientStream);
                UserPacket userPacket = new UserPacket();
                User user = userPacket.DeserializeUser(binaryReader);
                user.tcpClient = client;
                _connections.Add(user);
                foreach (User connection in _connections) {
                    BinaryWriter binaryWriter = new BinaryWriter(connection.tcpClient.GetStream());
                    binaryWriter.Write((Byte)EPacketFlag.KEY);
                    binaryWriter.Write(_connections.Count);
                    binaryWriter.Write(_connections.SelectMany(x => x.publicKey).ToArray());
                }
            }

            HandleIncommingMessageRequest();
        }
    }

    private static void HandleIncommingMessageRequest() {
        foreach (User client in _connections) {
            if (!client.Connected()) {
                continue;
            }

            UserPacket packet = new UserPacket();
            NetworkStream clientStream = client.tcpClient.GetStream();

            if (!clientStream.DataAvailable) {
                continue;
            }

            User user = packet.DeserializeUser(new BinaryReader(clientStream));
            foreach (User toSend in _connections.Where(usr => Enumerable.SequenceEqual(user.publicKey, usr.publicKey))) {
                if (toSend == client || !client.Connected()) {
                    continue;
                }

                NetworkStream toSendStream = toSend.tcpClient.GetStream();
                BinaryWriter toSendBinaryWriter = new BinaryWriter(toSendStream);
                UserPacket sender = new UserPacket();
                toSendStream.WriteByte((Byte)EPacketFlag.MESSAGE);
                sender.SerializeUser(toSendBinaryWriter, user);
            }
        }
    }
}