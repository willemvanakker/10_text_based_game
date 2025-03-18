using System;
class Game
{
    private Parser parser;
    private Player player;
    private Inventory inventory;

    public Game()
    {
        parser = new Parser();
        player = new Player();
        inventory = new Inventory(10);

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
        Room basement = new Room("in a dark basement", true); // locked room

        // Initialise room exits
        outside.AddExit("east", theatre);
        outside.AddExit("south", lab);
        outside.AddExit("west", pub);
        outside.AddExit("down", basement); // naar de kelder

        theatre.AddExit("west", outside);

        pub.AddExit("east", outside);

        lab.AddExit("north", outside);
        lab.AddExit("east", office);

        office.AddExit("west", lab);
        
        basement.AddExit("up", outside);

        // Create items
        Item medkit = new Item(3, "medkit", false, 50); // healt voor 50 punten
        Item key = new Item(1, "key");
        Item book = new Item(1, "book");
        Item laptop = new Item(2, "laptop");
        Item phone = new Item(1, "phone");
        Item poisonedApple = new Item(1, "apple", true, -30); // schaadt voor 30 punten
        Item rustyNail = new Item(1, "nail", true, -10); // schaadt voor 10 punten
        Item sword = new Item(5, "sword");

        // Add items to rooms
        outside.AddItem(book);
        theatre.AddItem(laptop);
        pub.AddItem(poisonedApple);
        lab.AddItem(medkit);
        lab.AddItem(rustyNail);
        office.AddItem(key);
        basement.AddItem(sword);

        // Add guards to rooms
        Guard pubGuard = new Guard("Barkeeper", 50, 10);
        pub.SetGuard(pubGuard);
        
        Guard basementGuard = new Guard("Security Guard", 100, 10);
        basement.SetGuard(basementGuard);

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
        while (!finished && player.isAlive())
        {
            Command command = parser.GetCommand();
            finished = ProcessCommand(command);
            
            // Check if player is still alive
            if (!player.isAlive())
            {
                Console.WriteLine("You have died! Game over.");
                finished = true;
            }
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
                GoUp(command);
                break;
            case "down":
                GoDown(command);
                break;
            case "health":
                GetHealth(command);
                break;
            case "inventory":
                GetInventory(command);
                break;
            case "take":
                TakeItem(command);
                break;
            case "drop":
                DropItem(command);
                break;
            case "use":
                UseItem(command);
                break;
            case "fight":
                Fight(command);
                break;
            case "unlock":
                UnlockRoom(command);
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
        
        // Check if the room is locked
        if (nextRoom.IsLocked)
        {
            Console.WriteLine("This room is locked! You need a key to enter.");
            return;
        }

        // Check if there's a guard in the current room
        Guard guard = player.CurrentRoom.GetGuard();
        if (guard != null && guard.IsAlive())
        {
            Console.WriteLine("The guard " + guard.Name + " blocks your way! You need to defeat them first.");
            guard.Attack(player);
            Console.WriteLine("The guard attacks you for " + guard.Damage + " damage!");
            Console.WriteLine("Your health: " + player.GetHealth());
            return;
        }

        player.Damage(5); // Beweging kost energie
        player.CurrentRoom = nextRoom;
        Console.WriteLine("Player health: " + player.GetHealth());
        Console.WriteLine(player.CurrentRoom.GetLongDescription());

        // Check if there's a guard in the new room
        guard = player.CurrentRoom.GetGuard();
        if (guard != null && guard.IsAlive())
        {
            Console.WriteLine("There's a guard here! The guard attacks you!");
            guard.Attack(player);
            Console.WriteLine("You take " + guard.Damage + " damage!");
            Console.WriteLine("Your health: " + player.GetHealth());
        }
    }

    // 'look' was entered. Print the long description of the current room.
    // This is the same as the long description of the room.
    private void LookArround(Command command)
    {
        Console.WriteLine(player.CurrentRoom.GetLongDescription());
    }

    // 'up' was entered. Move up if possible.
    private void GoUp(Command command)
    {
        Room nextRoom = player.CurrentRoom.GetExit("up");
        if (nextRoom == null)
        {
            Console.WriteLine("You can't go up from here!");
            return;
        }
        
        // Check if the room is locked
        if (nextRoom.IsLocked)
        {
            Console.WriteLine("This room is locked! You need a key to enter.");
            return;
        }
        
        player.CurrentRoom = nextRoom;
        Console.WriteLine("You go up.");
        Console.WriteLine(player.CurrentRoom.GetLongDescription());
    }

    // 'down' was entered. Move down if possible.
    private void GoDown(Command command)
    {
        Room nextRoom = player.CurrentRoom.GetExit("down");
        if (nextRoom == null)
        {
            Console.WriteLine("You can't go down from here!");
            return;
        }
        
        // Check if the room is locked
        if (nextRoom.IsLocked)
        {
            Console.WriteLine("This room is locked! You need a key to enter.");
            return;
        }
        
        player.CurrentRoom = nextRoom;
        Console.WriteLine("You go down.");
        Console.WriteLine(player.CurrentRoom.GetLongDescription());
    }

    // 'health' was entered. Print the player's health.
    private void GetHealth(Command command)
    {
        Console.WriteLine("Player health: " + player.GetHealth());
    }

    // 'inventory' was entered. Print the player's inventory.
    private void GetInventory(Command command)
    {
        if (inventory.GetItems().Length == 0)
        {
            Console.WriteLine("Your inventory is empty.");
        }
        else
        {
            Console.WriteLine("Your inventory: " + inventory.GetItems());
            Console.WriteLine("Total weight: " + inventory.TotalWeight() + " / " + inventory.FreeWeight() + " free");
        }
    }
    
    // 'take' was entered. Take an item from the room.
    private void TakeItem(Command command)
    {
        if (!command.HasSecondWord())
        {
            Console.WriteLine("Take what?");
            return;
        }

        string itemName = command.SecondWord;
        
        // Check if the item is in the room
        if (!player.CurrentRoom.HasItem(itemName))
        {
            Console.WriteLine("There is no " + itemName + " here!");
            return;
        }
        
        // Remove the item from the room
        Item item = player.CurrentRoom.RemoveItem(itemName);
        
        // Add the item to the inventory
        if (inventory.Put(itemName, item))
        {
            Console.WriteLine("You took the " + itemName + ".");
        }
        else
        {
            // If the inventory is full, put the item back in the room
            player.CurrentRoom.AddItem(item);
            Console.WriteLine("Your inventory is too full to carry the " + itemName + ".");
        }
    }
    
    // 'drop' was entered. Drop an item from the inventory.
    private void DropItem(Command command)
    {
        if (!command.HasSecondWord())
        {
            Console.WriteLine("Drop what?");
            return;
        }

        string itemName = command.SecondWord;
        
        // Remove the item from the inventory
        Item item = inventory.Get(itemName);
        
        if (item == null)
        {
            Console.WriteLine("You don't have a " + itemName + ".");
            return;
        }
        
        // Add the item to the room
        player.CurrentRoom.AddItem(item);
        Console.WriteLine("You dropped the " + itemName + ".");
    }
    
    // 'use' was entered. Use an item from the inventory.
    private void UseItem(Command command)
    {
        if (!command.HasSecondWord())
        {
            Console.WriteLine("Use what?");
            return;
        }

        string itemName = command.SecondWord;
        
        // Check if the item is in the inventory
        Item item = null;
        foreach (string key in inventory.GetItems().Split(", "))
        {
            if (key.ToLower() == itemName.ToLower())
            {
                item = inventory.Get(itemName);
                break;
            }
        }
        
        if (item == null)
        {
            Console.WriteLine("You don't have a " + itemName + ".");
            return;
        }
        
        // Use the item based on its type
        if (itemName.ToLower() == "medkit")
        {
            Console.WriteLine("You use the medkit to heal yourself.");
            player.Heal(item.HealthEffect);
            Console.WriteLine("Your health is now: " + player.GetHealth());
        }
        else if (itemName.ToLower() == "key")
        {
            Console.WriteLine("You can use the key to unlock a locked room. Use the 'unlock' command.");
            // Put the key back in the inventory
            inventory.Put(itemName, item);
        }
        else if (item.IsBadItem)
        {
            Console.WriteLine("You shouldn't have used that! It was harmful.");
            player.Damage(-item.HealthEffect); // Negative because the health effect is negative
            Console.WriteLine("Your health is now: " + player.GetHealth());
        }
        else
        {
            Console.WriteLine("You can't use the " + itemName + ".");
            // Put the item back in the inventory
            inventory.Put(itemName, item);
        }
    }
    
    // 'fight' was entered. Fight a guard in the room.
    private void Fight(Command command)
    {
        // Check if there's a guard in the room
        Guard guard = player.CurrentRoom.GetGuard();
        if (guard == null || !guard.IsAlive())
        {
            Console.WriteLine("There's no one to fight here.");
            return;
        }
        
        // Check if the player has a weapon
        bool hasSword = false;
        foreach (string item in inventory.GetItems().Split(", "))
        {
            if (item.ToLower() == "sword")
            {
                hasSword = true;
                break;
            }
        }
        
        int damageToGuard = hasSword ? 25 : 5;
        
        Console.WriteLine("You attack the guard!");
        guard.TakeDamage(damageToGuard);
        
        if (hasSword)
        {
            Console.WriteLine("You hit the guard with your sword for " + damageToGuard + " damage!");
        }
        else
        {
            Console.WriteLine("You hit the guard with your fist for " + damageToGuard + " damage!");
        }
        
        if (guard.IsAlive())
        {
            Console.WriteLine("The guard has " + guard.Health + " health left.");
            Console.WriteLine("The guard attacks you!");
            guard.Attack(player);
            Console.WriteLine("You take " + guard.Damage + " damage!");
            Console.WriteLine("Your health: " + player.GetHealth());
        }
        else
        {
            Console.WriteLine("You defeated the guard!");
            player.CurrentRoom.RemoveGuard();
        }
    }
    
    // 'unlock' was entered. Unlock a room.
    private void UnlockRoom(Command command)
    {
        if (!command.HasSecondWord())
        {
            Console.WriteLine("Unlock what?");
            return;
        }

        string direction = command.SecondWord;
        
        // Check if there's a room in that direction
        Room nextRoom = player.CurrentRoom.GetExit(direction);
        if (nextRoom == null)
        {
            Console.WriteLine("There is no door to " + direction + "!");
            return;
        }
        
        // Check if the room is locked
        if (!nextRoom.IsLocked)
        {
            Console.WriteLine("This room is not locked.");
            return;
        }
        
        // Check if the player has a key
        bool hasKey = false;
        foreach (string item in inventory.GetItems().Split(", "))
        {
            if (item.ToLower() == "key")
            {
                hasKey = true;
                break;
            }
        }
        
        if (!hasKey)
        {
            Console.WriteLine("You don't have a key to unlock this room.");
            return;
        }
        
        // Unlock the room
        nextRoom.IsLocked = false;
        Console.WriteLine("You unlocked the door to " + direction + ".");
    }
}
