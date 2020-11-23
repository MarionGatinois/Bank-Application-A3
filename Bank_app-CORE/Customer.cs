using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Bank_Application
{
    class Customer
    {
        FileStream customerFile;
        SortedList<string, CustomerInfo> listCustomer;

        public Customer(SortedList<string, CustomerInfo> listCustomer, FileStream customerFile)
        {
            this.customerFile = customerFile;
            this.listCustomer = listCustomer;

            Console.Clear();

            try
            {
                Console.WriteLine("\n" + "Enter your first name : ");
                string firstName = Console.ReadLine().ToLower();
                Console.WriteLine("\n" + "Enter your last name : ");
                string name = Console.ReadLine().ToLower();
                Console.WriteLine("\n" + "Enter your account number : ");
                string accountNumber = Console.ReadLine().ToLower();
                Console.WriteLine("\n" + "Enter your pin code : ");
                string pinCode = Console.ReadLine();
                Console.Clear();

                if (listCustomer.ContainsKey(accountNumber) == true)
                {
                    if (listCustomer[accountNumber].FirstName == firstName)
                    {
                        if (listCustomer[accountNumber].PinCode == pinCode)
                        {
                            if (listCustomer[accountNumber].LastName == name)
                            {
                                Console.WriteLine("\n" + "Welcome ! ");
                                CustomerInfo customer = listCustomer[accountNumber];
                                try
                                {
                                    while (true)

                                    {
                                        Console.Clear();
                                        Console.WriteLine("\n" + "What do you want to do ?");
                                        Console.WriteLine("1- See my history");
                                        Console.WriteLine("2- Make transactions");
                                        Console.WriteLine("return to stop");

                                        string reponse = Console.ReadLine();

                                        if (reponse != "")
                                        {

                                            if (reponse == "1")
                                            {
                                                Console.Clear();
                                                Console.WriteLine("Which file ?  ");
                                                Console.WriteLine("1 - saving     2 - current");
                                                string answer = Console.ReadLine();
                                                enumAccount account;
                                                if (answer == "1")
                                                {
                                                    account = enumAccount.saving;
                                                    historique(account, customer);
                                                }
                                                if (answer == "2")
                                                {
                                                    account = enumAccount.current;
                                                    historique(account, customer);
                                                }
                                                else
                                                {
                                                    Console.WriteLine("incorrect answer");
                                                }

                                            }
                                            if (reponse == "2")
                                            {
                                                Console.Clear();
                                                transaction(customer);

                                                // UPDATE COSTUMERFILE
                                                customerFile = Serialize(listCustomer);
                                                Deserialize();
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
                            }
                            else
                            {
                                Console.WriteLine("Error with your last name");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Error with your PIN code");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error with your first name");
                    }
                }
                else
                {
                    Console.WriteLine("Error with your account number");
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

        static void historique(enumAccount account, CustomerInfo customer)
        {
            Console.Clear();
            try
            {
                if (account == enumAccount.current)
                {
                    Console.WriteLine("HISTORY OF YOUR CURRENT ACCOUNT:");
                    foreach (TransactionInfo transaction in customer.CurrentAccount.Historic)
                    {
                        Console.WriteLine($"Date: {transaction.date} Action: {transaction.action} Amount: {transaction.moneyTransferred} Final balance :{transaction.finalBalance}");
                    }
                }
                else if (account == enumAccount.saving)
                {
                    Console.WriteLine("HISTORY OF YOUR SAVING ACCOUNT:");
                    foreach (TransactionInfo transaction in customer.SavingAccount.Historic)
                    {
                        Console.WriteLine($"Date: {transaction.date} Action: {transaction.action} Amount: {transaction.moneyTransferred} Final balance :{transaction.finalBalance}");
                    }
                }
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally { Console.WriteLine("Executing finally block."); }
        }

        static void transaction(CustomerInfo customer)
        {
            TransactionInfo New = new TransactionInfo();

            New.accountNumber = customer.Number;
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

            Console.WriteLine("How many ?");
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
                    Console.WriteLine($"You have {New.action} on your {New.account} account ({New.moneyTransferred} euros) the {New.date}. Your final balance is now {New.finalBalance} on this account. ");

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
                    Console.WriteLine($"You can substract {New.moneyTransferred} on this account");
                }
                else
                {
                    New.account = enumAccount.saving;
                    New.finalBalance = customer.SavingAccount.balance + New.moneyTransferred;
                    customer.SavingAccount.balance = New.finalBalance;
                    Console.WriteLine($"You have {New.action} on your {New.account} account ({New.moneyTransferred} euros) the {New.date}. Your final balance is now {New.finalBalance} on this account. ");

                    //UPDATE SAVINGFILE
                    List<TransactionInfo> savingList = customer.SavingAccount.Historic;
                    savingList.Add(New);
                    FileStream currentFile = SerializeAccount(savingList, $"{New.accountNumber}-savings.txt");
                    DeserializeAccount($"{New.accountNumber}-savings.txt");
                }
            }
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
