using EncryptedChatExample;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

class Program {
    private static async Task Main(String[] args) {
        RSAWrapper rsa = new RSAWrapper();
        rsa.LoadFromFile(@"C:\Users\user1\Klucze\keypair.pem");
        await Console.Out.WriteAsync("Enter Username: ");
        String? username = Console.ReadLine();
        if(username == null) {
            await Console.Out.WriteLineAsync("Username cannot be empty!");
        }

        User user = new User();
        user.Name = username;
        TcpClient tcpClient = new TcpClient();
        if(args.Length == 2) {
            await tcpClient.ConnectAsync(IPAddress.Parse(args[0]), int.Parse(args[1]));
        } else {
            await tcpClient.ConnectAsync(IPAddress.Parse("127.0.0.1"), 9999);
        }

        NetworkStream stream = tcpClient.GetStream();

        _ = Task.Run(async () => {
            while(true) {
                byte[] buffer = new byte[256];
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                if(bytesRead == 0) {
                    Console.WriteLine("Server disconnected.");
                    break;
                }

                User sender = JsonSerializer.Deserialize<User>(Encoding.UTF8.GetString(rsa.Decrypt(buffer)));
                Console.WriteLine($"{sender.Name}: {sender.MessageContent}");
            }
        });

        while(true) {
            user.MessageContent = await Console.In.ReadLineAsync();
            String jsonUser = JsonSerializer.Serialize(user);
            byte[] encryptedBytes = rsa.Encrypt(Encoding.UTF8.GetBytes(jsonUser));
            await stream.WriteAsync(encryptedBytes);
        }
    }
}