using System.Net;
using System.Net.Sockets;

class Program {
    private static TcpListener _listener;
    private static List<TcpClient> _connections = new List<TcpClient>();
    private static async Task Main(String[] args) {
        if(args.Length == 1) {
            _listener = new TcpListener(IPAddress.Any, int.Parse(args[1]));
        } else {
            _listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 9999);
        }

        _listener.Start();
        while(true) {
            if(_listener.Pending()) {
                await Console.Out.WriteLineAsync("Adding");
                _connections.Add(await _listener.AcceptTcpClientAsync());
            }

            await HandleIncommingMessageRequest();
        }

    }

    private static async Task HandleIncommingMessageRequest() {
        foreach(TcpClient client in _connections) {

            NetworkStream networkStream;
            if((networkStream = client.GetStream()).DataAvailable == true) {
                Byte[] bytes = new Byte[256];
                await networkStream.ReadAsync(bytes, 0, bytes.Length);
                foreach(TcpClient tcpClient in _connections) {
                    if(!tcpClient.Connected || client == tcpClient) {
                        continue;
                    }

                    await tcpClient.GetStream().WriteAsync(bytes);
                }
            }
        }
    }
}