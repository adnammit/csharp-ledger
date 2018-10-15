// A basic checkbook ledger program in C#
using System;

namespace Ledger
{
    public class Application
    {
        static void Main()
        {
            string username;
            string first;
            string last;

            Console.WriteLine("Welcome to Bank Machine 3000");

            Console.WriteLine("To create an account, please enter your username:");
            username = Console.ReadLine();
            while(string.IsNullOrEmpty(username)) {
                Console.WriteLine("Please enter a username.");
                username = Console.ReadLine();
            }

            Console.WriteLine("Please enter your first name:");
            first = Console.ReadLine();
            while(string.IsNullOrEmpty(first)) {
                Console.WriteLine("Please enter your first name.");
                first = Console.ReadLine();
            }

            Console.WriteLine("Please enter your last name:");
            last = Console.ReadLine();
            while(string.IsNullOrEmpty(last)) {
                Console.WriteLine("Please enter a last name.");
                last = Console.ReadLine();
            }

            Person user = new Person(username, first, last);
            user.WelcomeUser();
        }
    }

    public struct Transaction
    {
        double amount;
        string date;
        string author; // store username of person who performed action
    }

    // Checking and Savings derive from Account
    public class Account
    {
        private int balance;
        // private Transactions[] array; // make a linked list? the overhead is not really necessary but we need some way of making it dynamic

        public Account()
        {
            balance = 0;
            // Transactions[]
        }

        public double getBalance()
        {
            return balance;
        }
    }


    // User, Manager and Teller derive from Person
    public class Person
    {
        private string username;
        private string firstName;
        private string lastName;
        private Account checking;
        // private Account savings;

        public Person(string uname, string first, string last)
        {
            username = uname;
            firstName = first;
            lastName = last;
            checking = new Account();
        }

        public void WelcomeUser()
        {
            Console.WriteLine("Welcome to Bank Machine 3000, {0} {1}", firstName, lastName);
            Console.WriteLine("Your balance is {0}", checking.getBalance());
        }
    }
}
