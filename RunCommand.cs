using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
namespace PasswordManager
{
    public class RunCommand
    {
        public RunCommand(string[] args)
        {
            switch (args[0].ToLower())
            {
                case "init":
                    // NEW COMMAND INIT OBJECT THAT PROCESSES THE INIT COMMAND
                    runInitCommand(args); // WITH ERROR HANDLING
                    break;
                case "initv":
                    runInitCommand(new string[] { "init", "client.json", "server.json" });
                    break;
                case "create":
                    runCreateCommand(args);
                    break;
                case "get":
                    runGetCommand(args);
                    break;
                case "set":
                    runSetCommand(args);
                    break;
                case "delete":
                    runDeleteCommand(args);
                    break;
                case "secret":
                    // DISPLAY SECRET KEY FROM CLIENT FILE
                    runSecretCommand(args);
                    break;
                default:
                    Console.WriteLine(unknownCommandError(args[0]));
                    break;
            }
        }

        private void runInitCommand(string[] splitCmd)
        {
            try
            {
                string clientFilePath = splitCmd[1];
                string serverFilePath = splitCmd[2];
                //if (verifyFileName(clientFilePath) && verifyFileName(serverFilePath))
                //{
                    string masterPassword = promptMasterPass();
                    //CommandInit ci = new CommandInit(clientFilePath, serverFilePath, masterPassword);
                    CreateClientFile ccf = new CreateClientFile(clientFilePath);
                    CreateServerFile csf = new CreateServerFile(serverFilePath, masterPassword, ccf.getSecretKey());
                    Console.WriteLine("Successfully created new password vault");
                    Console.WriteLine("Client: " + clientFilePath);
                    Console.WriteLine("Server: " + serverFilePath);
                //}
                /*
                else
                {
                    Console.WriteLine("Error: File names must end with the extension \".json\"");
                }
                */
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine(invalidCommandError());
            }
        }
        private void runGetCommand(string[] splitCmd)
        {
            try
            {
                string clientFilePath = splitCmd[1];
                string serverFilePath = splitCmd[2];
                string property = "";
                try { property = splitCmd[3].ToLower(); }
                catch (IndexOutOfRangeException) {  }
                if (File.Exists(clientFilePath) && File.Exists(serverFilePath))
                {
                    //string property = splitCmd[3];
                    string masterPassword = promptMasterPass();
                    //string property = promptServiceName();
                    //property = property.ToLower();
                    //CommandInit ci = new CommandInit(clientFilePath, serverFilePath, masterPassword);
                    AccessVault av = new AccessVault(clientFilePath, serverFilePath, masterPassword);
                    if (property == "")
                    {
                        // PRINT FULL LIST
                        List<string> props = av.getProperties();
                        Console.WriteLine("Properties stored in vault: ");
                        foreach(string item in props)
                        {
                            Console.WriteLine(item);
                        }
                    }
                    else
                    {
                        Console.WriteLine(property + ": " + av.getPassword(property));
                    }
                }
                else
                {
                    Console.WriteLine("Error: One or more of the specified files do not exist");
                }
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine(invalidCommandError());
            }
        }
        private void runCreateCommand(string[] splitCmd)
        {
            try
            {
                string clientFilePath = splitCmd[1];
                string serverFilePath = splitCmd[2];
                if (File.Exists(serverFilePath))
                {
                    string masterPassword = promptMasterPass();

                    //SECRETKEY
                    string secretKey = promptSecretKey();
                    //byte[] secretKeyBytes = Encoding.Unicode.GetBytes(secretKey);
                    //byte[] secretKeyBytes = JsonSerializer.Deserialize<byte[]>(secretKey);
                    try
                    {
                        byte[] secretKeyBytes = Convert.FromBase64String(secretKey);
                        AccessVault av = new AccessVault(secretKeyBytes, serverFilePath, masterPassword);
                        if (av.testDecrypt())
                        {
                            CreateClientFile ccf = new CreateClientFile(clientFilePath, secretKeyBytes);
                            Console.WriteLine("Successfully created a new client at " + clientFilePath);
                        }
                        else
                        {
                            Console.WriteLine("Error: Incorrect master password");
                        }
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("Error: Invalid secret key");
                    }
                }
                else
                {
                    Console.WriteLine("Error: No server file found at the specified path");
                }

            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine(invalidCommandError());
            }
        }
        private void runSetCommand(string[] splitCmd)
        {
            try
            {
                //bool genFlag = false;
                string clientFilePath = splitCmd[1];
                string serverFilePath = splitCmd[2];
                if (File.Exists(clientFilePath) && File.Exists(serverFilePath))
                {
                    string property = splitCmd[3].ToLower();
                    string masterPassword = promptMasterPass();
                    string genFlag = "";
                    string passwordToAdd;
                    try { genFlag = splitCmd[4]; }
                    catch (IndexOutOfRangeException) { }
                    if (genFlag == "-g" || genFlag == "--generate")
                    {
                        passwordToAdd = generateRandomPassword();
                    }
                    else
                    {
                        passwordToAdd = promptPasswordToAdd(property);
                    }
                    AccessVault av = new AccessVault(clientFilePath, serverFilePath, masterPassword);
                    av.setPassword(property, passwordToAdd);
                    Console.WriteLine("Property " + property + " added to vault");
                }
                else
                {
                    Console.WriteLine("Error: One or more of the specified files do not exist");
                }
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine(invalidCommandError());
            }
        }
        private void runDeleteCommand(string[] splitCmd)
        {
            try
            {
                string clientFilePath = splitCmd[1];
                string serverFilePath = splitCmd[2];
                if (File.Exists(clientFilePath) && File.Exists(serverFilePath))
                {
                    string property = splitCmd[3].ToLower();
                    string masterPassword = promptMasterPass();
                    AccessVault av = new AccessVault(clientFilePath, serverFilePath, masterPassword);
                    bool succeeded = av.dropPassword(property);
                    if (succeeded) Console.WriteLine("Property " + property + " removed from vault");
                    else Console.WriteLine("No property found by the name of " + property);
                }
                else
                {
                    Console.WriteLine("Error: One or more of the specified files do not exist");
                }
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine(invalidCommandError());
            }
        }
        private void runSecretCommand(string[] splitCmd)
        {
            try
            {
                // client file path is second parameter of the secret-command
                string clientFilePath = splitCmd[1];
                if (File.Exists(clientFilePath))
                {
                    // Print secret key
                    AccessClientFile acf = new AccessClientFile(clientFilePath);
                    Console.WriteLine(acf.getSecretKey());
                }
                else
                {
                    Console.WriteLine("Error: No client file found at the specified path");
                }
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine(invalidCommandError());
            }
        }

        private string promptMasterPass()
        {
            Console.Write("Please enter your master password: ");
            return Console.ReadLine();
        }
        private string promptServiceName()
        {
            Console.Write("Please enter the name of a service: ");
            return Console.ReadLine();
        }
        private string promptSecretKey()
        {
            Console.Write("Please enter your secret key: ");
            return Console.ReadLine();
        }
        private string promptPasswordToAdd(string property)
        {
            Console.Write("Please enter the password for " + property + ": ");
            return Console.ReadLine();
        }

        private string invalidCommandError()
        {
            return "Invalid command";
        }
        private string unknownCommandError(string cmd)
        {
            return cmd + " is not recognized as a command";
        }
        private string generateRandomPassword()
        {
            /*
            RandomNumberGenerator random = RandomNumberGenerator.Create();
            byte[] sequence = new byte[14]; // Byte array w length 14 maps to 20-character string when using ToBase64String
            random.GetBytes(sequence);
            return Convert.ToBase64String(sequence);
            */

            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var generated = new char[20];
            var random = new Random();
            for (int i = 0; i < generated.Length; i++)
            {
                generated[i] = chars[random.Next(chars.Length)];
            }
            return new string(generated);
        }

        // OBSOLETE
        private bool verifyFileName(string filename)
        {
            char[] arr = filename.ToCharArray();
            Array.Reverse(arr);
            string reversedString = new string(arr);
            string extension = "";
            for(int i=0; i<5; i++)
            {
                extension += reversedString[i];
            }
            if (reversedString.Length > 5 && extension == "nosj.") return true;
            else return false;
        }
    }
}
