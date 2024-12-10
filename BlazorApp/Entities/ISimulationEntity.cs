using System;
using System.Drawing;

namespace BlazorApp.Entities;
public interface ISimulationEntity 
{
    Point Location {get; set;}                      // Get and Set Position in Map
    int Lifespan {get; set;}                        // Lifespan or Stamina of Human
    void Update();                                  // Update Entity Method
    void OnCollision(ISimulationEntity collided);   // Revise Collisions
}