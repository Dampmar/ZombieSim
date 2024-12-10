using BlazorApp.Entities;
using BlazorApp.Services;
using System.Drawing;

namespace BlazorApp.Services
{
    public class ProximityDecisionAlgorithm : IDecisionAlgorithm
    {
        private const int MaxStepSize = 10;     // Max Movement of Each Zombie 


        // Method in Charge of Selecting Where to Go
        public Point ChooseNextTarget(ISimulationEntity entity, Point currentLocation, Point[] goals)
        {
            if (goals == null || goals.Length == 0)
            {
                return currentLocation; // No goals, stay in place
            }

            // Find the nearest goal
            Point nearestGoal = goals[0];
            double shortestDistance = GetDistance(currentLocation, nearestGoal);

            for (int i = 1; i < goals.Length; i++)
            {
                double distance = GetDistance(currentLocation, goals[i]);

                if (distance >= 0 && distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestGoal = goals[i];
                }
            }

            // Calculate the step toward the nearest goal
            return MoveTowards(currentLocation, nearestGoal, MaxStepSize);
        }

        // Method in Charge of Moving Slightly Towards the Desired Goal
        private Point MoveTowards(Point current, Point target, int maxStep)
        {
            int dx = target.X - current.X;
            int dy = target.Y - current.Y;

            double distance = GetDistance(current, target);

            if (distance <= maxStep)
            {
                // If the distance is less than the max step, move directly to the target
                return target;
            }

            // Calculate the step proportionally
            double stepRatio = maxStep / distance;

            int stepX = current.X + (int)(dx * stepRatio);
            int stepY = current.Y + (int)(dy * stepRatio);

            return new Point(stepX, stepY);
        }

        // Euclidean Distance Calculator 
        private double GetDistance(Point a, Point b)
        {
            return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }
    }
}
