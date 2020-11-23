using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Bank_Application
{
    class Bank_employee
    {
        SortedList<string, CustomerInfo> listCustomer;
        FileStream customerFile;

        public Bank_employee(SortedList<string, CustomerInfo> listCustomer, FileStream customerFile)
        {
            this.listCustomer = listCustomer;
            this.customerFile = customerFile;
            string pin = "";
            BankEmployeeInfo employee = new BankEmployeeInfo();
            employee.PinCode = "A1234";

            do
            {
                Console.Clear();
                Console.WriteLine("\n" + "Enter your PIN code : ");
                pin = Console.ReadLine();
            }
            while (pin != employee.PinCode);

            Console.WriteLine("\n" + "Welcome ! ");

            try
            {
                while (true)

                {
                    Console.Clear();
                    Console.WriteLine("\n" + "What do you want to do ?");
                    Console.WriteLine("1- Create a customer account");
                    Console.WriteLine("2- Delete a customer account");
                    Console.WriteLine("3- Make transactions");
                    Console.WriteLine("4- Get the list of customer accounts ");
                    Console.WriteLine("5- Get the list of customer numbers ");
                    Console.WriteLine("\n return to come back \n");

                    string reponse = Console.ReadLine().ToLower();

                    if (reponse != "")
                    {
                        if (reponse == "1")
                        {
                            Console.Clear();
                            CustomerInfo New = CreationAccount();

                            // CREATION SAVING ACCOUNT
                            List<TransactionInfo> savingList = New.SavingAccount.Historic;
                            FileStream savingFile = SerializeAccount(savingList, $"{New.SavingAccount.name}.txt");
                            DeserializeAccount($"{New.SavingAccount.name}.txt");

                            // CREATION CURRENT ACCOUNT 
                            List<TransactionInfo> currentList = New.CurrentAccount.Historic;
                            FileStream currentFile = SerializeAccount(savingList, $"{New.CurrentAccount.name}.txt");
                            DeserializeAccount($"{New.CurrentAccount.name}.txt");

                            // ADD TO CUSTOMERLIST
                            listCustomer.Add(New.Number, New);
                            Console.Clear();

                            // UPDATE COSTUMERFILE
                            customerFile = Serialize(listCustomer);
                            Deserialize();

                            //WRITE A SUMMARY
                            Console.WriteLine($"Compte de {New.FirstName} {New.LastName} est crée.");
                            Console.WriteLine($"Numero de compte : {New.Number} PIN : {New.PinCode}");

                            Console.ReadKey();
                        }
                        if (reponse == "2")
                        {
                            Console.Clear();
                            Console.WriteLine("Who do you want to delete?");
                            Console.WriteLine("[enter account number]");
                            string number = Console.ReadLine().ToLower();
                            CustomerInfo customer = listCustomer[number];

                            if (customer.CurrentAccount.balance == 0 && customer.SavingAccount.balance == 0)
                            {
                                listCustomer.Remove("customer");
                                Console.WriteLine($"You delete the accout {customer.Number} of {customer.FirstName} {customer.LastName}");

                                // UPDAT CUSTOMERFILE
                                customerFile = Serialize(listCustomer);
                                Deserialize();
                            }
                            else
                            {
                                Console.WriteLine("You can't delete this customer.");
                            }

                            Console.ReadKey();

                        }
                        if (reponse == "3")
                        {
                            Console.Clear();
                            transaction(listCustomer);

                            // UPDATE COSTUMERFILE
                            customerFile = Serialize(listCustomer);
                            Deserialize();
                            Console.ReadKey();

                        }
                        if (reponse == "4")
                        {
                            Console.Clear();
                            Console.WriteLine("Here, the list of customer accounts");
                            foreach (KeyValuePair<string, CustomerInfo> eachCustomer in listCustomer)
                            {
                                Console.WriteLine($"{eachCustomer.Value.FirstName} {eachCustomer.Value.LastName} have {eachCustomer.Value.CurrentAccount.balance} in his current account and {eachCustomer.Value.SavingAccount.balance} in his saving account");
                            }
                            Console.ReadKey();

                        }
                        if (reponse == "5")
                        {
                            Console.Clear();
                            Console.WriteLine("Here, the list of customer numbers");
                            foreach (KeyValuePair<string, CustomerInfo> eachCustomer in listCustomer)
                            {
                                Console.WriteLine($"Name : {eachCustomer.Value.FirstName} {eachCustomer.Value.LastName} | Number: {eachCustomer.Value.Number}");
                            }
                            Console.ReadKey();

                        }
                        if (reponse == "return")
                        {
                            break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("////////////////");
                        Console.WriteLine("Error on the answer, do it again!");
                        Console.WriteLine("////////////////");
                    }
                }
            }
            catch (FileNotFoundException e) ///test des exception pour pas que le fichier ne plante
            {
                Console.WriteLine(e.Message);
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadKey();

        }

        static void transaction(SortedList<string, CustomerInfo> listCustomer)
        {
            TransactionInfo New = new TransactionInfo();

            Console.WriteLine("Write the customer Number");
            Console.WriteLine("[enter account number]");
            string number = Console.ReadLine().ToLower();
            New.accountNumber = number;
            CustomerInfo customer = listCustomer[number];

            Console.WriteLine("1- Add          2- Substract");
            string action = Console.ReadLine();
            if (action == "1")
            {
                New.action = enumAction.add;
            }
            else if (action == "2")
            {
                New.action = enumAction.substract;
            }

            Console.WriteLine("How much ?");
            New.moneyTransferred = Convert.ToInt32(Console.ReadLine());
            if (New.action == enumAction.substract)
            {
                New.moneyTransferred = -(New.moneyTransferred);
            }
            New.date = DateTime.Now.ToString("yyyy/MM/dd");

            Console.WriteLine("1- Current account    2- Saving account");
            string accountType = Console.ReadLine();
            if (accountType == "1")
            {
                if (New.moneyTransferred + customer.CurrentAccount.balance < 0)
                {
                    Console.WriteLine($"You can't substract {New.moneyTransferred} on this account");
                }
                else
                {
                    New.account = enumAccount.current;
                    New.finalBalance = customer.CurrentAccount.balance + New.moneyTransferred;
                    customer.CurrentAccount.balance = New.finalBalance;

                    Console.WriteLine($"You have {New.action} on {New.account} account of {customer.FirstName} {customer.LastName} ({New.moneyTransferred} euros) the {New.date}. Final balance is now {New.finalBalance} on this account. ");

                    //UPDATE CURRENTFILE
                    List<TransactionInfo> currentList = customer.CurrentAccount.Historic;
                    currentList.Add(New);
                    FileStream currentFile = SerializeAccount(currentList, $"{New.accountNumber}-current.txt");
                    DeserializeAccount($"{New.accountNumber}-current.txt");
                }
            }
            else if (accountType == "2")
            {
                if (New.moneyTransferred + customer.SavingAccount.balance < 0)
                {
                    Console.WriteLine($"You can't substract {New.moneyTransferred} on this account");
                }
                else
                {
                    New.account = enumAccount.saving;
                    New.finalBalance = customer.SavingAccount.balance + New.moneyTransferred;
                    customer.SavingAccount.balance = New.finalBalance;

                    Console.WriteLine($"You have {New.action} on {New.account} account of {customer.FirstName} {customer.LastName} ({New.moneyTransferred} euros) the {New.date}. Final balance is now {New.finalBalance} on this account. ");

                    // UPDATE SAVINGFILE
                    List<TransactionInfo> savingList = customer.SavingAccount.Historic;
                    savingList.Add(New);
                    FileStream currentFile = SerializeAccount(savingList, $"{New.accountNumber}-savings.txt");
                    DeserializeAccount($"{New.accountNumber}-savings.txt");
                }
            }
        }

        private static CustomerInfo CreationAccount()
        {
            CustomerInfo New = new CustomerInfo();

            Console.WriteLine("ACCOUNT CREATION");
            Console.WriteLine("First name of the customer :"); //prenom
            string firstName = Console.ReadLine().ToLower();
            New.FirstName = firstName;

            Console.WriteLine("Last name of the customer :"); //nom
            string lastName = Console.ReadLine().ToLower();
            New.LastName = lastName;

            Console.WriteLine("Mail of the customer :"); //mail
            string mail = Console.ReadLine().ToLower();
            New.Mail = mail;

            //The filename will be called xx-nn-yy-zz
            //xx is the initials of the customer,
            List<char> charLastName = new List<char>(lastName.ToCharArray());
            List<char> charFirstName = new List<char>(firstName.ToCharArray());
            string xx = Convert.ToString(charFirstName[0]) + Convert.ToString(charLastName[0]);

            //nn is the length of the total name (first name and last name,
            string totalName = firstName + lastName;
            int nn = totalName.Length;

            //yy is the alphabetical position of the first initial
            string yy = Convert.ToString(ConvertionLetterInNumber(Convert.ToString(charFirstName[0])));
            //Console.WriteLine(yy);

            //zz is the alphabetical position of the second initial
            string zz = Convert.ToString(ConvertionLetterInNumber(Convert.ToString(charLastName[0])));
            //Console.WriteLine(zz);

            string number = xx + "-" + nn + "-" + yy + "-" + zz;
            New.Number = number;

            string currentAccount = xx + "-" + nn + "-" + yy + "-" + zz + "-current";
            New.CurrentAccount = new CurrentAccount { balance = 0, name = currentAccount, Historic = new List<TransactionInfo>() };
            string savingsAccount = xx + "-" + nn + "-" + yy + "-" + zz + "-savings";
            New.SavingAccount = new SavingAccount { balance = 0, name = savingsAccount, Historic = new List<TransactionInfo>() };

            string PIN = yy + zz;
            New.PinCode = PIN;
            return New;
        }

        static int ConvertionLetterInNumber(string letter)
        {
            int convertion = 0;
            if (letter == "A" || letter == "a")
            {
                convertion = 1;
            }
            else if (letter == "B" || letter == "b")
            {
                convertion = 2;
            }
            else if (letter == "C" || letter == "c")
            {
                convertion = 3;
            }
            else if (letter == "D" || letter == "d")
            {
                convertion = 4;
            }
            else if (letter == "E" || letter == "e")
            {
                convertion = 5;
            }
            else if (letter == "F" || letter == "f")
            {
                convertion = 6;
            }
            else if (letter == "G" || letter == "g")
            {
                convertion = 7;

            }
            else if (letter == "H" || letter == "h")
            {
                convertion = 8;
            }
            else if (letter == "I" || letter == "i")
            {
                convertion = 9;
            }
            else if (letter == "J" || letter == "j")
            {
                convertion = 10;
            }
            else if (letter == "K" || letter == "k")
            {
                convertion = 11;
            }
            else if (letter == "L" || letter == "l")
            {
                convertion = 12;
            }
            else if (letter == "M" || letter == "m")
            {
                convertion = 13;
            }

            else if (letter == "N" || letter == "n")
            {
                convertion = 14;
            }
            else if (letter == "O" || letter == "o")
            {
                convertion = 15;
            }
            else if (letter == "P" || letter == "p")
            {
                convertion = 16;
            }
            else if (letter == "Q" || letter == "q")
            {
                convertion = 17;
            }
            else if (letter == "R" || letter == "r")
            {
                convertion = 18;
            }
            else if (letter == "S" || letter == "s")
            {
                convertion = 19;
            }
            else if (letter == "T" || letter == "t")
            {
                convertion = 20;
            }
            else if (letter == "U" || letter == "u")
            {
                convertion = 21;
            }
            else if (letter == "V" || letter == "v")
            {
                convertion = 22;
            }
            else if (letter == "W" || letter == "w")
            {
                convertion = 23;
            }

            else if (letter == "X" || letter == "x")
            {
                convertion = 24;
            }
            else if (letter == "Y" || letter == "y")
            {
                convertion = 25;
            }
            else if (letter == "Z" || letter == "z")
            {
                convertion = 26;
            }
            return convertion;
        }
        static FileStream Serialize(SortedList<string, CustomerInfo> listCustomer)
        {

            FileStream customerFile = new FileStream("C:/Users/mario/Desktop/ProjetInfo/customers.txt", FileMode.Create);

            BinaryFormatter formatter = new BinaryFormatter();

            try
            {
                formatter.Serialize(customerFile, listCustomer);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to execute. Reason" + e.Message);
            }
            finally
            {
                customerFile.Close();

            }
            return customerFile;
        }
        static void Deserialize()
        {
            SortedList<string, CustomerInfo> listCustomer = new SortedList<string, CustomerInfo>();
            FileStream customerFile = new FileStream("C:/Users/mario/Desktop/ProjetInfo/customers.txt", FileMode.Open);


            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                listCustomer = (SortedList<string, CustomerInfo>)formatter.Deserialize(customerFile);
            }

            catch (SerializationException e)
            {
                Console.WriteLine("Failed to execute. Reason" + e.Message);
            }
            finally
            {
                customerFile.Close();

            }
        }
        static FileStream SerializeAccount(List<TransactionInfo> accountList, string accountName)
        {
            FileStream accountFile = new FileStream($"C:/Users/mario/Desktop/ProjetInfo/{accountName}.txt", FileMode.Create);

            BinaryFormatter formatter = new BinaryFormatter();

            try
            {
                formatter.Serialize(accountFile, accountList);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to execute. Reason" + e.Message);
            }
            finally
            {
                accountFile.Close();

            }
            return accountFile;
        }
        static void DeserializeAccount(string accountName)
        {
            List<TransactionInfo> accountList = new List<TransactionInfo>();
            FileStream accountFile = new FileStream($"C:/Users/mario/Desktop/ProjetInfo/{accountName}.txt", FileMode.Open);

            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                accountList = (List<TransactionInfo>)formatter.Deserialize(accountFile);
            }

            catch (SerializationException e)
            {
                Console.WriteLine("Failed to execute. Reason" + e.Message);
            }
            finally
            {
                accountFile.Close();

            }
        }
    }
}
