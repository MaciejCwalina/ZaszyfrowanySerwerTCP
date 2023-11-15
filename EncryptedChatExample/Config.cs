using Newtonsoft.Json;
using System.Text;

namespace EncryptedChatExample {
    internal class Config {
        public String Name { get; set; } = String.Empty;
        public String PathToKeys { get; set; } = String.Empty;
        public async Task StartConfigurationAsync(String username) {
            this.PathToKeys = @$"C:\Users\{Environment.UserName}\Documents\ServerConfig\Keys\";
            this.Name = username;

            Directory.CreateDirectory(@$"C:\Users\{Environment.UserName}\Documents\ServerConfig");
            Directory.CreateDirectory(this.PathToKeys);

            FileStream fileStream = File.Create(@$"C:\Users\{Environment.UserName}\Documents\ServerConfig\config.json");
            String configJson = JsonConvert.SerializeObject(this);
            await fileStream.WriteAsync(Encoding.UTF8.GetBytes(configJson));
            fileStream.Close();
        }

        public static async Task<Config?>LoadConfigurationAsync(String path) {
            String configJson = await File.ReadAllTextAsync(path);
            return JsonConvert.DeserializeObject<Config>(configJson);
        }

    }
}
