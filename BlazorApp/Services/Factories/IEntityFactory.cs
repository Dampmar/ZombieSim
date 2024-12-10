using System.Drawing;
using System.Drawing;
using BlazorApp.Entities;

namespace BlazorApp.Factories;
public interface IEntityFactory 
{
    ISimulationEntity CreateEntity();
}