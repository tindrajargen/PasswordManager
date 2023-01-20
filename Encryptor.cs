using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace PasswordManager
{
    public class Encryptor
    {
        byte[] vaultKey;
        byte[] IV;

        public Encryptor(string masterPassword, byte[] secretKey, byte[] IV)
        {
            this.IV = IV;
            vaultKey = combineKeys(masterPassword, secretKey);
        }

        private byte[] combineKeys(string key1, byte[] key2)
        {
            // förbättra denna senare med derivebytes
            /*
            byte[] key1bytes = Encoding.ASCII.GetBytes(key1);
            return key1bytes.Concat(key2).ToArray();
            */
            //string key2AsString = Encoding.ASCII.GetString(key2);
            //string combined = key1 + key2AsString;
            return CreateHash(key1, key2);
        }

        private byte[] CreateHash(string input, byte[] secretKey)
        {
            // Generate a salt
            /*
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            byte[] salt = new byte[24];
            provider.GetBytes(salt);
            */
            
            // Generate the hash
            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(input, secretKey, 100000);
            byte[] output = pbkdf2.GetBytes(24);
            //foreach (byte b in output) Console.WriteLine(b.ToString());
            return output;
        }

        public byte[] encrypt(string plainText)
        {
            byte[] encrypted;
            using (Aes aesObj = Aes.Create())
            {
                aesObj.Key = vaultKey;
                aesObj.IV = IV;
                ICryptoTransform encryptor = aesObj.CreateEncryptor(aesObj.Key, aesObj.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using(CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            return encrypted;
        }

        public string decrypt(byte[] encryptedText)
        {
            if (encryptedText == null || encryptedText.Length <= 0)
                throw new ArgumentNullException("encryptedText");
            if (vaultKey == null || vaultKey.Length <= 0)
                throw new ArgumentNullException("vaultKey");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            string plainText = "";
            using (Aes aesObj = Aes.Create())
            {
                aesObj.Key = vaultKey;
                aesObj.IV = IV;
                ICryptoTransform decryptor = aesObj.CreateDecryptor(aesObj.Key, aesObj.IV);
                using (MemoryStream msDecrypt = new MemoryStream(encryptedText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            try
                            {
                                plainText = srDecrypt.ReadToEnd();
                            }
                            catch (CryptographicException)
                            {
                                Console.WriteLine("Incorrect master password.");
                                Environment.Exit(0);
                            }
                        }
                    }
                }
            }
            return plainText;
        }
    }
}
