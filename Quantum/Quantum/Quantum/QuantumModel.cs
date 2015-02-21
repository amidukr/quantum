using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using Point = System.Drawing.Point;

namespace Quantum.Quantum
{
    class QuantumModel
    {
        readonly const int radiusMin = 10;
        readonly const int radiusMax = 13;

        public General currentGeneral = new General(Team.red);
    }

    class General
    {
        private Team team;
        public General(Team team)
        {
            this.team = team;
            Health = 3; //default value            
        }

        public Team CurrentTeam { get; }
        
        public Point CurrentPosition {get; set;}
        public Vector movingVector;

        public int Health { get; set; }
        public List<string> Drones { get; set; }
    }

    public enum Team { red, blue}
}
