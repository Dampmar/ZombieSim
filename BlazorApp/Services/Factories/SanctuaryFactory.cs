using BlazorApp.Entities;
using System.Drawing;
namespace BlazorApp.Factories;
public class SanctuaryFactory : IEntityFactory
{
    private readonly Random random = new Random();

    public ISimulationEntity CreateEntity()
    {
        return new Sanctuary(
            new Point(random.Next(0, 1000 - 50), random.Next(0, 600 - 50)),
            random.Next(100, 200));
    }
}
