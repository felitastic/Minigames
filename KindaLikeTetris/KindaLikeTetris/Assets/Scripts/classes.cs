
public class Ability
{
    public eAbility type { get; private set; }
    public bool unlocked { get; private set; }
    public bool active { get; private set; }
    public Ability(eAbility type, bool unlocked, bool active)
    {
        this.type = type;
        this.unlocked = unlocked;
        this.active = active;
    }

    public void Unlock()
    {
        unlocked = true;
    }

    public void ToggleActive(bool active)
    {
        this.active = active;

    }
}

public class TileInfo
{
    public TileInfo(Tile tile, ePosState state)
    {
        //this.x = x;
        //this.y = y;
        this.tile = tile;
        this.state = state;
    }

    public void Update(ePosState newState)
    {
        state = newState;
    }
        
    //public int x { get; private set; }
    //public int y { get; private set; }
    public Tile tile { get; private set; }

    public ePosState state { get; private set; }
}
