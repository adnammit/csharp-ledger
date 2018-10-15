# BANKING LEDGER
## MY FIRST C# PROJECT

### OVERVIEW
* Here is a simple command line banking ledger
* The ledger performs the following tasks:
    - Create a new account
    - Login
    - Record a deposit
    - Record a withdrawal
    - Check balance
    - See transaction history
    - Log out


### REQUIREMENTS
* make sure any packages are available via npm etc
* UI is optional
* use a temporary memory store (local cache) instead of creating an actual database

### RUNNING THE PROGRAM
Compile with `$ csc Ledger.cs` which generates `Ledger.exe`. Then `$ ./Ledger.exe` to run the program. That's it!


### TO-DO
* add a UI
* develop more user roles (teller, manager)
* correct use of public/private classes/methods/props?
* struct vs class for Transaction?
* should we store `balance`?
