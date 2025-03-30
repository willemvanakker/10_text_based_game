class Room
{
    // Private fields
    private string description;
    private Dictionary<string, Room> exits; // stores exits of this room.
    private Inventory chest;
    private List<Item> items; // items in the room
    private bool isLocked;
    private Guard guard; // optional guard in the room

    public Inventory Chest
    {
        get
        {
            return chest;
        }
    }

    public bool IsLocked
    {
        get { return isLocked; }
        set { isLocked = value; }
    }

    // Create a room described "description". Initially, it has no exits.
    // "description" is something like "in a kitchen" or "in a court yard".
    public Room(string desc, bool locked = false)
    {
        description = desc;
        exits = new Dictionary<string, Room>();
        chest = new Inventory(999999);
        items = new List<Item>();
        isLocked = locked;
    }

    // Define an exit for this room.
    public void AddExit(string direction, Room neighbor)
    {
        exits.Add(direction, neighbor);
    }

    // Add an item to the room
    public void AddItem(Item item)
    {
        items.Add(item);
    }

    // Remove an item from the room
    public Item RemoveItem(string itemName)
    {
        foreach (Item item in items)
        {
            if (item.Description.ToLower() == itemName.ToLower())
            {
                items.Remove(item);
                return item;
            }
        }
        return null;
    }

    // Check if the room has an item
    public bool HasItem(string itemName)
    {
        foreach (Item item in items)
        {
            if (item.Description.ToLower() == itemName.ToLower())
            {
                return true;
            }
        }
        return false;
    }

    // Check if the room has any items
    public bool HasItems()
    {
        return items.Count > 0;
    }

    // Get an item from the room without removing it
    public Item GetItem(string itemName)
    {
        foreach (Item item in items)
        {
            if (item.Description.ToLower() == itemName.ToLower())
            {
                return item;
            }
        }
        return null;
    }

    // Return the description of the room.
    public string GetShortDescription()
    {
        return description;
    }

    // Return a long description of this room, in the form:
    //     You are in the kitchen.
    //     Exits: north, west
    //     A guard is here.
    public string GetLongDescription()
    {
        string str = "";

        str += "You are ";
        str += description;
        str += ".\n";
        str += GetExitString();
        
        if (guard != null && guard.IsAlive())
        {
            str += "\nA guard is here. Health: " + guard.Health;
        }
        
        return str;
    }

    // Return a string describing the items in the room
    public string GetItemString()
    {
        if (items.Count == 0)
        {
            return "There are no items here.";
        }
        
        string str = "Items: ";
        List<string> itemNames = new List<string>();
        
        foreach (Item item in items)
        {
            itemNames.Add(item.Description);
        }
        
        str += String.Join(", ", itemNames);
        return str;
    }

    // Return the room that is reached if we go from this room in direction
    // "direction". If there is no room in that direction, return null.
    public Room GetExit(string direction)
    {
        if (exits.ContainsKey(direction))
        {
            return exits[direction];
        }
        return null;
    }

    // Return a string describing the room's exits, for example
    // "Exits: north, west".
    private string GetExitString()
    {
        string str = "Exits: ";
        str += String.Join(", ", exits.Keys);

        return str;
    }

    // Set a guard in this room
    public void SetGuard(Guard guard)
    {
        this.guard = guard;
    }

    // Get the guard in this room
    public Guard GetGuard()
    {
        return guard;
    }

    // Remove the guard from this room
    public void RemoveGuard()
    {
        guard = null;
    }
}