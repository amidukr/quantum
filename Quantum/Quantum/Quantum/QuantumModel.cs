using Quantum.Quantum.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using Point = System.Drawing.Point;

namespace Quantum.Quantum
{

    

    public enum Team { green, blue, neutral }
    public enum DroneOrder { MoveToGeneral, MoveToPosition, MoveToOutpost}

    [Serializable]
    class QuantumModel
    {
        private int nextUniqueId = 1;

        public readonly int maxRespawnAmount = 100;
        public readonly double milsPerDronRespawn = 60;

        public readonly int cloudRadius = 90;
        public readonly int moveToPositionRadius = 5;

        public readonly double milsOrderFactor = 100000000;
        public readonly double milsPerDronToRecruite = 0.2;

        public readonly double speedConstant = 1.0;
        public readonly double dronSpeedConstant = 1.8;
        public readonly double outpostConquestTime = 300;

        public readonly double mapWidth, mapHeight;
        public readonly List<General> Generals = new List<General>();
        public readonly List<Outpost> Outposts = new List<Outpost>();
        public readonly List<Beam> Beams = new List<Beam>();
        public Team Winner = Team.neutral;

        public QuantumModel(double mapWidth, double mapHeight)
        {
            this.mapWidth  = mapWidth;
            this.mapHeight = mapHeight;
        }

        public Outpost findOutpostById(int id)
        {
            foreach(Outpost outpost in this.Outposts) {
                if(outpost.id == id) return outpost;
            }
            
            return null;
        }

        public Outpost findOutpostByPosition(Vector position, double cloudRadius)
        {
            foreach (Outpost outpost in this.Outposts)
            {
                if (Vector.Subtract(position, outpost.Position).Length < cloudRadius)
                {
                    return outpost;
                }
            }

            return null;
        }

        public int generateID()
        {
            return nextUniqueId++;
        }

        internal General FindGeneralByTeam(Team team)
        {
            foreach (General general in Generals)
            {
                if (general.Team == team) return general;
            }

            return null;
        }
    }
}
