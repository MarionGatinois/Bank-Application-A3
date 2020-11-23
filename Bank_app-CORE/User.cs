using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank_Application
{
    [Serializable]
    abstract class User
    {
        public string PinCode { get; set; }
    }

    interface IAdult
    {
        void adult();
    }

    [Serializable]
    class CustomerInfo : User
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Number { get; set; }
        public string Mail { get; set; }
        public CurrentAccount CurrentAccount { get; set; }
        public SavingAccount SavingAccount { get; set; }
    }

    class BankEmployeeInfo : User, IAdult
    {
        public void adult()
        {
            Console.Write("I am an adult");
        }
    }

}
