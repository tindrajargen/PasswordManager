using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;

namespace PasswordManager
{
    public class AccessClientFile
    {
        private Dictionary<string, string> client;

        public AccessClientFile(string atPath)
        {
            // Deserialize the client fron json to dictionary format
            client = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(atPath));
        }

        public string getSecretKey()
        {
            return client["secret"];
        }
    }
}
