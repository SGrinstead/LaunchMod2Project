﻿// See https://aka.ms/new-console-template for more information
using MessageLogger.Data;
using MessageLogger.Models;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

//this is the part that runs the other methods
using (var context = new MessageLoggerContext())
{
    Console.WriteLine("Welcome to Message Logger!");
    RunProgram(context);
    ClosingInfo(context);
}

//this is where the loops and if statements that run the whole program live
static void RunProgram(MessageLoggerContext context)
{
    User currentUser = LogIn(context);
    string userInput = "";
    if (currentUser == null) userInput = "quit";
    if(userInput != "quit")
    {
        HelpMessage();
        userInput = Console.ReadLine();
    }
    
    while (Format(userInput) != "quit")
    {
        while (Format(userInput) != "logout" && Format(userInput) != "quit")
        {
            currentUser.Messages.Add(new Message(userInput));
            context.SaveChanges();

            currentUser.PrintMessages();

            Console.Write("Add a message: ");
            userInput = Console.ReadLine();
            Console.WriteLine();
        }
        if(Format(userInput) == "logout")
        {
            currentUser = LogIn(context);
        }
        if(Format(userInput) == "quit")
        {
            break;
        }
        else if (currentUser != null)
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

//USER BASED METHODS

//asks whether you want a new or existing user and runs the related method, this runs at the start
static User LogIn(MessageLoggerContext context)
{
    Console.Write("Would you like to log in a `new` or `existing` user? Or, `quit`? ");
    string userInput = "";
    while (Format(userInput) != "quit")
    {
        userInput = Console.ReadLine();
        if (Format(userInput) == "new")
        {
            return NewUser(context);
        }
        else if (Format(userInput) == "existing")
        {
            return GetUser(context);
        }
        else if (Format(userInput) != "quit")
        {
            Console.WriteLine("Input not recognized");
        }
    }
    return null;
}

//creates a new user and adds them to the database, then returns that user
static User NewUser(MessageLoggerContext context)
{
    string name = "";
    string username = "";
    Console.WriteLine();
    Console.WriteLine("Let's create a user pofile for you.");
    while (name == "")
    {
        Console.Write("What is your name? ");
        name = Console.ReadLine();
        if(Format(name) == "") Console.WriteLine("Please enter a valid name");
    }
    while(username == "")
    {
        Console.Write("What is your username? (one word, no spaces!) ");
        username = Console.ReadLine();
        if (Format(username) == "")
        {
            Console.WriteLine("Please enter a valid name");
            username = "";
        }
        else if (username.Contains(" "))
        {
            Console.WriteLine("Please enter a username with no spaces");
            username = "";
        }
        else if(context.Users.Any(user => user.Username == username))
        {
            Console.WriteLine("Sorry, that username is already taken");
            username = "";
        }
    }
    User currentUser = new User(name, username);
    context.Users.Add(currentUser);
    context.SaveChanges();
    return currentUser;
}

//finds an existing user and returns them
static User GetUser(MessageLoggerContext context)
{
    string username = "";
    while (true)
    {
        Console.Write("Please enter your username or 'quit':");
        username = Console.ReadLine();
        if(Format(username) == "quit")
        {
            return null;
        }
        else if(context.Users.Any(user => user.Username == username))
        {
            return context.Users.First(user => user.Username == username);
        }
        else
        {
            Console.WriteLine("Username not recognized");
        }
    }
}

//STATISTICS METHODS

//prompts the user to see neat statistics, runs related methods
static void ClosingInfo(MessageLoggerContext context)
{
    Console.WriteLine("Thanks for using Message Logger!");
    Console.WriteLine();
    Console.WriteLine("Please enter a number to see some neat statistics:");
    Console.WriteLine("1: The number of messages each user has written");
    Console.WriteLine("2: Users ordered by the number of messages they have written");
    Console.WriteLine("3: The hour with the most messages written");
    Console.WriteLine("4: The most common word users wrote");
    Console.WriteLine("5: quit");
    string userInput = "";
    while (userInput != "quit")
    {
        userInput = Console.ReadLine();
        switch (userInput)
        {
            case "1":
                MessageCountByUser(context);
                break;
            case "2":
                UsersOrderedByMessageCount(context);
                break;
            case "3":
                HourWithMostMessages(context);
                break;
            case "4":
                MostCommonWord(context);
                break;
            case "5":
                userInput = "quit";
                break;
            case "quit":
                userInput = "quit";
                break;
            default:
                Console.WriteLine("Input not recognized");
                break;
        }
    }
    Console.WriteLine("Goodbye");
}

//prints out each user and how many messages they have written
static void MessageCountByUser(MessageLoggerContext context)
{
    foreach (var u in context.Users)
    {
        Console.WriteLine($"{u.Name} has written {u.Messages.Count} messages.");
    }
}

//prints out each user and how many messages they have written, ordered by that number
static void UsersOrderedByMessageCount(MessageLoggerContext context)
{
    var orderedUsers = context.Users.OrderByDescending
        (user => user.Messages.Count());
    foreach (var user in orderedUsers)
    {
        Console.WriteLine($"{user.Name}: {user.Messages.Count()} messages");
    }
}

//prints the hour the most messages were written and the number of messages written in that hour
static void HourWithMostMessages(MessageLoggerContext context)
{
    var hours = context.Messages.GroupBy(message => message.CreatedAt.ToLocalTime().Hour);
    int hourWithMost = 0;
    int messageCount = 0;
    foreach(var hour in hours)
    {
        if (hour.Count() > messageCount)
        {
            hourWithMost = hour.Key;
            messageCount = hour.Count();
        }
    }
    Console.WriteLine($"Hour {hourWithMost} had the most messsages with a count of {messageCount}");
}

//prints the most commonly used word
static void MostCommonWord(MessageLoggerContext context)
{
    string allWords = AllWords(context);
    var split = allWords.Split(",");
    var wordCount = new Dictionary<string, int>();

    foreach (string word in split)
    {
        string lowerWord = word.ToLower();
        if (string.IsNullOrEmpty(lowerWord))
        {
            continue;
        }
        if (!wordCount.ContainsKey(lowerWord))
        {
            wordCount.Add(lowerWord, 0);
        }
        wordCount[lowerWord]++;
    }
    var maxPair = wordCount.First(word => word.Value == wordCount.Max(word => word.Value));

    Console.WriteLine($"The most common word is {maxPair.Key} with {maxPair.Value} uses");
}

//HELPER METHODS

//returns all words of all messages as a single string with commas separating the words
static string AllWords(MessageLoggerContext context)
{
    string allWords = "";
    var characterList = new List<string> { " ", "!", "' ", "?", ";", ":", ".", "/" };
    foreach(var message in context.Messages)
    {
        allWords += message.Content + " ";
    }
    foreach(string character in characterList)
    {
        if (allWords.Contains(character))
        {
            allWords = allWords.Replace(character, ",");
        }
    }
    return allWords;
}

//returns string in lowercase with no spaces
static string Format(string s)
{
    return s.ToLower().Replace(" ", "");
}

//info message after logging in
static void HelpMessage()
{
    Console.WriteLine();
    Console.WriteLine("To log out of your user profile, enter `log out`.");

    Console.WriteLine();
    Console.Write("Add a message (or `quit` to exit): ");
}