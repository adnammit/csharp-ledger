// A basic checkbook ledger program in C#
using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using System.Globalization;
using Newtonsoft.Json;

namespace Ledger {

    public struct Transaction {
        public decimal Amount { get; set; }
        public string Author { get; set; } // store username of person who performed action
        public DateTime Timestamp { get; set; }
        public string Note { get; set; } // optional note

        public Transaction(decimal amount, string username, DateTime datetime, string note) {
            this.Amount = amount;
            this.Author = username;
            this.Timestamp = datetime;
            this.Note = note;
        }

        public void printInfo() {
            string str = "$"+String.Format("{0:0.00}",this.Amount)+" ";
            str += "\t"+this.Timestamp.ToString("MM/dd/yyyy")+" ";
            // str += this.Author+" "; // this is not particularly meaningful yet -- the user is the author. also don't expose the user name
            str += "\t"+this.Note;
            Console.WriteLine(str);
        }
    }

    public struct Result {
        public int value;
        public string message;

        public Result(int val, string msg) {
            value = val;
            message = msg;
        }
    }

    // a flattened version of data from different classes for file-writing
    public struct JSONobj {
        public string Username { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Type { get; private set; }
        public List<Transaction> Transactions { get; private set; }

        public JSONobj(string username, string firstname, string lastname, string type, List<Transaction> transactions) {
            this.Username = username;
            this.FirstName = firstname;
            this.LastName = lastname;
            this.Type = type;
            this.Transactions = transactions;
        }
    }

    // TO DO: Checking and Savings derive from Account
    public class Account {
        public List<Transaction> Transactions { get; private set; }

        public Account() {
            this.Transactions = new List<Transaction>();
        }

        public Account(List<Transaction> transactions) {
            this.Transactions = transactions;
        }

        public decimal getBalance() {
            decimal balance = 0;
            foreach (Transaction transaction in this.Transactions) {
                balance += transaction.Amount;
            }
            return balance;
        }

        public List<Transaction> getTransactions() {
            return this.Transactions;
        }

        public void makeTransaction(Transaction transaction) {
            this.Transactions.Add(transaction);
        }
        public void printTransactions() {
            Console.WriteLine();

            if(this.Transactions.Count > 0) {
                Console.WriteLine("Your transaction history is: ");
                foreach (Transaction transaction in this.Transactions) {
                    transaction.printInfo();
                }
            } else {
                Console.WriteLine("You don't have any transactions yet. Let's get to banking!");
            }
        }
    }

    // Base class Person:
    // Thus far only Client inherits, but this sets us up for Manager, Teller, etc
    public abstract class Person {
        public string Username { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }

        public Person() { }

        public Person(string username, string firstname, string lastname) {
            Username = username;
            FirstName = firstname;
            LastName = lastname;
        }

        public virtual void WelcomeUser() {
            Console.WriteLine("Welcome to Bank Machine 3000, {0} {1}", FirstName, LastName);
        }

        public abstract JSONobj getJsonObj();

        // for employees classes they'll be accessing other people's info
        public abstract void makeTransaction(decimal amount, string note);
        public abstract decimal getBalance();
        public abstract void printTransactions();
    }

    public class Client : Person {
        public Account Checking { get; private set; }
        // private Account Savings { get; private set; }

        public Client() { }

        public Client(string username, string firstname, string lastname) : base (username, firstname, lastname) {
            this.Checking = new Account();
        }

        public Client(string username, string firstname, string lastname, List<Transaction> transactions) : base (username, firstname, lastname) {
            this.Checking = new Account(transactions);
        }

        public override void WelcomeUser() {
            base.WelcomeUser();
            Console.WriteLine("Your balance is ${0}", this.Checking.getBalance());
        }

        public List<Transaction> getTransactions() {
            return this.Checking.getTransactions();
        }

        public override JSONobj getJsonObj() {
            return new JSONobj(this.Username, this.FirstName, this.LastName, "Client", this.getTransactions());
        }

        public override decimal getBalance() {
            return this.Checking.getBalance();
        }

        public override void makeTransaction(decimal amount, string note) {
            Transaction transaction = new Transaction(amount, this.Username, DateTime.Now, note);
            this.Checking.makeTransaction(transaction);
        }

        public override void printTransactions() {
            this.Checking.printTransactions();
        }
    }

    public class Session {

        private Person currUser;
        private string dbFilename;
        private List<Person> users = new List<Person>();

        public Session() { }

        public Session(string filename) {
            // load all existing user and transaction data
            this.dbFilename = filename;
            Result result = this.loadSession();
            int value = result.value;
            if(value == 0) {
                this.printError(result.message);
            }
        }

        private void printError(string msg) {
            Console.WriteLine();
            Console.WriteLine("Oh dear, we're afraid something has gone wrong:");
            Console.WriteLine("> {0}", msg);
            Console.WriteLine("Please contact our customer service department for assistance.");
        }

        private bool checkUserExists(string username) {
            return this.users.Exists( x => x.Username.Equals(username) );
        }

        private Person getUser(string username) {
            return this.users.Find( x => x.Username.Equals(username) );
        }

        private Person getCurrUser() {
            return this.currUser;
        }

        // For now this is mostly an abstract concept -- we just check if currUser has been assigned.
        // Ideally this would be a server call to check the password (since we don't want passwords
        //    on the client side)
        public bool isAuthenticated() {
            return (this.currUser != null);
        }

        // TO DO: Add cases for employees
        public void logIn(string username, string firstname, string lastname) {
            Person user;
            if(!this.checkUserExists(username)) {
                user = new Client(username, firstname, lastname);
                this.users.Add(user);
            } else {
                user = this.getUser(username);
            }
            this.logIn(user);
        }

        private void logIn(Person user) {
            this.currUser = user;
            user.WelcomeUser();
        }

        private void signIn() {
            string username;
            char u = '\0';
            bool found = false;

            do {
                Console.WriteLine("Please enter your username:   ['Q' to quit]");
                username = Console.ReadLine();

                if(!string.IsNullOrEmpty(username) ) {
                    u = char.ToUpper(username[0]);
                    found = this.checkUserExists(username);
                }
            } while (!found && u != 'Q');

            if(found) {
                Person user = this.getUser(username);
                this.logIn(user);
            }
        }

        private void createAccount() {
            string username;
            string firstname;
            string lastname;
            bool isValid = false;

            Console.WriteLine("To create an account, please enter your username:");
            do {
                username = Console.ReadLine();
                if(string.IsNullOrEmpty(username) ) {
                    Console.WriteLine("Please enter a username.");
                } else if (this.checkUserExists(username)) {
                    Console.WriteLine("That username already exists. Please choose a different username.");
                } else {
                    isValid = true;
                }
            } while (!isValid);

            do {
                Console.WriteLine("Please enter your first name:");
                firstname = Console.ReadLine();
            } while (string.IsNullOrEmpty(firstname));

            do {
                Console.WriteLine("Please enter your last name:");
                lastname = Console.ReadLine();
            } while (string.IsNullOrEmpty(lastname));

            this.logIn(username, firstname, lastname);
        }

        // A UI switch for creating a new account, signing into an existing one or exiting the program
        public void signinMenu() {
            Console.WriteLine("Welcome to Bank Machine 3000");

            string input;
            char a;
            do {
                a = '\0';
                Console.WriteLine();
                Console.WriteLine("Select one of the following options:   ['Q' to quit]");
                Console.WriteLine("E - Login to an existing account");
                Console.WriteLine("N - Create a new account");
                Console.WriteLine("Q - End your session");
                input = Console.ReadLine();
                if(input.Length>0)
                    a = char.ToUpper(input[0]);
                if(a != 'E' && a != 'N' && a != 'Q') {
                    a = '\0';
                    Console.WriteLine("Invalid input.");
                    Console.WriteLine("Please enter 'E', 'N' or 'Q'.");
                }
            } while (a == '\0');

            switch(a) {
                case 'E':
                    this.signIn();
                    break;
                case 'N':
                    this.createAccount();
                    break;
                default:
                    break;
            }
        }

        private void loadUserData(JSONobj jsonObj) {
            if(!this.checkUserExists(jsonObj.Username)) {
                // so far everyone is a client
                List<Transaction> transactions = jsonObj.Transactions;
                Client user = new Client(jsonObj.Username, jsonObj.FirstName, jsonObj.LastName, transactions);
                this.users.Add(user);
            }
        }

        private string getFilePath() {
            string path = Directory.GetCurrentDirectory();
            return Path.Combine(path, this.dbFilename);
        }

        private Result loadSession() {
            Result result = new Result();
            int success = this.readFile();
            if(success == 1) {
                result.value = 1;
                result.message = "";
            } else {
                result.value = 0;
                result.message = "Error encountered while loading system data.";
            }
            return result;
        }

        private Result saveSession() {
            Result result = new Result();
            int success = this.writeFile();
            if(success == 1) {
                result.value = 1;
                result.message = "";
            } else {
                result.value = 0;
                result.message = "Error encountered while saving session data.";
            }
            return result;
        }

        // TO DO: Currently just returns 1 -- add error handling
        private int readFile() {
            int success = 1;
            string filePath = this.getFilePath();

            if(File.Exists(filePath)) {
                string json = File.ReadAllText(@filePath);
                if(!string.IsNullOrEmpty(json)) {
                    var jsonList = JsonConvert.DeserializeObject<List<JSONobj>>(json);
                    if(jsonList.Count > 0) {
                        foreach (var jsonObj in jsonList) {
                            this.loadUserData(jsonObj);
                        }
                    }
                }
            }
            return success;
        }

        // TO DO: Currently just returns 1 -- add error handling
        private int writeFile() {
            int success = 1;
            string filePath = this.getFilePath();

            List<JSONobj> jsonList = new List<JSONobj>();
            foreach (var user in this.users) {
                jsonList.Add(user.getJsonObj());
            }

            using (StreamWriter file = File.CreateText(@filePath)) {
                string json = JsonConvert.SerializeObject(jsonList.ToArray(), Formatting.Indented);
                file.WriteLine(json);
            }
            return success;
        }

        private void makeDeposit() {
            string input;
            bool quit = false;
            decimal amount;
            Console.WriteLine("How much would you like to deposit?   ['Q' to quit]");
            do {
                amount = 0;
                input = Console.ReadLine();
                if(input.Length>0 && char.ToUpper(input[0]) == 'Q')
                    quit = true;
                else {
                    if(!decimal.TryParse(input, NumberStyles.Currency, CultureInfo.GetCultureInfo("en-US"), out amount) || amount <= 0) {
                        Console.WriteLine("That is not a valid amount. Please enter a valid monetary amount.");
                        amount = 0;
                    } else if(Decimal.Round(amount, 2) != amount) {
                        Console.WriteLine("Please enter an amount with no more than two decimal places.");
                        amount = 0;
                    }
                }
            } while (amount <= 0 && !quit);

            if(!quit) {
                Console.WriteLine("Enter an optional note for your transaction: ");
                string note = Console.ReadLine();
                this.currUser.makeTransaction(amount, note);
            }
        }

        private void makeWithdrawal() {
            string input;
            bool quit = false;
            decimal amount;
            decimal balance = this.currUser.getBalance();
            Console.WriteLine("How much would you like to withdraw?   ['Q' to quit]");
            do {
                amount = 0;
                input = Console.ReadLine();
                if(input.Length>0 && char.ToUpper(input[0]) == 'Q')
                    quit = true;
                else {
                    if(!decimal.TryParse(input, NumberStyles.Currency, CultureInfo.GetCultureInfo("en-US"), out amount) || amount <= 0) {
                        Console.WriteLine("That is not a valid amount. Please enter a valid monetary amount.");
                        amount = 0;
                    } else if(Decimal.Round(amount, 2) != amount) {
                        Console.WriteLine("Please enter an amount with no more than two decimal places.");
                        amount = 0;
                    } else if (amount > balance) {
                        Console.WriteLine("I'm afraid you have insufficient funds for that transaction. Please select a smaller amount.");
                        amount = 0;
                    }
                }
            } while (amount <= 0 && !quit);

            if(!quit) {
                Console.WriteLine("Enter an optional note for your transaction: ");
                string note = Console.ReadLine();
                amount = amount * -1;
                this.currUser.makeTransaction(amount, note);
            }
        }

        private void checkBalance() {
            Console.WriteLine("Your current balance is ${0}", this.currUser.getBalance());
        }

        private void printTransactions() {
            this.currUser.printTransactions();
        }

        // purpose is to get a valid input from the user and return an
        //   enum value that correlates to an action
        private char mainMenu() {
            string action;
            char a;
            do {
                Console.WriteLine();
                Console.WriteLine("Select one of the following options:");
                Console.WriteLine("D - Make a deposit");
                Console.WriteLine("W - Make a withdrawal");
                Console.WriteLine("B - Check your balance");
                Console.WriteLine("T - View transaction history");
                Console.WriteLine("Q - End your session");
                action = Console.ReadLine();
                a = '\0';
                if(action.Length>0)
                    a = char.ToUpper(action[0]);
                if(a != 'D' && a != 'W' && a != 'B' && a != 'T' && a != 'Q') {
                    action = "";
                    Console.WriteLine("Invalid input.");
                    Console.WriteLine("Please enter 'D', 'W', 'B', 'T' or 'Q'.");
                }
            } while (action.Length==0);
            return a;
        }

        public void run() {
            bool quit = false;
            char a;
            do {
                a = this.mainMenu();
                switch(a) {
                    case 'D':
                        this.makeDeposit();
                        break;
                    case 'W':
                        this.makeWithdrawal();
                        break;
                    case 'B':
                        this.checkBalance();
                        break;
                    case 'T':
                        this.printTransactions();
                        break;
                    case 'Q':
                        quit = true;
                        break;
                }
            } while (!quit);
        }

        public void goodbye() {
            Result result = this.saveSession();
            int value = result.value;
            if(value == 1) {
                Console.WriteLine();
                Console.WriteLine("Thank you for using Bank Machine 3000");
                Console.WriteLine("We hope you have a prosperous day!");
            }
            else
                this.printError(result.message);
        }
    }

    public class Application {
        static void Main(string[] args) {
            // our lovely 'database'
            string file = "bankstuff";
            Session session = new Session(file);
            // If we passed in the auto-login flag, login as our test user
            if(args.Length > 0 && (args[0].Equals("-l") || args[0].Equals("--auto-login")))
                session.logIn("kesselboss", "Han", "Solo");
            else
                session.signinMenu();
            if(session.isAuthenticated())
                session.run();
            session.goodbye();
        }
    }
}
