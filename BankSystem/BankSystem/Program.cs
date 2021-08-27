using System;

namespace BankSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            Bank bankRef = new Bank();
            string menuChoice = string.Empty;
            do
            {
                Console.Clear();
                bankRef.WriteHeaderText();
                bankRef.WriteAt(Console.WindowWidth / 2 - 15, Console.WindowHeight / 2 - 2, "1. Create new account");
                bankRef.WriteAt(Console.WindowWidth / 2 - 15, Console.WindowHeight / 2 - 1, "2. Sign into existing account.");
                bankRef.WriteAt(Console.WindowWidth / 2 - 15, Console.WindowHeight / 2, "X. Close program");
                bankRef.WriteAt(Console.WindowWidth / 2 - 15, Console.WindowHeight / 2 + 2, "Choose a menu item: ");
                menuChoice = Console.ReadLine().ToLower();

                switch (menuChoice)
                {
                    case "1":
                        bankRef.CreateAccount();
                        break;
                    case "2":
                        bankRef.SignIntoAccount();
                        break;
                    case "x":
                        Environment.Exit(0);
                        break;
                }
            } while (menuChoice != "x");
            
        }
    }
}
