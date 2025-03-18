class Player
{
    // Auto property
    public Room CurrentRoom { get; set; }
    private int health;
    private Inventory backpack;

    // Constructor
    public Player()
    {
        CurrentRoom = null;
        health = 100;

        backpack = new Inventory(25);
    }

    // Damage player
    public void Damage(int amount)
    {
        health -= amount;
    }

    // Heal player
    public void Heal(int amount)
    {
        health += amount;
    }

    // Get player health
    public int GetHealth()
    {
        return health;
    }

    // Check if player is alive
    public bool isAlive()
    {
        return health > 0;
    }


    public bool TakeFromChest(string itemName)
    {
        Item item = CurrentRoom.Chest.Get(itemName);

        if (item == null)
        {
            Console.WriteLine($"Item: {itemName} does not exist here");
            return false;
        }

        if (backpack.Put(itemName, item))
        {
            Console.WriteLine($"Added {itemName} to backpack!");
            return true;
        }
        else
        {
            Console.WriteLine($"{itemName} is too heavy!");
            CurrentRoom.Chest.Put(itemName, item);
            return false;
        }
    }

    public bool DropToChest(string itemName)
    {
        Item item = backpack.Get(itemName);

        if (CurrentRoom.Chest.Put(itemName, item))
        {
            Console.WriteLine($"Dropped {itemName}!", ConsoleColor.Gray, true);
            return true;
        }

        return false;
    }

    public void ShowBackpack()
    {
        Console.WriteLine("Your backpack contains: " + backpack.GetItems());
    }

    public void Use(string itemName, string ItemKey)
    {
        Item item = backpack.Get(itemName);
        if (item == null)
        {
            Console.WriteLine("You don't have that item.");
        }

        if (item.Description.ToLower().Contains("medkit"))
        {
            Heal(20);
            backpack.RemoveItem(item);
            Console.WriteLine($"You used {item.Description} and gained 20 health!");
        }

        Console.WriteLine($"You used {item.Description}, but nothing happened.");
    }

}
