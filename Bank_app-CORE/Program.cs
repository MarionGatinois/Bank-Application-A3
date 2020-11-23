using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Bank_Application
{
    class Program
    {
        static void Main(string[] args)
        {
            FileStream customerFile;
            SortedList<string, CustomerInfo> listCustomer;

            while (true)
            {
                string curFile = "C:/Users/mario/Desktop/ProjetInfo/customers.txt";
                string answer = (File.Exists(curFile) ? "File exists." : "File does not exist.");

                if (answer == "File exists.")
                {
                    Console.WriteLine(" Do you want to delete all the bank account ?");
                    Console.WriteLine("        1- Yes               2 - No");
                    Console.WriteLine("\n" + "               'end' to stop");
                    string answerUser = Console.ReadLine();

                    if (answerUser == "1")
                    {
                        listCustomer = Deserialize();

                        foreach (KeyValuePair<string, CustomerInfo> eachCustomer in listCustomer)
                        {
                            string currentAccount = eachCustomer.Value.CurrentAccount.name + ".txt";
                            File.Delete(currentAccount);
                            string savingAccount = eachCustomer.Value.SavingAccount.name + ".txt";
                            File.Delete(savingAccount);
                            Console.ReadKey();
                        }
                        File.Delete(@"C:/Users/mario/Desktop/ProjetInfo/jg-13-10-7-current.txt");

                        //CREATION LISTCUSTOMER
                        listCustomer = new SortedList<string, CustomerInfo>();
                        
                        //CREATION CUSTOMERFILE           
                        customerFile = Serialize(listCustomer);
                        Deserialize();

                        menu(customerFile, listCustomer);
                    }
                    if (answerUser == "end")
                    {
                        Console.WriteLine("Thank you and press enter to stop");
                        break;
                    }
                    else if (answerUser == "2")
                    {
                        listCustomer = Deserialize();
                        customerFile = new FileStream("C:/Users/mario/Desktop/ProjetInfo/customers.txt", FileMode.Open);

                        menu(customerFile, listCustomer);
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("//////////////////////////////////");
                        Console.WriteLine("Error on the answer, do it again!");
                        Console.WriteLine("//////////////////////////////////");
                    }
                }
                else if (answer == "File does not exist.")
                {
                    //CREATION LISTCUSTOMER
                    listCustomer = new SortedList<string, CustomerInfo>();

                    //CREATION CUSTOMERFILE           
                    customerFile = Serialize(listCustomer);
                    Deserialize();

                    menu(customerFile, listCustomer);
                }
            }
        }

        static void menu(FileStream customerFile, SortedList<string, CustomerInfo> listCustomer)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("\n" + "   Welcome to your Banking Application ");
                Console.WriteLine("\n" + "               Who are you ? ");
                Console.WriteLine("1 - A bank employee        2 - A customer  ");
                Console.WriteLine("\n" + "               'end' to stop");

                string reponse = Console.ReadLine().ToLower();
                if (reponse != "")
                {
                    if (reponse == "1")

                    {
                        Bank_employee employee = new Bank_employee(listCustomer, customerFile);
                    }

                    if (reponse == "2")

                    {
                        Customer customer = new Customer(listCustomer, customerFile);
                    }

                    if (reponse == "end")
                    {
                        Console.WriteLine("Thank you and press enter to stop");
                        break;
                    }
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("//////////////////////////////////");
                    Console.WriteLine("Error on the answer, do it again!");
                    Console.WriteLine("//////////////////////////////////");
                }
            }

            Console.ReadKey();
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
        static SortedList<string, CustomerInfo> Deserialize()
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
            return listCustomer;
        }
    }
}
