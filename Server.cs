using System;
using System.Collections.Generic;
using System.Text.Json;
namespace PasswordManager
{
    public class Server
    {
        public byte[] IV { get; set; }
        public byte[] encryptedVault { get; set; }
        private Dictionary<string, string> vault;

        /*
        public Server(byte[] IV, Dictionary<string,string> vault)
        {
            this.IV = IV;
            this.vault = vault;
        }
        */
        public void InitServer(byte[] IV, Dictionary<string, string> vault)
        {
            this.IV = IV;
            this.vault = vault;
        }

        public void encryptVault(Encryptor encryptor)
        {
            string vaultAsString = JsonSerializer.Serialize(vault);
            encryptedVault = encryptor.encrypt(vaultAsString);
        }
        public void decryptVault(Encryptor encryptor)
        {
            string decrypted = encryptor.decrypt(encryptedVault);
            Dictionary<string, string> dictFromJson = JsonSerializer.Deserialize<Dictionary<string, string>>(decrypted);
            vault = dictFromJson;
        }
        public string getPassword(string prop)
        {
            return vault[prop];
        }
        public void dropItemAt(string prop)
        {
            vault.Remove(prop);
        }
        public void addItemAt(string prop, string pass)
        {
            vault[prop] = pass;
        }
        public bool hasProperty(string property)
        {
            if (vault.ContainsKey(property)) return true;
            else return false;
        }
        public Dictionary<string, string> getVault()
        {
            return vault;
        }
    }
}
