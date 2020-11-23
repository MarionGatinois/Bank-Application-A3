    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace Bank_Application
    {

        [Serializable]
        public abstract class Account
        {
            public int balance { get; set; }
            public string name { get; set; }
            public List<TransactionInfo> Historic { get; set; }
        }

        [Serializable]
        public class SavingAccount : Account
        {

        }

        [Serializable]
        public class CurrentAccount : Account
        {

        }

        [Serializable]
        public enum enumAction
        {
            add, substract
        }

        [Serializable]
        public enum enumAccount
        {
            saving, current
        }

        [Serializable]
        public class TransactionInfo
        {
            public string date;
            public enumAction action;
            public int moneyTransferred;
            public int finalBalance;
            public enumAccount account;
            public string accountNumber;

        }
    }


