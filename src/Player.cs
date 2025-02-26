class Player
{
    // Auto property
    public Room CurrentRoom { get; set; }
    private int health;

    // Constructor
    public Player()
    {
        CurrentRoom = null;
        health = 100;
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
    public void getHealth()
    {
        return health;
    }

    // Check if player is alive
    public void isAlive()
    {
        return health > 0;
    }
}
