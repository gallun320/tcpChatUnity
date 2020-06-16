using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace TcpChatServer.Configs
{
    public static class ConfigLoader
    {
        private static Config _config;
        private static string _path = "./config.json";

        public static Config GetConfig()
        {
            if (_config != null) return _config;
            var file = File.ReadAllText(_path);
            var config = JsonSerializer.Deserialize<Config>(file);
            _config = config;
            return config;
        }
    }
}
