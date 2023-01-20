using System;

/*

PASSWORD MANAGER
PG1 - Fredrik Gustafsson, Tindra Järgenstedt, Nelly Kvernplassen

 */

namespace PasswordManager
{
    class Program
    {
        static void Main(string[] args)
        {
            //MainMenu mm = new MainMenu();
            RunCommand rc = new RunCommand(args);
            //CreateClientFile ccf = new CreateClientFile("Hej");
            //CreateServerFile csf = new CreateServerFile("path");

        }
    }
}