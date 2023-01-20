using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Text.Json;
using System.IO;

namespace PasswordManager
{
    public class CreateServerFile
    {
        public CreateServerFile(string path, string masterPassword, byte[] secretKey)
        {
            // SERVER FILE CONTAINS THE ENCRYPTED VAULT (WHERE ALL PWs ARE STORED)
            // AND IV (INITIALIZATION VECTOR) WHICH IS USED TO ENCRYPT AND DECRYPT

            // VAULT GOES HERE?
            /*
            Dictionary<string, Dictionary<string, byte[]>> server = new Dictionary<string, Dictionary<string, byte[]>>();
            server.Add("IV", new Dictionary<string, byte[]>());
            server.Add("vault", new Dictionary<string, byte[]>());
            */

            // INITIALIZATION VECTOR
            RandomNumberGenerator random = RandomNumberGenerator.Create();
            byte[] sequence = new byte[16];
            random.GetBytes(sequence);

            // ADD IV TO PROPER POSITION IN SERVER-DICT
            //server["IV"].Add("IV", sequence);

            // ADD PLACEHOLDER VALUE TO VAULT IN SERVER DICT
            //Encryptor enc = new Encryptor(masterPassword, secretKey, sequence);
            //server["vault"].Add("AService", enc.encrypt("aPassword"));

            // CREATE A DICT W PLACEHOLDER VALUE TO STUFF INTO SERVER
            Dictionary<string, string> vault = new Dictionary<string, string>();
            vault["aservice"] = "aPassword";

            //CREATE A SERVER FROM CUSTOM DATA TYPE
            Server server = new Server();
            server.InitServer(sequence, vault);
            Encryptor enc = new Encryptor(masterPassword, secretKey, sequence);
            server.encryptVault(enc);

            // CREATE SERVER-FILE
            string jsonServer = JsonSerializer.Serialize(server);
            File.WriteAllText(path, jsonServer);
        }
    }
}
