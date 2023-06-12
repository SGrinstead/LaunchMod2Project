// See https://aka.ms/new-console-template for more information
using MessageLogger.Data;
using MessageLogger.Models;

//add a using context block around most of the program file
using (var context = new MessageLoggerContext())
{
    //foreach loops with users will become context.users
    Console.WriteLine("Welcome to Message Logger!");
    Console.WriteLine();
    Console.WriteLine("Let's create a user pofile for you.");
    Console.Write("What is your name? ");
    string name = Console.ReadLine();
    Console.Write("What is your username? (one word, no spaces!) ");
    string username = Console.ReadLine();
    //context.users.add then savechanges to add created user to the database
    User currentUser = new User(name, username);
    context.Users.Add(currentUser);
    context.SaveChanges();

    Console.WriteLine();
    Console.WriteLine("To log out of your user profile, enter `log out`.");

    Console.WriteLine();
    Console.Write("Add a message (or `quit` to exit): ");

    string userInput = Console.ReadLine();
    //this can be removed to be replaced by context.users
    //List<User> users = new List<User>() { user };

    while (userInput.ToLower() != "quit")
    {
        while (userInput.ToLower() != "log out")
        {
            currentUser.Messages.Add(new Message(userInput));
            //add a savechanges here
            context.SaveChanges();

            foreach (var message in currentUser.Messages)
            {
                Console.WriteLine($"{currentUser.Name} {message.CreatedAt:t}: {message.Content}");
            }

            Console.Write("Add a message: ");

            userInput = Console.ReadLine();
            Console.WriteLine();
        }

        Console.Write("Would you like to log in a `new` or `existing` user? Or, `quit`? ");
        userInput = Console.ReadLine();
        if (userInput.ToLower() == "new")
        {
            Console.Write("What is your name? ");
            name = Console.ReadLine();
            Console.Write("What is your username? (one word, no spaces!) ");
            username = Console.ReadLine();
            //add to context.users and savechanges
            currentUser = new User(name, username);
            context.Users.Add(currentUser);
            context.SaveChanges();
            Console.Write("Add a message: ");

            userInput = Console.ReadLine();

        }
        else if (userInput.ToLower() == "existing")
        {
            //in the future this will find the user in context.users
            Console.Write("What is your username? ");
            username = Console.ReadLine();
            currentUser = null;
            foreach (var existingUser in context.Users)
            {
                if (existingUser.Username == username)
                {
                    currentUser = existingUser;
                }
            }

            if (currentUser != null)
            {
                Console.Write("Add a message: ");
                userInput = Console.ReadLine();
            }
            else
            {
                Console.WriteLine("could not find user");
                userInput = "quit";

            }
        }

    }


    Console.WriteLine("Thanks for using Message Logger!");
    foreach (var u in context.Users)
    {
        Console.WriteLine($"{u.Name} wrote {u.Messages.Count} messages.");
    }
}
