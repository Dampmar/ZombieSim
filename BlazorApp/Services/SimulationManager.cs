using System.Collections.Generic;
using System.Threading.Tasks; // For asynchronous task handling.
using System.Threading; // For cancellation of asynchronous tasks.
using System.Drawing;
using BlazorApp.Entities;
using BlazorApp.Services;
using BlazorApp.Factories;

namespace BlazorApp.Services;
public class SimulationManager
{
    private List<ISimulationEntity> entities = new List<ISimulationEntity>();
    private CancellationTokenSource cancellationTokenSource;
    private Task simulationTask;
    private const int CollisionDistance = 5; 
    private const int SanctuaryCollisionDistance = 50;
    public event Action OnSimulationUpdated; 

    // Initial counts for each type of entity in the simulation.
    private int initialHumanCount = 100;
    private int initialZombieCount = 50;
    private int initialSanctuaryCount = 5;
    private int initialBurgerCount = 5;
    private int initialAxeCount = 10;
    private int humanSpeed = 5; // Default speed for humans
    private int zombieSpeed = 3; // Default speed for zombies


    public SimulationManager() 
    {
        // Initalize entities with default values. 
        InitializeEntities(initialHumanCount, humanSpeed, initialZombieCount, zombieSpeed, initialSanctuaryCount, initialBurgerCount, initialAxeCount, new RandomDecisionAlgorithm());
    }

    public void InitializeEntities(int initialHumanCount, int humanSpeed, int initialZombieCount, int zombieSpeed,
                               int initialSanctuaryCount, int initialBurgerCount, int initialAxeCount,
                               IDecisionAlgorithm randomAlgorithm)
    {
        var factories = new List<(IEntityFactory factory, int count)>
        {
            (new HumanFactory(humanSpeed, this, randomAlgorithm), initialHumanCount),
            (new ZombieFactory(zombieSpeed, this, randomAlgorithm), initialZombieCount),
            (new SanctuaryFactory(), initialSanctuaryCount),
            (new BurgerKingFactory(), initialBurgerCount),
            (new AxeFactory(), initialAxeCount)
        };

        foreach (var (factory, count) in factories)
        {
            for (int i = 0; i < count; i++)
            {
                this.AddEntity(factory.CreateEntity());
            }
        }
    }


    public async Task StartSimulation()
    {
        cancellationTokenSource = new CancellationTokenSource(); // Initialize the cancellation token source.

        // Loop continuously until cancellation is requested.
        while (!cancellationTokenSource.Token.IsCancellationRequested)
        {
            // Update each entity's state.
            foreach (var entity in entities)
            {
                entity.Update();
            }
            CheckCollisions();
            CheckLifespans();

            // Trigger the OnSimulationUpdated event to notify listeners of the update.
            OnSimulationUpdated?.Invoke();
                
            // Delay between updates to control simulation speed (~60 FPS).
            await Task.Delay(100); 
        }
    }

    public void StopSimulation()
    {
        cancellationTokenSource?.Cancel();
    }

    public void AddEntity(ISimulationEntity entity) {
        entities.Add(entity);
    }

    public void RemoveEntity(ISimulationEntity entity) {
        entities.Remove(entity);
    }

    public IReadOnlyList<ISimulationEntity> GetEntities() {
        return entities.AsReadOnly();
    }

    // Method in charge of checking collisions 
    private void CheckCollisions() {

        // Use a copy of the list of entities so that external deletion of entities doesn't affect the simulation
        var entitiesCopy = new List<ISimulationEntity>(entities);
        var entitiesToRemove = new HashSet<ISimulationEntity>();

        // Iterate over all pairs of entities 
        for (int i = 0; i < entitiesCopy.Count; i++)
        {
            // If entity A is already set up for deletion don't consider it 
            var entityA = entitiesCopy[i];
            if (entitiesToRemove.Contains(entityA)) {
                continue;
            }

            for (int j = i + 1; j < entitiesCopy.Count; j++)
            {
                var entityB = entitiesCopy[j];
                // If entity B is already set up for deletion don't consider it 
                if (entitiesToRemove.Contains(entityB))
                    continue;

                if ((entityA is Human || entityA is Zombie) || (entityB is Human || entityB is Zombie) && !(entityA.GetType() == entityB.GetType())) 
                {
                    if (IsColliding(entityA, entityB)) {
                        Console.WriteLine($"Collision detected between {entityA} and {entityB}.");
                        entityA.OnCollision(entityB);
                        entityB.OnCollision(entityA);

                        // Check if any of the collisions implied that either got deleted
                        if (!entities.Contains(entityA)) entitiesToRemove.Add(entityA);
                        if (!entities.Contains(entityB)) entitiesToRemove.Add(entityB);
                    }
                }
            }
        }
    }

    // Method in charge of checking if two entities have collided
    private bool IsColliding(ISimulationEntity a, ISimulationEntity b)
    {      
        // Normal euclidean distance calculation 
        double distance = Math.Sqrt(Math.Pow(a.Location.X - b.Location.X, 2) + Math.Pow(a.Location.Y - b.Location.Y, 2));

        // Collision distances are dependent on type of entities.
        double collisionDistance = (a is Sanctuary || b is Sanctuary) ? SanctuaryCollisionDistance : CollisionDistance;
        return distance <= collisionDistance;
    }

    // Method in charge of checking lifespans 
    private void CheckLifespans() {

        // List of entities to remove 
        var entitiesToRemove = new List<ISimulationEntity>();

        // Go through all entities in the list of entities and remove any who has a lifespan smaller or equal to 0, by adding to the list of to be removed
        foreach (var entity in entities) 
        {
            if (entity.Lifespan <= 0) 
            {
                entitiesToRemove.Add(entity);
            }
        }

        // Remove the entities in the removal list, make sure they get deleted properly
        foreach (var entity in entitiesToRemove) {
            entities.Remove(entity);
        }
    }

    public void SetInitialCounts(int humanCount, int zombieCount, int sanctuaryCount, int burgerCount, int axeCount, int humanSpeed, int zombieSpeed, string algorithmType)
    {
        initialHumanCount = humanCount;
        initialZombieCount = zombieCount;
        initialSanctuaryCount = sanctuaryCount;
        initialBurgerCount = burgerCount;
        initialAxeCount = axeCount;
        this.humanSpeed = humanSpeed;
        this.zombieSpeed = zombieSpeed;

        // Reiniciar la simulación con los nuevos valores
        ResetSimulation(algorithmType);
    }

    private void ResetSimulation(string algorithmType)
    {   
        // Clear existing entities
        entities.Clear();

        // Select algorithm based on input
        IDecisionAlgorithm decisionAlgorithm = 
            (algorithmType == "ProximityDecisionAlgorithm") ? 
            new ProximityDecisionAlgorithm() :  
            new RandomDecisionAlgorithm();

        // Re-initialize entitites using factories 
        InitializeEntities(initialHumanCount, humanSpeed, initialZombieCount, zombieSpeed, initialSanctuaryCount, initialBurgerCount, initialAxeCount, decisionAlgorithm);
    }
}