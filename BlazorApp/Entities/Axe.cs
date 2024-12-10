using System.Drawing;

namespace BlazorApp.Entities;
public class Axe : ISimulationEntity {
    public Point Location {get; set; }  // Location of the Entity 
    public int Lifespan {get; set; }    // Lifespan of the Entity

    // Constructor, involving dependency inversion
    public Axe(Point initLocation, int lifespan)
    {
        this.Location = initLocation;
        this.Lifespan = lifespan;
    }

    // Only Update, reduce the lifespan
    public void Update() { Lifespan--; }

    // Collisions for this object are primarily being handled by the Human Entity 
    public void OnCollision(ISimulationEntity collided) { return; }
}