class Guard
{
    public int Health { get; private set; }
    public int Damage { get; private set; }
    public string Name { get; private set; }

    public Guard(string name, int health, int damage)
    {
        Name = name;
        Health = health;
        Damage = damage;
    }

    // Attack the player
    public void Attack(Player player)
    {
        player.Damage(Damage);
    }

    // Take damage
    public void TakeDamage(int amount)
    {
        Health -= amount;
    }

    // Check if the guard is alive
    public bool IsAlive()
    {
        return Health > 0;
    }
}