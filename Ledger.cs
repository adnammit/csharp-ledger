// A basic checkbook ledger program in C#
using System;

namespace Ledger {

    public struct Transaction {
        double amount;
        string date;
        string author; // store username of person who performed action
        string note; // optional note
    }

    // Checking and Savings derive from Account
    public class Account {
        private int balance;
        // private Transactions[] array; // make a linked list? the overhead is not really necessary but we need some way of making it dynamic

        public Account() {
            balance = 0;
            // Transactions[]
        }

        public double getBalance() {
            return balance;
        }
    }


    // Base class Person:
    // Thus far, only Client inherits, but this sets us up for Manager, Teller, etc
    public class Person {
        private string username;
        private string firstName;
        private string lastName;

        public Person() { }

        public Person(string uname, string first, string last) {
            username = uname;
            firstName = first;
            lastName = last;
        }

        public virtual void WelcomeUser() {
            Console.WriteLine("Welcome to Bank Machine 3000, {0} {1}", firstName, lastName);
        }
    }

    public class Client : Person {
        private Account checking;
        // private Account savings;

        public Client() {}

        public Client(string uname, string first, string last) : base (uname, first, last) {
            checking = new Account();
        }

        public override void WelcomeUser() {
            base.WelcomeUser();
            Console.WriteLine("Your balance is ${0}", checking.getBalance());
        }
    }

    public class Session {

        private Person currUser;

        public Session() { }

        public Session(string filename) {
            // load all user and transaction data
        }

        public void logIn() {
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

            Client user = new Client(username, first, last);
            // Person user = new Person(username, first, last);
            user.WelcomeUser();
        }

        public void run() {
            Console.WriteLine("Press any key to end your session.");
            Console.ReadLine();
        }
    }


    public class Application {
        static void Main(string[] args) {

            // our lovely 'database'
            string file = "bankstuff.txt";

            Session session = new Session(file);
            session.logIn();
            session.run();
        }
    }
}
