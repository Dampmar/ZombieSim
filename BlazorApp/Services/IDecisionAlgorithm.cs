using System.Drawing;
using BlazorApp.Entities;

namespace BlazorApp.Services;
public interface IDecisionAlgorithm
{
    Point ChooseNextTarget(ISimulationEntity entity, Point currentLocation, Point[] goals);
}