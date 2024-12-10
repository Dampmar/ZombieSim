using System; 
using System.Drawing;
using System.Linq;
using BlazorApp.Services;

namespace BlazorApp.Entities;

public class Zombie : ISimulationEntity 
{   
    // Properties act similarly to the ones in the human
    public int Lifespan {get; set;}
    public int Speed { get; private set; }
    public Point Location {get; set;}
    private IDecisionAlgorithm DecisionAlgorithm;
    private SimulationManager SimulationManager {get; set;}

    private void AddEntity(ISimulationEntity entity) => SimulationManager.AddEntity(entity);
    private void RemoveEntity(ISimulationEntity entity) => SimulationManager.RemoveEntity(entity);

    // Constructor
    public Zombie(Point initialLocation, int lifespan, SimulationManager simulationManager, IDecisionAlgorithm decisionAlgorithm)
    {
        this.SimulationManager = simulationManager;
        this.DecisionAlgorithm = decisionAlgorithm;
        this.Location = initialLocation;
        this.Lifespan = lifespan;
    }

    // Move Method: Receives Goal Locations to Move Towards 
    public void Move(Point[] goals)
    {
        if (goals == null || goals.Length == 0) return;

        var newLocation = DecisionAlgorithm.ChooseNextTarget(this, this.Location, goals);
        this.Location = new Point(Math.Clamp(newLocation.X, 0, 1000 - 10),
                                  Math.Clamp(newLocation.Y, 0, 600 - 10));
    }

    // Update Method: move towards humans, and reduce the lifespan 
    public void Update()
    {
        var entities = SimulationManager.GetEntities();
        var humanLocations = entities.Where(e => e is Human).Select(e => e.Location).ToArray();
        
        Lifespan--;
        Move(humanLocations);
    }

    // Since collision between human and zombie is handled in the human, collision between zombie and sanctuary is handled in here.
    public void OnCollision(ISimulationEntity collided)
    {
        if (collided is Sanctuary)
        {
            RemoveEntity(this);
        }
    }


    public void SetSpeed(int speed)
    {
        this.Speed = speed;
    }
}