using System.Drawing;

namespace BlazorApp.Entities;
public class BurgerKing : ISimulationEntity 
{
    public Point Location { get; set; } // Location of the Entity 
    public int Lifespan {get; set; }    // Lifespan of the Entity 

    // Constructor, involving dependency inversion 
    public BurgerKing(Point initLocation, int lifespan)
    {
        this.Location = initLocation;
        this.Lifespan = lifespan;
    }

    // Same method definitions as for Axe
    public void Update() { Lifespan--; }
    public void OnCollision(ISimulationEntity collided) { return; }
}