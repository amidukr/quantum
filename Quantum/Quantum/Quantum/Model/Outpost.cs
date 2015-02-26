using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Quantum.Quantum.Model
{
    [Serializable]
    class Outpost
    {
        public Outpost()
        {
            this.Team = Team.neutral;
        }

        public int id { get; set; }
        public Vector Position { get; set; }
        public Team Team { get; set; }
        public double respawnTimerAccumulator { get; set; }
    }
}
