﻿// See https://aka.ms/new-console-template for more information
using MessageLogger.Data;
using MessageLogger.Models;
using System.Xml.Linq;

using (var context = new MessageLoggerContext())
{
    Console.WriteLine("Welcome to Message Logger!");
    RunProgram(context);
    ClosingInfo(context);
}

static string Format(string s)
{
    return s.ToLower().Replace(" ", "");
}

static void HelpMessage()
{
    Console.WriteLine();
    Console.WriteLine("To log out of your user profile, enter `log out`.");

    Console.WriteLine();
    Console.Write("Add a message (or `quit` to exit): ");
}

static void RunProgram(MessageLoggerContext context)
{
    User currentUser = LogIn(context);

    HelpMessage();
    string userInput = Console.ReadLine();
    if (currentUser == null) userInput = "quit";

    while (Format(userInput) != "quit")
    {
        while (Format(userInput) != "logout")
        {
            currentUser.Messages.Add(new Message(userInput));
            context.SaveChanges();

            currentUser.PrintMessages();

            Console.Write("Add a message: ");
            userInput = Console.ReadLine();
            Console.WriteLine();
        }
        currentUser = LogIn(context);
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

static User NewUser(MessageLoggerContext context)
{
    Console.WriteLine();
    Console.WriteLine("Let's create a user pofile for you.");
    Console.Write("What is your name? ");
    string name = Console.ReadLine();
    Console.Write("What is your username? (one word, no spaces!) ");
    string username = Console.ReadLine();
    User currentUser = new User(name, username);
    context.Users.Add(currentUser);
    context.SaveChanges();
    return currentUser;
}

static void ClosingInfo(MessageLoggerContext context)
{
    Console.WriteLine("Thanks for using Message Logger!");
    foreach (var u in context.Users)
    {
        Console.WriteLine($"{u.Name} has written {u.Messages.Count} messages.");
    }
}

static User GetUser(MessageLoggerContext context)
{
    Console.Write("What is your username? ");
    string username = Console.ReadLine();
    User currentUser = null;
    foreach (var existingUser in context.Users)
    {
        if (existingUser.Username == username)
        {
            currentUser = existingUser;
        }
    }
    return currentUser;
}

static User LogIn(MessageLoggerContext context)
{
    Console.Write("Would you like to log in a `new` or `existing` user? Or, `quit`? ");
    string userInput = Console.ReadLine();
    if (Format(userInput) == "new")
    {
        return NewUser(context);
    }
    else if (Format(userInput) == "existing")
    {
        return GetUser(context);
    }
    else return null;
}
