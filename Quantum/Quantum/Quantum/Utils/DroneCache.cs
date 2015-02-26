using Quantum.Quantum.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using IntPoint = System.Drawing.Point;

namespace Quantum.Quantum.Utils
{

    delegate void GetCacheRange(List<Drone> drones);

    class DronesCache {
        //ivate Dictionary<IntPoint, List<Drone>> cache = new Dictionary<IntPoint,List<Drone>>();
        private readonly int cacheXSize;
        private readonly int cacheYSize;

        private readonly int xOffset;
        private readonly int yOffset;

        private List<Drone>[,] cache;

        public readonly double frameCacheSize;

        public DronesCache(double frameCacheSize)
        {
            this.frameCacheSize = frameCacheSize;

            cacheXSize = (int)(5000/frameCacheSize);
            cacheYSize = (int)(5000/frameCacheSize);

            xOffset = cacheXSize/4;
            yOffset = cacheYSize/4;

            this.cache = new List<Drone>[cacheXSize, cacheYSize];
        }

        public void clear()
        {
            for (int i = 0; i <= cache.GetUpperBound(0); i++)
            {
                for (int j = 0; j <= cache.GetUpperBound(1); j++)
                {
                    cache[i, j] = null;
                }
            }
        }

        public void cacheDrones(List<Drone> drones)
        {
            foreach(Drone drone in drones) {
                IntPoint point = ToFramePoint(drone.Position);
                getDrones(point, true).Add(drone);

                //if (!cache.ContainsKey(point))
                //{
                //    cache[point] = new List<Drone>();
                //}

                //cache[point].Add(drone);
            }
        }

        public List<Drone> getDrones(IntPoint point, bool lazyInit = false) {
            if (point.X < 0) point.X = 0;
            if (point.Y < 0) point.Y = 0;

            if (point.X > cache.GetUpperBound(0)) point.X = cache.GetUpperBound(0);
            if (point.Y > cache.GetUpperBound(1)) point.Y = cache.GetUpperBound(1);

            if (lazyInit)
            {
                if (cache[point.X, point.Y] == null)
                {
                    cache[point.X, point.Y] = new List<Drone>();
                }
            }

            return cache[point.X, point.Y];
        }

        public void findDrones(Vector fromPoint, Vector toPoint, GetCacheRange callback)
        {
            IntPoint iFromPoint = ToFramePoint(fromPoint);
            IntPoint iToPoint = ToFramePoint(toPoint);

            IntPoint iPoint = new IntPoint();

            for (iPoint.X = iFromPoint.X; iPoint.X <= iToPoint.X; iPoint.X++)
            {
                for (iPoint.Y = iFromPoint.Y; iPoint.Y <= iToPoint.Y; iPoint.Y++)
                {
                    //if(!cache.ContainsKey(iPoint)) continue;
                    //foreach (Drone drone in cache[iPoint])
                    //{
                    //    yield return drone;
                    //}

                    List<Drone> drones = getDrones(iPoint);

                    if (drones == null) continue;

                    callback(drones);
                }
            }
        }


        public IntPoint ToFramePoint(Vector vector)
        {
            return new IntPoint((int)(vector.X / frameCacheSize) + xOffset,
                                (int)(vector.Y / frameCacheSize) + yOffset);
        }
    }

    class GeneralsDronesCache
    {
        private Dictionary<Team, DronesCache> cache = new Dictionary<Team, DronesCache>();
        private readonly double frameCacheSize;

        public GeneralsDronesCache(double frameCacheSize) {
            this.frameCacheSize = frameCacheSize;
        }

        public void cacheDrones(General general)
        {
            if (!cache.ContainsKey(general.Team))
            {
                cache[general.Team] = new DronesCache(frameCacheSize);
            }
            DronesCache teamCache = cache[general.Team];

            teamCache.clear();
            teamCache.cacheDrones(general.Drones);
        }

        public void cacheModel(QuantumModel model)
        {       
            foreach (General general in model.Generals)
            {
                cacheDrones(general);
            }
        }

        public void findDrones(List<Team> teams, Vector fromPoint, Vector toPoint, GetCacheRange callback) {
            List<Drone> results = new List<Drone>();

            foreach (Team team in teams)
            {
                //foreach (Drone done in cache[team].findDrones(fromPoint, toPoint))
                //{
                //    yield return done;
                //}

                cache[team].findDrones(fromPoint, toPoint, callback);
            }
        }

        public DronesCache getTeamCache(Team team)
        {
            return cache[team];
        }
    }
}
