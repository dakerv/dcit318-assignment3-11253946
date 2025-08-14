using System;
using System.Collections.Generic;

namespace DCIT318_Assignment3_Q1
{
   
    public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

    // ---------- Interface for processors ----------
    public interface ITransactionProcessor
    {
        void Process(Transaction transaction);
    }

    // ---------- Concrete processors ----------
    public class BankTransferProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[BankTransfer] Processing transaction #{transaction.Id}: {transaction.Category} - Amount: {transaction.Amount:C}");
            // Real implementation would contact bank APIs, etc.
        }
    }

    public class MobileMoneyProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[MobileMoney] Processing transaction #{transaction.Id}: {transaction.Category} - Amount: {transaction.Amount:C}");
            // Real implementation would call mobile payment gateway, etc.
        }
    }

    public class CryptoWalletProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[CryptoWallet] Processing transaction #{transaction.Id}: {transaction.Category} - Amount: {transaction.Amount:C}");
            // Real implementation would interact with blockchain/wallet APIs, etc.
        }
    }

    // ---------- Account base class ----------
    public class Account
    {
        public string AccountNumber { get; }
        public decimal Balance { get; protected set; }

        public Account(string accountNumber, decimal initialBalance)
        {
            AccountNumber = accountNumber;
            Balance = initialBalance;
        }

        // Default behavior: deduct amount from balance
        public virtual void ApplyTransaction(Transaction transaction)
        {
            Balance -= transaction.Amount;
            Console.WriteLine($"[Account] Applied transaction #{transaction.Id}. New balance: {Balance:C}");
        }
    }

    // ---------- Sealed SavingsAccount ----------
    public sealed class SavingsAccount : Account
    {
        public SavingsAccount(string accountNumber, decimal initialBalance)
            : base(accountNumber, initialBalance)
        { }

        public override void ApplyTransaction(Transaction transaction)
        {
            if (transaction.Amount > Balance)
            {
                Console.WriteLine($"[SavingsAccount] Insufficient funds for transaction #{transaction.Id}. Transaction amount: {transaction.Amount:C}, Balance: {Balance:C}");
                return;
            }

            Balance -= transaction.Amount;
            Console.WriteLine($"[SavingsAccount] Transaction #{transaction.Id} of {transaction.Amount:C} applied. Updated balance: {Balance:C}");
        }
    }

    // ---------- FinanceApp (simulation) ----------
    public class FinanceApp
    {
        private readonly List<Transaction> _transactions = new();

        public void Run()
        {
            // i. Instantiate a SavingsAccount (example: account number "SA-1001", initial balance 1000)
            var mySavings = new SavingsAccount("SA-1001", 1000m);
            Console.WriteLine($"Created SavingsAccount {mySavings.AccountNumber} with initial balance {mySavings.Balance:C}\n");

            // ii. Create three Transaction records (Groceries, Utilities, Entertainment)
            var t1 = new Transaction(1, DateTime.Now, 120.50m, "Groceries");
            var t2 = new Transaction(2, DateTime.Now, 300.00m, "Utilities");
            var t3 = new Transaction(3, DateTime.Now, 700.00m, "Entertainment");

            // iii. Use processors to process each transaction
            ITransactionProcessor mobileMoney = new MobileMoneyProcessor();
            ITransactionProcessor bankTransfer = new BankTransferProcessor();
            ITransactionProcessor cryptoWallet = new CryptoWalletProcessor();

            Console.WriteLine("Processing transactions via processors:\n");

            mobileMoney.Process(t1);   // Transaction 1 -> MobileMoneyProcessor
            bankTransfer.Process(t2);  // Transaction 2 -> BankTransferProcessor
            cryptoWallet.Process(t3);  // Transaction 3 -> CryptoWalletProcessor

            Console.WriteLine();

            // iv. Apply each transaction to the SavingsAccount using ApplyTransaction
            Console.WriteLine("Applying transactions to savings account:\n");

            mySavings.ApplyTransaction(t1); // should succeed (120.50)
            mySavings.ApplyTransaction(t2); // should succeed if balance sufficient
            mySavings.ApplyTransaction(t3); // may fail due to insufficient funds

            Console.WriteLine();

            // v. Add all transactions to _transactions
            _transactions.Add(t1);
            _transactions.Add(t2);
            _transactions.Add(t3);

            // Print transaction summary
            Console.WriteLine("Transaction summary stored in FinanceApp:");
            foreach (var tx in _transactions)
            {
                Console.WriteLine($"- ID: {tx.Id}, Date: {tx.Date}, Category: {tx.Category}, Amount: {tx.Amount:C}");
            }

            Console.WriteLine($"\nFinal account balance for {mySavings.AccountNumber}: {mySavings.Balance:C}");
        }
    }

    // ---------- Program entry ----------
    class Program
    {
        static void Main(string[] args)
        {
            var app = new FinanceApp();
            app.Run();

            // Keep console open if run outside debugger
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
