using System;
using System.Linq;
using System.Transactions;
using Microsoft.EntityFrameworkCore;

namespace DBfin
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //1
            using (var context = new AppContext())
            {
                var user = context.Users.FirstOrDefault(u => u.UserName == "Nick");

                if (user != null)
                {
                    var newTransaction = new Transaction
                    {
                        Amount = 75,
                        UserId = user.UserId,
                        CategoryId = 1
                    };

                    context.Transactions.Add(newTransaction);
                    context.SaveChanges();
                }

                // 2
                var allTransactions = context.Transactions.ToList();
                foreach (var transaction in allTransactions)
                {
                    Console.WriteLine($"Transaction ID: {transaction.TransactionId}, Amount: {transaction.Amount}, Type: {transaction.Category.CategoryName}");
                }

                // 3
                var totalIncome = context.Transactions.Where(t => t.Amount > 0).Sum(t => t.Amount);
                var totalExpense = context.Transactions.Where(t => t.Amount < 0).Sum(t => t.Amount * -1);

                Console.WriteLine($"Total Income: {totalIncome}");
                Console.WriteLine($"Total Expense: {totalExpense}");

                // 4
                var filteredTransactions = context.Transactions
                    .Where(t => t.Amount < 0 && t.TransactionDate >= DateTime.Now.AddDays(-30))
                    .ToList();

                foreach (var transaction in filteredTransactions)
                {
                    Console.WriteLine($"Transaction ID: {transaction.TransactionId}, Amount: {transaction.Amount}, Type: {transaction.Category.CategoryName}");
                }

                // 5
                var userFinancialReport = context.Users
                    .Where(u => u.UserName == "Nick")
                    .Select(u => new
                    {
                        TotalIncome = u.Transactions.Where(t => t.Amount > 0).Sum(t => t.Amount),
                        TotalExpense = u.Transactions.Where(t => t.Amount < 0).Sum(t => t.Amount * -1),
                        Balance = u.Transactions.Sum(t => t.Amount)
                    })
                    .FirstOrDefault();

                Console.WriteLine($"Total Income: {userFinancialReport.TotalIncome}");
                Console.WriteLine($"Total Expense: {userFinancialReport.TotalExpense}");
                Console.WriteLine($"Balance: {userFinancialReport.Balance}");
            }
        }
    }
}