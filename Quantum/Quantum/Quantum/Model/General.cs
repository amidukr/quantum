using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Quantum.Quantum.Model
{
    [Serializable]
    class General
    {
        public readonly Team Team;

        public General(Team team)
        {
            this.Team = team;
            Health = 3; //default value   
            Position = new Vector(500, 500);
        }

        public Vector Position { get; set; }
        public Vector Velocity { get; set; }

        public int Health { get; set; }
        public readonly List<Drone> Drones = new List<Drone>();

        public Vector PrevSpeed { get; set; }

        public void AddDrone(Drone drone)
        {
            this.Drones.Add(drone);
            drone.Team = this.Team;
        }

        public Drone FindDroneCloseToOutpost(Outpost outpost, double radius)
        {
            Vector outpostPosition = outpost.Position;

            foreach (Drone drone in Drones)
            {
                if (1.3 * radius > Vector.Subtract(outpostPosition, drone.Position).Length)
                {
                    return drone;
                }
            }

            return null;
        }
    }
}
