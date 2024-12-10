using System;
using System.Drawing;
using BlazorApp.Services;

namespace BlazorApp.Entities;
public class Human : ISimulationEntity 
{
    public Point Location {get; set;}                   // Location of the Entity 
    public int Lifespan {get; set;}                     // Lifespan of the Entity 
    public int Speed {get; private set; }               // Speed of Movement of the Human
    private IDecisionAlgorithm DecisionAlgorithm;       // Algorithm in Charge of Deciding Movement
    private int AttackSpan;                             // Counter Indicating Human in Attack Mode
    private SimulationManager SimulationManager;        // Access to the Simulation Manager to Create and Delete Entities

    // Aliases for Creating and Deleting Entities in Simulation
    private void AddEntity(ISimulationEntity entity) => SimulationManager.AddEntity(entity);
    private void RemoveEntity(ISimulationEntity entity) => SimulationManager.RemoveEntity(entity);

    // Constructor
    public Human(Point initLocation, int lifespan, SimulationManager simulationManager, IDecisionAlgorithm decisionAlgorithm) 
    {
        this.Location = initLocation; 
        this.Lifespan = lifespan;
        this.SimulationManager = simulationManager;
        this.DecisionAlgorithm = decisionAlgorithm;
        this.AttackSpan = 0;
    }

    // Move Method: Receives Goal Locations to Move Towards 
    private void Move(Point[] goals) 
    {
        if (goals == null || goals.Length == 0) return;
        var newLocation = DecisionAlgorithm.ChooseNextTarget(this, this.Location, goals);
        this.Location = new Point(Math.Clamp(newLocation.X, 0, 1000 - 10),
                                  Math.Clamp(newLocation.Y, 0, 600 - 10));
    }

    // Update Method: based on the entities and the current state of the human it should call a move to a different set of goals 
    public void Update()
    {   
        // Calculate Goals
        var entities = SimulationManager.GetEntities();
        var zombiesLocations = entities.Where(e => e is Zombie).Select(e => e.Location).ToArray();
        var itemsLocations = entities.Where(e => e is Axe || e is BurgerKing).Select(e => e.Location).ToArray();
        var sanctuaryLocations = entities.Where(e => e is Sanctuary).Select(e => e.Location).ToArray();

        // If Attacking Reduce this Span
        if (AttackSpan > 0) AttackSpan--;
        Lifespan--;

        if (AttackSpan > 0 && zombiesLocations.Any()) 
        {   
            // Attack Zombies 
            Move(zombiesLocations);
        } 
        else if ((Lifespan <= 10 && itemsLocations.Any()) || (!sanctuaryLocations.Any() && itemsLocations.Any())) 
        {   
            // Look for Items to Refill Lifespan
            Move(itemsLocations);
        }
        else if (sanctuaryLocations.Any()) 
        {   
            // Look for Shelter
            Move(sanctuaryLocations);
        }
        else 
        {  
            // Move Randomly 
            Move(new[] { new Point(new Random().Next(0, 1000 - 10), new Random().Next(0, 600 - 10)) });
        }
    }

    // Collision Handler Method: Knows what to do, based on collided entity
    public void OnCollision(ISimulationEntity collided) 
    {
        if (collided is Zombie)
        {
            if (AttackSpan > 0) 
            {   
                // Attack Mode: Kill Zombie 
                RemoveEntity(collided);
            } 
            else 
            {
                // Not Attack Mode: Turn into a zombie
                var newZombie = new Zombie(Location, new Random().Next(100, 200), SimulationManager, DecisionAlgorithm);
                AddEntity(newZombie);
                RemoveEntity(this);
            }
        }
        else if (collided is BurgerKing)
        {   
            // Restore Lifespan of the Human
            RestoreStamina();
            RemoveEntity(collided);
        }
        else if (collided is Axe)
        {   
            // Restore Lifespan and initialize Attack Mode
            SetAttackMode();
            RemoveEntity(collided);
        }
        else 
        {
            // In Sanctuary: Jitter Around 
            Jitter();
        }
    }

    // Move Slightly if in Sanctuary, Indicating Panic
    private void Jitter()
    {
        int jitterRange = 2;
        int jitterX = Location.X + new Random().Next(-jitterRange, jitterRange + 1);
        int jitterY = Location.Y + new Random().Next(-jitterRange, jitterRange + 1);
        Location = new Point(jitterX, jitterY);
    }

    public void SetAttackMode()
    {
        Lifespan = 100;
        AttackSpan = new Random().Next(50, 100);
    }

    public void SetSpeed(int speed){
        this.Speed = speed;
    }

    public void RestoreStamina()
    {
        Lifespan = 100;
    }
}