using System;
namespace PasswordManager
{
    public class MainMenu
    {
        public MainMenu()
        {
            string cmd = "";
            // exit is used to exit program
            while (!cmd.ToLower().Equals("exit"))
            {
                // WELCOME SCREEN
                Console.Write("Enter command: ");

                // USER INPUT
                cmd = Console.ReadLine();

                // PROCESS COMMAND
                // Split command to access command and arguments separately
                string[] splitCmd = cmd.Split(" ");

                // Process based on command
                switch (splitCmd[0].ToLower())
                {
                    case "init":
                        // NEW COMMAND INIT OBJECT THAT PROCESSES THE INIT COMMAND
                        runInitCommand(splitCmd); // WITH ERROR HANDLING
                        break;
                    case "initv":
                        runInitCommand(new string[]{ "init", "client.json", "server.json", "myMasterPass" });
                        break;
                    case "create":

                        break;
                    case "get":
                        runGetCommand(splitCmd);
                        break;
                    case "set":

                        break;
                    case "delete":

                        break;
                    case "secret":
                        // DISPLAY SECRET KEY FROM CLIENT FILE
                        runSecretCommand(splitCmd);
                        break;
                    default:
                        Console.WriteLine(unknownCommandError(splitCmd[0]));
                        break;
                }
            }
        }

        private void runInitCommand(string[] splitCmd)
        {
            try
            {
                string clientFilePath = splitCmd[1];
                string serverFilePath = splitCmd[2];
                string masterPassword = splitCmd[3];
                //CommandInit ci = new CommandInit(clientFilePath, serverFilePath, masterPassword);
                CreateClientFile ccf = new CreateClientFile(clientFilePath);
                CreateServerFile csf = new CreateServerFile(serverFilePath, masterPassword, ccf.getSecretKey());
            }
            catch(IndexOutOfRangeException)
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
                string property = splitCmd[3];
                string masterPassword = splitCmd[4];
                //CommandInit ci = new CommandInit(clientFilePath, serverFilePath, masterPassword);
                AccessVault av = new AccessVault(clientFilePath, serverFilePath, masterPassword);
                Console.WriteLine(property + ": " + av.getPassword(property));
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
                // Print secret key
                AccessClientFile acf = new AccessClientFile(clientFilePath);
                Console.WriteLine(acf.getSecretKey());
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine(invalidCommandError());
            }
        }



        private string invalidCommandError()
        {
            return "Invalid command";
        }
        private string unknownCommandError(string cmd)
        {
            return cmd + " is not recognized as a command";
        }
    }
}
