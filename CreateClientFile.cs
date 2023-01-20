using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json;

namespace PasswordManager
{
    public class CreateClientFile
    {
        byte[] secretKey;

        public CreateClientFile(string path)
        {
            // CLIENT FILE CONTAINS ONLY SECRET KEY (NOT ENCRYPTED)
            // CREATE SECRET KEY
            RandomNumberGenerator random = RandomNumberGenerator.Create();
            byte[] sequence = new byte[16];
            random.GetBytes(sequence);

            // CREATE CLIENT-FIL (JSON)
            Dictionary<string, byte[]> client = new Dictionary<string, byte[]>();
            client["secret"] = sequence;
            secretKey = sequence;

            string jsonClient = JsonSerializer.Serialize(client);

            File.WriteAllText(path, jsonClient);
        }
        // OVERLOADED CONSTRUCTOR FOR WHEN SECRET KEY IS PROVIDED
        public CreateClientFile(string path, byte[] asecretKey)
        {
            // CREATE CLIENT-FIL (JSON)
            Dictionary<string, byte[]> client = new Dictionary<string, byte[]>();
            client["secret"] = asecretKey;
            secretKey = asecretKey;

            string jsonClient = JsonSerializer.Serialize(client);

            File.WriteAllText(path, jsonClient);
        }

        public byte[] getSecretKey()
        {
            return secretKey;
        }
    }
}
