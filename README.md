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


### RUNNING THE PROGRAM
I wanted to keep it pretty simple for this program, so I skipped VS and stuck to the command line. This program uses fancy packages, so use `dotnet` to build and run. See "Using Dotnet" below for more info.
<!-- Compile with `$ csc Ledger.cs` which generates `Ledger.exe`. Then `$ ./Ledger.exe` to run the program. That's it! -->

For the purposes of this little program, we're just going to use a simple local file rather than an actual database. A test user with a bit of banking history is provided for a quick demonstration of how it works. Delete the `bankstuff` file if you'd like to start over with a clean slate.

To invoke the auto-login feature, use `$ dotnet run -l` -- otherwise just `$ dotnet run` will do.


### USING DOTNET
* Initially I just used the `csc` compiler to build my program but when I needed to pull in the `Newtonsoft.Json` package, I switched over to using `dotnet`
* Using `dotnet`:
    - once installed, you can use `dotnet add package` to install and make packages available.
    - `dotnet` implicitly calls the `nuget` package manager to install packages and maintain dependencies
    - basic `dotnet` commands:
    ```
        dotnet new console          // create a new project with a Program.cs file and main()
        dotnet build                // builds a project and all its dependencies
        dotnet run                  // first builds and then runs the application
        dotnet install package foo  // install package foo and make it available to your code
    ```


### TO-DO
* In a real-world implementation, each transaction would invoke a server request as opposed to getting all the information at once and then saving it all at the end. This would serve two purposes:
    - 1) moment-by-moment requests ensure that the information is up to date (the user _still_ has enough money in their account and didn't sneak a withdrawal in elsewhere)
    - 2) if an error were to occur on one transaction the program could be immediately aborted, rather than allowing the user to make further actions that are predicated on an action that will fail when it is applied.
* Currently all data (usernames, accounts and transaction info) are being stored together -- they should be accessed independently, so there's no chance of Charlie getting Marilyn's banking information
* Now that I've got the basics, take it up a level: migrate to using VisualStudio, break up the file by class, and build a UI
* Learn how to build and run C# tests
* Further develop class hierarchy
    - Right now `Client` is derived from `Person` -- we can add `Teller` and `Manager` as derived classes as well
    - `Checking` and `Savings` should derive from `Account` -- add distinct derived classes and give `Client`s one of each
* Make use of the Result struct for better error reporting
    - have printError() take the result as an arg and check for msg there?
* Perform more cleansing of input (remove/guard against special chars, store usernames as all lower case, etc)
* Currently there is no authentication -- make signing in more meaningful by password protecting it
* Make sure return values are meaningful
* Add try/catch blocks for file writing
* Create and enforce rules about usernames and other input

### STUFF TO ACTUALLY DO RIGHT NOW
* break into multiple files
* clean up `bankstuff`
* replace getters/setters with properties
* check access of all class methods/props
    - session is done
* Allow a filename to be specified as a command line arg
* delete auto-login when we are done developing
* correct use of public/private classes/methods/props?
* struct vs class for Transaction?
* should we store `balance` or calculate dynamically? probably store it when we load, don't store it in db
* walk through [msdocs coding conventions](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions) and make sure your code flies
* [Json.NET Samples](https://www.newtonsoft.com/json/help/html/Samples.htm)
