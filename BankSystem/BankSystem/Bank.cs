using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace BankSystem
{
    public class Bank
    {
        public string GetUserId()
        {
            int id = 1;
            string path = @"c:\BankSystem\Accounts.txt";
            if (!File.Exists(path))
            {
                File.Create(path).Close();
            }
            else
            {
                string[] arrLine = File.ReadAllLines(path);
                foreach (var line in arrLine)
                {
                    id++;
                }
            }
            return id.ToString();

        }
        public void WriteHeaderText()
        {
            string headerText = "Bank of Zimbabwe v0.1";
            string creditText = "Credit to Dav Sko, for being my rubber duck";
            WriteAt(Console.WindowWidth / 2 - (headerText.Length / 2), 0, headerText);
            WriteAt(Console.WindowWidth / 2 - (creditText.Length / 2), Console.WindowHeight - 1, creditText);
        }
        public void WriteAt(int x, int y, string str)
        {
            Console.SetCursorPosition(x, y);
            Console.Write(str);
        }
        public void CreateAccount()
        {
            Console.Clear();
            WriteHeaderText();
            // Create a file if one currently doesn't exist.
            string path = @"c:\BankSystem\Accounts.txt";
            if (!File.Exists(path))
            {
                File.Create(path).Close();
            }

            // Print account labels and save user input for later.
            string[] accountLabels = { "ID", "Username", "Password", "Card number", "CVV" };
            string[] accountInformation = new string[accountLabels.Length - 1];
            string userInput = GetUserId() + ";";
            int i = 0;
            foreach (var lines in accountLabels)
            {
                if (i != 0)
                {
                    WriteAt(Console.WindowWidth / 2 - 8, 10 + i, accountLabels[i] + ": ");
                    userInput += Console.ReadLine() + ';';
                }

                i++;
            }

            // Add amount field that starts with 0 and then add account to file.
            userInput = userInput + "0";
            File.AppendAllText(path, userInput + Environment.NewLine);
        }
        public void SignIntoAccount()
        {
            bool isAuthorised = false;
            string path = @"c:\BankSystem\Accounts.txt";
            string[] fileContents = File.ReadAllLines(path);
            string username = string.Empty;
            string balance = string.Empty;
            string accountInfo = string.Empty;
            do
            {
                Console.Clear();
                WriteHeaderText();
                WriteAt(Console.WindowWidth / 2 - 15, 12, "Enter username: ");
                string expected = Console.ReadLine();
                WriteAt(Console.WindowWidth / 2 - 15, 13, "Enter Password: ");
                expected = expected + ";" + Console.ReadLine();

                // Look for account in file.
                foreach (var lines in fileContents)
                {
                    string[] lineSplit = lines.Split(';');
                    string actual = $"{lineSplit[1]};{lineSplit[2]}";
                    if (expected == actual)
                    {
                        isAuthorised = true;
                        accountInfo = lines;
                    }
                }
            } while (!isAuthorised);
            SignedInPage(accountInfo);
        }
        public void SignedInPage(string accountInfo)
        {
            string[] information = accountInfo.Split(';');
            string menuChoice = string.Empty;
            string[] lineSplit = new string[5];
            // Loop so once you've finished an action you'll be brought back here
            do
            {
                string[] arrLine = File.ReadAllLines(@"c:\BankSystem\Accounts.txt");
                foreach (var line in arrLine)
                {
                    lineSplit = line.Split(';');
                    if (lineSplit[0] == information[0])
                    {
                        accountInfo = line;
                        information = lineSplit;
                    }
                }
                Console.Clear();
                WriteHeaderText();
                WriteAt(Console.WindowWidth / 2 - 20, 10, "1. Add balance to account.");
                WriteAt(Console.WindowWidth / 2 - 20, 11, "2. Transfer money to a different account.");
                WriteAt(Console.WindowWidth / 2 - 20, 12, "3. View transfer log.");
                WriteAt(Console.WindowWidth / 2 - 20, 14, $"Hello {information[1]}. Your current balance is: {information[5]} DKK");
                WriteAt(Console.WindowWidth / 2 - 20, 15, "Choose one of the menu options: ");
                menuChoice = Console.ReadLine().ToLower();
                switch (menuChoice)
                {
                    case "1":
                        AddMoneyToAccount(accountInfo);
                        break;
                    case "2":
                        TransferToAccount(accountInfo);
                        break;
                    case "3":
                        ViewAccountLog(accountInfo);
                        break;
                    case "x":
                        Environment.Exit(0);
                        break;
                }
            } while (menuChoice != "x");

        }
        public void AddMoneyToAccount(string accountInfo)
        {
            Console.Clear();
            WriteHeaderText();
            double amount = 0;
            bool canParse = false;
            double balance = 0;
            do
            {
                WriteAt(20, 8, " ".PadRight(Console.WindowWidth - 21));
                WriteAt(20, 8, "Write the amount in DKK that you wish to add to your account's balance: ");
                canParse = double.TryParse(Console.ReadLine(), out amount);
                if (canParse == false)
                {
                    WriteAt(20, 11, "Invalid amount, please enter a number.");
                }
            } while (!canParse);

            string path = @"c:\BankSystem\Accounts.txt";
            string[] fileContents = File.ReadAllLines(path);
            string[] updatedAccount = new string[5];
            foreach (var line in fileContents)
            {
                string[] lineSplit = line.Split(';');
                if (line == accountInfo)
                {
                    updatedAccount = accountInfo.Split(';');
                    balance = amount + Convert.ToDouble(lineSplit[lineSplit.Length - 1]);
                    updatedAccount[updatedAccount.Length - 1] = balance.ToString();
                }
            }
            LineEditor(updatedAccount, path, updatedAccount[0]);
        }
        public void LineEditor(string[] newLine, string path, string lineToEdit)
        {
            // Update a single line
            string updatedLine = string.Empty;
            foreach (var item in newLine)
            {
                updatedLine = updatedLine + item;
                if (item != newLine[newLine.Length - 1])
                {
                    updatedLine = updatedLine + ";";
                }
            }
            string[] arrLine = File.ReadAllLines(path);
            arrLine[Convert.ToInt32(lineToEdit) - 1] = updatedLine;
            File.WriteAllLines(path, arrLine);
        }
        public void TransferToAccount(string accountInfo)
        {
            Console.Clear();
            WriteHeaderText();
            double amount = 0;
            string path = @"c:\BankSystem\Accounts.txt";
            bool canParse = false;
            string[] fileContent = File.ReadAllLines(path);
            string destinationId = string.Empty;
            string targetAccount = string.Empty;

            WriteAt(20, 7, "Type the account number for the account you wish to transfer to: ");
            string userInput = Console.ReadLine();

            // Find row that matches the account user wants to transfer to, accept amount
            foreach (var line in fileContent)
            {
                string[] lineSplit = line.Split(';');
                if (lineSplit[3] == userInput)
                {
                    targetAccount = line;
                    destinationId = lineSplit[0];
                    do
                    {
                        WriteAt(20, 8, " ".PadRight(Console.WindowWidth - 21));
                        WriteAt(20, 8, "Write the amount in DKK that you wish to transfer: ");
                        canParse = double.TryParse(Console.ReadLine(), out amount);
                        if (canParse == false)
                        {
                            WriteAt(20, 11, "Invalid amount, please enter a number.");
                        }
                    } while (!canParse);
                }
                else
                {
                    bool isIt = false;
                    foreach (var lines in fileContent)
                    {

                        string[] verify = lines.Split(';');
                        if (verify[3] == userInput)
                        {
                            isIt = true;
                        }
                    }
                    if (isIt == false)
                    {
                        WriteAt(20, 12, "Couldn't find the account in the database.");
                        Thread.Sleep(800);
                        Environment.Exit(0);
                    }
                }
            }
            // Send information to edit multiple lines method
            string sourceId = accountInfo.Substring(0, 1);
            EditMultipleLines(path, accountInfo, targetAccount, sourceId, destinationId, amount);


        }
        public void EditMultipleLines(string path, string accountInfo, string targetAccount, string sourceLineNumber, string destinationLineNmber, double amount)
        {
            string sourceLine = string.Empty;
            string destinationLine = string.Empty;

            // Update source account and turn into simple string for writing to file
            string[] sourceAccount = accountInfo.Split(';');
            double sourceBalance = Convert.ToDouble(sourceAccount[5]);
            sourceBalance = sourceBalance - amount;
            sourceAccount[5] = sourceBalance.ToString();
            foreach (var item in sourceAccount)
            {
                sourceLine = sourceLine + item;
                if (item != sourceAccount[sourceAccount.Length - 1])
                {
                    sourceLine = sourceLine + ";";
                }
            }

            // Update target account and turn into simple string for writing to file
            string[] tarAccount = targetAccount.Split(';');
            double targetBalance = Convert.ToDouble(tarAccount[5]);
            targetBalance = targetBalance + amount;
            tarAccount[5] = targetBalance.ToString();
            foreach (var item in tarAccount)
            {
                destinationLine = destinationLine + item;
                if (item != tarAccount[tarAccount.Length - 1])
                {
                    destinationLine = destinationLine + ";";
                }
            }

            // Take all lines into an array and edit based on ID and rewrite updated array to file.
            string[] arrLine = File.ReadAllLines(path);
            arrLine[Convert.ToInt32(sourceLineNumber) - 1] = sourceLine;
            arrLine[Convert.ToInt32(destinationLineNmber) - 1] = destinationLine;
            File.WriteAllLines(path, arrLine);

            // Add transaction to both log files for source and destination accounts
            string sourcePath = @"c:\BankSystem\" + sourceAccount[0] + sourceAccount[1] + "Log.txt";
            if (!File.Exists(sourcePath))
            {
                File.Create(sourcePath).Close();
            }
            File.AppendAllText(sourcePath, $"You have transferred {amount} DKK to account: {tarAccount[3]}" + Environment.NewLine);
            string destinationPath = @"c:\BankSystem\" + tarAccount[0] + tarAccount[1] + "Log.txt";
            if (!File.Exists(destinationPath))
            {
                File.Create(destinationPath).Close();
            }
            File.AppendAllText(destinationPath, $"You have received {amount} DKK from: {sourceAccount[1]}" + Environment.NewLine);
        }
        public void ViewAccountLog(string accountInfo)
        {
            Console.Clear();
            string[] loginInformation = accountInfo.Split(';');
            string[] fileContent = File.ReadAllLines(@"c:\BankSystem\" + loginInformation[0] + loginInformation[1] + "Log.txt");
            int i = 0;
            foreach (var line in fileContent)
            {
                WriteAt(Console.WindowWidth / 2 - (line.Length / 2), 5 + i, line);
                if (i > 14)
                {
                    WriteAt(Console.WindowWidth / 2 - (38 / 2), 7 + i, "Press any key to go to the next page: ");
                    Console.ReadKey();
                    Console.Clear();
                    i = 0;
                }
                i++;
            }
            Console.ReadKey();
        }
    }
}
