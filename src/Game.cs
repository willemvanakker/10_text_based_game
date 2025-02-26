using System;

class Game
{
    private Parser parser;
    private Player player;

    public Game()
    {
        parser = new Parser();
        player = new Player();
        CreateRooms();
    }

    // Initialise the Rooms (and the Items)
    private void CreateRooms()
    {
        // Create the rooms
        Room outside = new Room("outside the main entrance of the university");
        Room theatre = new Room("in a lecture theatre");
        Room pub = new Room("in the campus pub");
        Room lab = new Room("in a computing lab");
        Room office = new Room("in the computing admin office");

        // Initialise room exits
        outside.AddExit("east", theatre);
        outside.AddExit("south", lab);
        outside.AddExit("west", pub);

        theatre.AddExit("west", outside);

        pub.AddExit("east", outside);

        lab.AddExit("north", outside);
        lab.AddExit("east", office);

        office.AddExit("west", lab);

        // Create your Items here
        Item medkit = new Item(3, "medkit");
        Item key = new Item(1, "key");
        Item book = new Item(1, "book");
        Item laptop = new Item(2, "laptop");
        Item phone = new Item(1, "phone");

        // Start game outside
        player.CurrentRoom = outside;
    }

    //  Main play routine. Loops until end of play.
    public void Play()
    {
        PrintWelcome();

        // Enter the main command loop. Here we repeatedly read commands and
        // execute them until the player wants to quit.
        bool finished = false;
        while (!finished)
        {
            Command command = parser.GetCommand();
            finished = ProcessCommand(command);
        }
        Console.WriteLine("Thank you for playing.");
        Console.WriteLine("Press [Enter] to continue.");
        Console.ReadLine();
    }

    // Print out the opening message for the player.
    private void PrintWelcome()
    {
        Console.WriteLine();
        Console.WriteLine("Welcome to Zuul!");
        Console.WriteLine("Zuul is a new, incredibly boring adventure game.");
        Console.WriteLine("Type 'help' if you need help.");
        Console.WriteLine();
        Console.WriteLine(player.CurrentRoom.GetLongDescription());
    }

    // Given a command, process (that is: execute) the command.
    // If this command ends the game, it returns true.
    // Otherwise false is returned.
    private bool ProcessCommand(Command command)
    {
        bool wantToQuit = false;

        if (command.IsUnknown())
        {
            Console.WriteLine("I don't know what you mean...");
            return wantToQuit; // false
        }

        switch (command.CommandWord)
        {
            case "help":
                PrintHelp();
                break;
            case "go":
                GoRoom(command);
                break;

            case "look":
                LookArround(command);
                break;

            case "up":
                goUp(command);
                break;

            case "down":
                goDown(command);
                break;

            case "health":
                getHealth(command);
                break;

            case "quit":
                wantToQuit = true;
                break;
        }

        return wantToQuit;
    }

    // ######################################
    // implementations of user commands:
    // ######################################

    // Print out some help information.
    // Here we print the mission and a list of the command words.
    private void PrintHelp()
    {
        Console.WriteLine("You are lost. You are alone.");
        Console.WriteLine("You wander around at the university.");
        Console.WriteLine();
        // let the parser print the commands
        parser.PrintValidCommands();
    }

    // Try to go to one direction. If there is an exit, enter the new
    // room, otherwise print an error message.
    private void GoRoom(Command command)
    {
        if (!command.HasSecondWord())
        {
            // if there is no second word, we don't know where to go...
            Console.WriteLine("Go where?");
            return;
        }

        string direction = command.SecondWord;

        // Try to go to the next room.
        Room nextRoom = player.CurrentRoom.GetExit(direction);
        if (nextRoom == null)
        {
            Console.WriteLine("There is no door to " + direction + "!");
            return;
        }

        player.Damage(5);
        player.CurrentRoom = nextRoom;
        Console.WriteLine("Player health: " + player.getHealth());
        Console.WriteLine(player.CurrentRoom.GetLongDescription());
    }

    // 'look' was entered. Print the long description of the current room.
    // This is the same as the long description of the room.
    private void LookArround(Command command)
    {
        Console.WriteLine(player.CurrentRoom.GetLongDescription());
    }

    // 'up' was entered. Print the long description of the current room.
    // This is the same as the long description of the room.
    private void goUp(Command command)
    {
        Console.WriteLine("You go up.");
    }

    // 'down' was entered. Print the long description of the current room.
    // This is the same as the long description of the room.
    private void goDown(Command command)
    {
        Console.WriteLine("You go down.");
    }

    // 'health' was entered. Print the player's health.
    private void getHealth(Command command)
    {
        Console.WriteLine("Player health: " + player.getHealth());
    }
}
