using EncryptedChatExample;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

class Program {
    private static async Task Main(String[] args) {
        RSAWrapper rsa = new RSAWrapper();
        User user = new User();
        Byte[] originalKey;
        if (Directory.Exists(@$"C:\Users\{Environment.UserName}\Documents\ServerConfig")) {
            Config? config = await Config.LoadConfigurationAsync(@$"C:\Users\{Environment.UserName}\Documents\ServerConfig\config.json");
            if (config == null) {
                throw new Exception(@$"Failed To Read Configuration File at:\nC:\Users\{Environment.UserName}\Documents\ServerConfig\config.json\nPlease ensure the file exists");
            }

            user = new User {
                name = config.Name
            };

            rsa.LoadFromFile(config.PathToKeys + "keypair.pem");
            user.publicKey = rsa.GetPublicKey();
            originalKey = user.publicKey;
        } else {
            Config config = new Config();
            await Console.Out.WriteLineAsync("Enter Username");
            String? username = Console.ReadLine();
            await config.StartConfigurationAsync(username!);
            Console.WriteLine("Remember to put in the keys in the keys Folder /Documents/ServerConfig/Keys\nThe configuration is done please re run the application to start using it ! :)");
            return;
        }

        TcpClient tcpClient = new TcpClient();
        if (args.Length == 2) {
            await tcpClient.ConnectAsync(IPAddress.Parse(args[0]), int.Parse(args[1]));
        } else {
            await tcpClient.ConnectAsync(IPAddress.Parse("127.0.0.1"), 9999);
        }

        using NetworkStream stream = tcpClient.GetStream();
        using BinaryWriter writer = new BinaryWriter(stream);
        using BinaryReader reader = new BinaryReader(stream);
        UserPacket userPacket = new UserPacket();
        userPacket.SerializeUser(writer, user);
        List<Byte[]> publicKeys = new List<Byte[]>();
        _ = Task.Run(async () => {
            while (true) {
                if (!stream.DataAvailable) {
                    continue;
                }

                EPacketFlag packetFlag = (EPacketFlag)stream.ReadByte();
                switch (packetFlag) {
                    case EPacketFlag.KEY:
                        publicKeys.Clear();
                        Int32 sizeOfKeyList = reader.ReadInt32();
                        for (int i = 0; i < sizeOfKeyList; i++) {
                            Byte[] key = reader.ReadBytes(294);
                            publicKeys.Add(key);
                        }

                        break;
                    case EPacketFlag.MESSAGE:
                        UserPacket packet = new UserPacket();
                        User sender = packet.DeserializeUser(reader);
                        await Console.Out.WriteLineAsync($"{sender.name}: {Encoding.UTF8.GetString(rsa.Decrypt(sender.messageContent))}");
                        break;
                    case EPacketFlag.USERDISCONNECTED:
                        UserPacket disconnectedUserPacket = new UserPacket();
                        User disconnectedUser = disconnectedUserPacket.DeserializeUser(reader);
                        for (int i = 0; i < publicKeys.Count; i++) {
                            if (!Enumerable.SequenceEqual(publicKeys[i], disconnectedUser.publicKey)) {
                                continue;
                            }

                            publicKeys.RemoveAt(i);
                            break;
                        }

                        break;
                }
            }
        });

        while (true) {
            String? inputText = await Console.In.ReadLineAsync();
            UserPacket packet = new UserPacket();
            foreach (Byte[] key in publicKeys) {
                using RSACryptoServiceProvider rsaCrypto = new RSACryptoServiceProvider();
                rsaCrypto.ImportSubjectPublicKeyInfo(key, out _);
                user.publicKey = key;
                user.messageContent = rsaCrypto.Encrypt(Encoding.UTF8.GetBytes(inputText!), false);
                packet.SerializeUser(writer, user);
            }

            user.publicKey = originalKey;
        }
    }
}