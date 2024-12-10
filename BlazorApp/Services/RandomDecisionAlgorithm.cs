using BlazorApp.Services;
using System.Drawing;
using System;
using BlazorApp.Entities;

namespace BlazorApp.Services
{
    public class RandomDecisionAlgorithm : IDecisionAlgorithm
    {
        private static readonly Random rand = new Random();

        public Point ChooseNextTarget(ISimulationEntity entity, Point currentLocation, Point[] goals)
        {
            return new Point(
                currentLocation.X + rand.Next(-10, 11),
                currentLocation.Y + rand.Next(-10, 11)
            );
        }
    }
}
