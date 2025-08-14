using System;
using System.Collections.Generic;

namespace DCIT318_Q1
{
    // record Transaction
    public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

    // ITransactionProcessor interface
    public interface ITransactionProcessor
    {
        void Process(Transaction transaction);
    }

    // Processors
    public class BankTransferProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[BankTransfer] Transaction #{transaction.Id}: Category = {transaction.Category}, Amount = {transaction.Amount}");
        }
    }

    public class MobileMoneyProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[MobileMoney] Transaction #{transaction.Id}: Category = {transaction.Category}, Amount = {transaction.Amount}");
        }
    }

    public class CryptoWalletProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[CryptoWallet] Transaction #{transaction.Id}: Category = {transaction.Category}, Amount = {transaction.Amount}");
        }
    }

    // Base Account class
    public class Account
    {
        public string AccountNumber { get; }
        public decimal Balance { get; protected set; }

        public Account(string accountNumber, decimal initialBalance)
        {
            AccountNumber = accountNumber;
            Balance = initialBalance;
        }

        public virtual void ApplyTransaction(Transaction transaction)
        {
            Balance -= transaction.Amount;
            Console.WriteLine($"[Account] Applied transaction #{transaction.Id}. New balance: {Balance}");
        }
    }

    // Sealed SavingsAccount
    public sealed class SavingsAccount : Account
    {
        public SavingsAccount(string accountNumber, decimal initialBalance)
            : base(accountNumber, initialBalance)
        { }

        public override void ApplyTransaction(Transaction transaction)
        {
            if (transaction.Amount > Balance)
            {
                Console.WriteLine("Insufficient funds");
                return;
            }

            Balance -= transaction.Amount;
            Console.WriteLine($"Updated balance: {Balance}");
        }
    }

    // FinanceApp
    public class FinanceApp
    {
        private List<Transaction> _transactions = new();

        public void Run()
        {
            // i. Instantiate SavingsAccount with account number and initial balance (e.g., 1000)
            var account = new SavingsAccount("SA-0001", 1000m);

            // ii. Create three Transaction records with sample values
            var t1 = new Transaction(1, DateTime.Now, 120.50m, "Groceries");
            var t2 = new Transaction(2, DateTime.Now, 300.00m, "Utilities");
            var t3 = new Transaction(3, DateTime.Now, 700.00m, "Entertainment");

            // iii. Use processors
            ITransactionProcessor mobileMoney = new MobileMoneyProcessor();
            ITransactionProcessor bankTransfer = new BankTransferProcessor();
            ITransactionProcessor cryptoWallet = new CryptoWalletProcessor();

            mobileMoney.Process(t1);     // Transaction 1 -> MobileMoneyProcessor
            bankTransfer.Process(t2);    // Transaction 2 -> BankTransferProcessor
            cryptoWallet.Process(t3);    // Transaction 3 -> CryptoWalletProcessor

            // iv. Apply each transaction to the SavingsAccount
            account.ApplyTransaction(t1);
            account.ApplyTransaction(t2);
            account.ApplyTransaction(t3);

            // v. Add all transactions to _transactions
            _transactions.Add(t1);
            _transactions.Add(t2);
            _transactions.Add(t3);
        }
    }

    class Program
    {
        static void Main()
        {
            var app = new FinanceApp();
            app.Run();
        }
    }
}
