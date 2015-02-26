using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Quantum.Quantum.Model
{
    [Serializable]
    class Drone
    {
        public Drone()
        {
            this.Health = 3; //default value  
            this.id = Guid.NewGuid().ToString();
        }

        public readonly string id;
        public Team Team;
        public int Health { get; set; }
        public Vector Position { get; set; }

        public Vector TargetPosition { get; set; }
        public int TargetOutpost { get; set; }
        public DroneOrder Order { get; set; }
    }
}
