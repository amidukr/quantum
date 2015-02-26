using Quantum.Quantum.Model;
using Quantum.Quantum.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Quantum.Quantum.Controllers
{
    class DronsFighting : GameController
    {
        public void execute(GameEvent gameEvent)
        {
            QuantumModel model = gameEvent.model;
            GeneralsDronesCache dronesCached = gameEvent.game.largeCache;

            double maxFightDistance = 150;

            foreach (General general in model.Generals)
            {
                List<Team> enemyTeams = new List<Team>();

                int beamSizeLimit = 1000;

                foreach (General enemyGeneral in model.Generals)
                {
                    if (general.Team == enemyGeneral.Team) continue;

                    enemyTeams.Add(enemyGeneral.Team);
                }

                foreach (Drone drone in general.Drones)
                {
                    Vector fightDistanceCorner = new Vector(maxFightDistance, maxFightDistance);

                    dronesCached.findDrones(enemyTeams, Vector.Subtract(drone.Position, fightDistanceCorner),
                                                                                     Vector.Add(drone.Position,      fightDistanceCorner),
                        droneList => {
                            foreach (Drone enemyDrone in droneList)
                            {

                                if (Vector.Subtract(enemyDrone.Position, drone.Position).Length > maxFightDistance) continue;

                                enemyDrone.Health--;

                                if (beamSizeLimit-- > 0)
                                {
                                    model.Beams.Add(new Beam(drone.Position, enemyDrone.Position, general.Team));
                                }
                                
                                break;
                            }
                        
                        });

                    
                }
            }
           

            foreach (General general in gameEvent.model.Generals)
            {
                general.Drones.RemoveAll(p => p.Health <= 0);
            }
            
        }
    }
}
