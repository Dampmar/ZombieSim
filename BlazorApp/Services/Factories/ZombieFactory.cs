using BlazorApp.Entities;
using BlazorApp.Services;
using System.Drawing;

namespace BlazorApp.Factories;

public class ZombieFactory : IEntityFactory
{
    private readonly int speed;
    private readonly SimulationManager simulation;
    private readonly IDecisionAlgorithm algorithm;
    private readonly Random random = new Random();

    public ZombieFactory(int speed, SimulationManager simulation, IDecisionAlgorithm algorithm)
    {
        this.speed = speed;
        this.simulation = simulation;
        this.algorithm = algorithm;
    }

    public ISimulationEntity CreateEntity()
    {
        var zombie = new Zombie(
            new Point(random.Next(0, 1000 - 10), random.Next(0, 600 - 10)),
            random.Next(50, 100),
            simulation,
            algorithm);
        zombie.SetSpeed(speed);
        return zombie;
    }
}