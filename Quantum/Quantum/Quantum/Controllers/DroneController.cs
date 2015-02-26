using Quantum.Quantum.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows;

namespace Quantum.Quantum.Controllers
{   
    class DroneController: GameController
    {
        private Pen grenPen = new Pen(Color.Green, 3);
        private Pen grayPen = new Pen(Color.Gray, 3);
        private Random random = new Random();


        private Vector getCloudCenterPosition(QuantumModel model, General general, Drone drone)
        {
            if (drone.Order == DroneOrder.MoveToGeneral)
            {
                return general.Position;
            }
            else if (drone.Order == DroneOrder.MoveToOutpost)
            {
                return model.findOutpostById(drone.TargetOutpost).Position;
            }
            else if (drone.Order == DroneOrder.MoveToPosition)
            {
                return drone.TargetPosition;
            }

            throw new Exception("Unable to found cloud center position");

        }

        private void moveDrone(GameEvent gameEvent, General general, Drone drone)
        {
            Vector targetPosition = getCloudCenterPosition(gameEvent.model, general, drone);

            QuantumModel model = gameEvent.model;
            double cloudRadius = model.cloudRadius;
            Vector distanceToCenter = Vector.Subtract(targetPosition, drone.Position);
            Vector directionToCenter = distanceToCenter;


            double positionChange = model.dronSpeedConstant * gameEvent.deltaTime;
            if (directionToCenter.Length == 0)
            {
                drone.Position += new Vector(random.NextDouble(), random.NextDouble());
                return;
            }
            directionToCenter.Normalize();

            Vector droneMovement = Vector.Multiply(positionChange, directionToCenter);

            double precision = 0.9;
            double randomValue = random.NextDouble()*precision*2 + 1 - precision;

            double minDistanceForAction;

            if (   drone.Order == DroneOrder.MoveToGeneral
                || drone.Order == DroneOrder.MoveToOutpost)
            {
                minDistanceForAction = model.cloudRadius;
            }
            else if (drone.Order == DroneOrder.MoveToPosition)
            {
                minDistanceForAction = model.moveToPositionRadius;
            }
            else
            {
                minDistanceForAction = 1;
            }

            if (distanceToCenter.Length > minDistanceForAction + positionChange)
            {
                drone.Position = Vector.Add(drone.Position, Vector.Multiply(droneMovement, randomValue));
                
            }
            else if(  drone.Order == DroneOrder.MoveToGeneral 
                    || drone.Order ==  DroneOrder.MoveToOutpost)
            {
                if (general.Team == Team.green)
                {
                    drone.Position = Vector.Add(drone.Position, new Vector(randomValue * droneMovement.Y, -randomValue * droneMovement.X));
                }
                else
                {
                    drone.Position = Vector.Add(drone.Position, new Vector(randomValue * -droneMovement.Y, randomValue * droneMovement.X));
                }
                
            } 
            else if(drone.Order == DroneOrder.MoveToPosition) 
            {
                drone.Order = DroneOrder.MoveToGeneral;
            }
        }

        public void execute(GameEvent gameEvent)
        {
            QuantumModel model = gameEvent.model;

            foreach (General general in model.Generals)
            {
                foreach (Drone drone in general.Drones)
                {
                    moveDrone(gameEvent, general, drone);
                }
            }
        }
    }
}
