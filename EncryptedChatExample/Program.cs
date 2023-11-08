using System.Net;
using System.Net.Sockets;
using System.Text;

class Program {
    private static async Task Main() {
        RSAWrapper rsa = new RSAWrapper();
        //Niech Pan Zmieni lokalizacje klucza RSA bo to jest sciezka z mojego komputera
        rsa.LoadFromFile(@"C:\Users\user1\Klucze\keypair.pem");
        TcpClient tcpClient = new TcpClient();
        await tcpClient.ConnectAsync(IPAddress.Parse("127.0.0.1"), 9999);
        NetworkStream stream = tcpClient.GetStream();


        _ = Task.Run(async () => {
            while(true) {
                byte[] buffer = new byte[256];
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                if(bytesRead == 0) {
                    Console.WriteLine("Server disconnected.");
                    break;
                }

                Console.WriteLine(Encoding.UTF8.GetString(rsa.Decrypt(buffer)));
            }
        });

        while(true) {
            var input = await Console.In.ReadLineAsync();
            byte[] encryptedBytes = rsa.Encrypt(Encoding.UTF8.GetBytes(input));
            await stream.WriteAsync(encryptedBytes);
        }
    }
}