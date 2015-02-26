using Quantum.Quantum.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Quantum.Quantum.Controllers
{
    class DrownRespawnerController: GameController
    {
        private readonly Random random = new Random();

        public int countDronesOnOutpost(Outpost outpost, General general, double cloudRadius)
        {
            int count = 0;
            foreach(Drone drone in general.Drones) 
            {
                if (Vector.Subtract(drone.Position, outpost.Position).Length < cloudRadius * 1.3)
                {
                    count++;
                }
            }

            return count;
        }

        public void execute(GameEvent gameEvent)
        {
            QuantumModel model = gameEvent.model;

            foreach(Outpost outpost in model.Outposts) {
                if (outpost.Team == Team.neutral) continue;

                int newDronesToRespawn = (int)((gameEvent.deltaTime + outpost.respawnTimerAccumulator) / model.milsPerDronRespawn);
                outpost.respawnTimerAccumulator += gameEvent.deltaTime - newDronesToRespawn * model.milsPerDronRespawn;

                if (newDronesToRespawn == 0) continue;

                General general = model.FindGeneralByTeam(outpost.Team);

                int currrentAmountDronesOnOutpost = countDronesOnOutpost(outpost, general, model.cloudRadius);

                if (currrentAmountDronesOnOutpost > model.maxRespawnAmount) continue;

                int amountToRespawn = Math.Min(model.maxRespawnAmount - currrentAmountDronesOnOutpost, newDronesToRespawn);

                for (int i = 0; i < amountToRespawn; i++)
                {
                    Drone drone = new Drone();

                    drone.Order = DroneOrder.MoveToOutpost;
                    drone.TargetOutpost = outpost.id;
                    drone.Position = new Vector(outpost.Position.X + random.NextDouble(), outpost.Position.Y + random.NextDouble());
                    //drone.Position = new Vector(outpost.Position.X, outpost.Position.Y);

                    general.AddDrone(drone);
                }
            }
        }
    }
}
