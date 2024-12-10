using BlazorApp.Services;
using System; 
using System.Drawing;
using System.Linq;

namespace BlazorApp.Entities;

public class Sanctuary : ISimulationEntity
{
    public Point Location {get; set;}
    public int Lifespan {get; set;}

    public Sanctuary(Point initialLocation, int lifespan)
    {
        this.Location = initialLocation;
        this.Lifespan = lifespan;
    }

    public void Update() { Lifespan--; }
    public void OnCollision(ISimulationEntity collided) { return; }
}