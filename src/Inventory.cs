class Inventory
{
    // fields
    private int maxWeight;
    private Dictionary<string, Item> items;
    private int currentWeight;

    // constructor
    public Inventory(int maxWeight)
    {
        this.maxWeight = maxWeight;
        this.items = new Dictionary<string, Item>();
        this.currentWeight = 0;
    }

    // methods
    public bool Put(string itemName, Item item)
    {
        // Check the weight of the item and check for enough space in the inventory
        if (currentWeight + item.Weight <= maxWeight)
        {
            // Put item in the items dictionary
            items[itemName] = item;
            currentWeight += item.Weight;
            return true; // success
        }
        return false; // failure
    }

    public Item Get(string itemName)
    {
        // Find item in items dictionary
        if (items.TryGetValue(itemName, out Item item))
        {
            // Remove item from items dictionary if found
            items.Remove(itemName);
            currentWeight -= item.Weight;
            return item; // return item
        }
        return null; // item not found
    }

    public string GetItems()
    {
        return string.Join(", ", items.Keys);
    }

    public int TotalWeight()
    {
        return currentWeight;
    }

    public int FreeWeight()
    {
        return maxWeight - currentWeight;
    }

    public bool RemoveItem(Item item)
    {
        var itemName = item.Description.ToLower();

        if (items.ContainsKey(itemName))
        {
            currentWeight -= item.Weight;
            items.Remove(itemName);
            return true;
        }
        return false;
    }

}
