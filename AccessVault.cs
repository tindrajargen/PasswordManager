using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json;

namespace PasswordManager
{
    public class AccessVault
    {
        string client;
        string server;
        string masterPass;

        // Used only if created by overloaded constructor
        byte[] secretKey;

        public AccessVault(string clientPath, string serverPath, string masterPassword)
        {
            client = clientPath;
            server = serverPath;
            masterPass = masterPassword;
        }
        // OVERLOADED CONSTRUCTOR
        public AccessVault(byte[] secretKey, string serverPath, string masterPassword)
        {
            this.secretKey = secretKey;
            server = serverPath;
            masterPass = masterPassword;
        }

        public string getPassword(string property)
        {
            // GET SECRET KEY FROM CLIENT FILE
            Dictionary<string, byte[]> clientFromJson = JsonSerializer.Deserialize<Dictionary<string, byte[]>>(File.ReadAllText(client));
            byte[] secretKey = clientFromJson["secret"];

            // DESERIALIZE JSON

            Server serv = JsonSerializer.Deserialize<Server>(File.ReadAllText(server));
            byte[] IV = serv.IV;
            Encryptor enc = new Encryptor(masterPass, secretKey, IV);
            serv.decryptVault(enc);
            if(serv.hasProperty(property)) return serv.getPassword(property);
            else return "Error: No service found by the name of " + property;


            /*
            Dictionary<string, Dictionary<string, byte[]>> vaultFromJson = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, byte[]>>>(File.ReadAllText(server));
            Dictionary<string, byte[]> outerDict = vaultFromJson["IV"];
            byte[] IV = outerDict["IV"];
            //try
            //{
                Console.WriteLine("We here");
                byte[] encryptedPass = (vaultFromJson["vault"])[property];
                Console.WriteLine("We all the way here");
                Encryptor enc = new Encryptor(masterPass, secretKey, IV);
                return enc.decrypt(encryptedPass);
            //}
            //catch
            //{
                //return "Error: No service found by the name of " + property;
            //}
            */
        }

        public bool testDecrypt()
        {
            Server serv = JsonSerializer.Deserialize<Server>(File.ReadAllText(server));
            byte[] IV = serv.IV;
            Encryptor enc = new Encryptor(masterPass, this.secretKey, IV);
            serv.decryptVault(enc);
            if (serv.getVault() == null)
            {
                return false;
            }
            else return true;
        }

        public bool dropPassword(string property)
        {
            bool succeeded = false;
            // GET SECRET KEY FROM CLIENT FILE
            Dictionary<string, byte[]> clientFromJson = JsonSerializer.Deserialize<Dictionary<string, byte[]>>(File.ReadAllText(client));
            byte[] secretKey = clientFromJson["secret"];

            //ACCESS SERVER FILE
            Server serv = JsonSerializer.Deserialize<Server>(File.ReadAllText(server));
            byte[] IV = serv.IV;
            Encryptor enc = new Encryptor(masterPass, secretKey, IV);
            serv.decryptVault(enc);
            if (serv.hasProperty(property))
            {
                serv.dropItemAt(property);
                succeeded = true;
            }
            serv.encryptVault(enc);

            // OVERWRITE SERVER FILE
            string jsonServer = JsonSerializer.Serialize(serv);
            File.WriteAllText(server, jsonServer);

            return succeeded;

            //return serv.getPassword(property);
        }
        public void setPassword(string property, string password)
        {
            // GET SECRET KEY FROM CLIENT FILE
            Dictionary<string, byte[]> clientFromJson = JsonSerializer.Deserialize<Dictionary<string, byte[]>>(File.ReadAllText(client));
            byte[] secretKey = clientFromJson["secret"];

            //ACCESS SERVER FILE
            Server serv = JsonSerializer.Deserialize<Server>(File.ReadAllText(server));
            byte[] IV = serv.IV;
            Encryptor enc = new Encryptor(masterPass, secretKey, IV);
            serv.decryptVault(enc);
            serv.addItemAt(property, password);
            serv.encryptVault(enc);

            // OVERWRITE SERVER FILE
            string jsonServer = JsonSerializer.Serialize(serv);
            File.WriteAllText(server, jsonServer);
        }
        public List<string> getProperties()
        {
            // GET SECRET KEY FROM CLIENT FILE
            Dictionary<string, byte[]> clientFromJson = JsonSerializer.Deserialize<Dictionary<string, byte[]>>(File.ReadAllText(client));
            byte[] secretKey = clientFromJson["secret"];

            //ACCESS SERVER FILE
            Server serv = JsonSerializer.Deserialize<Server>(File.ReadAllText(server));
            byte[] IV = serv.IV;
            Encryptor enc = new Encryptor(masterPass, secretKey, IV);
            serv.decryptVault(enc);
            return new List<string>(serv.getVault().Keys);
        }
    }
}
