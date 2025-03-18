class Item
{
    // fields
    public int Weight { get; }
    public string Description { get; }
    public bool IsBadItem { get; }
    public int HealthEffect { get; } // negatief voor badItems, positief voor medkits

    // constructor
    public Item(int weight, string description, bool isBadItem = false, int healthEffect = 0)
    {
        Weight = weight;
        Description = description;
        IsBadItem = isBadItem;
        HealthEffect = healthEffect;
    }
}