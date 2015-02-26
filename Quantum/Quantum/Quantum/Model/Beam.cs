using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Quantum.Quantum.Model
{
    [Serializable]
    class Beam
    {
        public readonly int TimeToLive_Iterations = 10;
        public Vector position1;
        public Vector position2;
        public Team team;

        public Beam(Vector position1, Vector position2, Team team)
        {
            this.position1 = position1;
            this.position2 = position2;
            this.team = team;
        }
    }
}
