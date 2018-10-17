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

        // auto-login
        public void logIn(string username, string first, string last) {
            Client user = new Client(username, first, last);
            user.WelcomeUser();
            this.currUser = user;
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
            user.WelcomeUser();
            this.currUser = user;
        }

        // purpose is to get a valid input from the user and return an enum value that correlates to an action
        public string menu() {
            string selection;
            do {
                Console.WriteLine();
                Console.WriteLine("Please select an option:");
                Console.WriteLine("D - Make a deposit");
                Console.WriteLine("W - Make a withdrawal");
                Console.WriteLine("B - Check your balance");
                Console.WriteLine("T - View transaction history");
                Console.WriteLine("Q - End your session");
                selection = Console.ReadLine();
                //parse, print if failure
            } while (selection.Length==0);
            return selection;
        }

        public void run() {
            string selection = this.menu();
            Console.WriteLine("we have selected {0}", selection);
            // int continue = 1;
            // while(continue) {
            //     operation = this.menu();
            //     if(operation.Equals("Q"))
            //
            //     // do action
            // }
        }
    }


    public class Application {
        static void Main(string[] args) {

            // our lovely 'database'
            string file = "bankstuff.txt";
            Session session = new Session(file);

            // If we passed in the auto-login flag, login as our test user
            // DELETE WHEN DONE
            if(args.Length > 0 && (args[0].Equals("-l") || args[0].Equals("--auto-login")))
                session.logIn("", "", "");
            else
                session.logIn();

            session.run();
        }
    }
}
