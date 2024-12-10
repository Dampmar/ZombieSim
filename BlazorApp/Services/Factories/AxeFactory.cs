using BlazorApp.Entities;
using System.Drawing;
namespace BlazorApp.Factories;
public class AxeFactory : IEntityFactory
{
    private readonly Random random = new Random();

    public ISimulationEntity CreateEntity()
    {
        return new Axe(
            new Point(random.Next(0, 1000 - 10), random.Next(0, 600 - 10)),
            random.Next(50, 100));
    }
}
