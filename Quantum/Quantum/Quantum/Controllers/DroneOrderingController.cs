using Quantum.Quantum.Model;
using Quantum.Quantum.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;

namespace Quantum.Quantum.Controllers
{
    class DroneOrderingController: GameController
    {
        private double orderAccumulatedTime = 0;
        private double recruiteAccumulatedTime = 0;

        private readonly Keys recruiteKey;
        private readonly MouseButtons orderButton;
        private readonly Team team;

        public DroneOrderingController(Team team, Keys recruiteKey, MouseButtons orderButton)
        {
            this.team = team;
            this.recruiteKey = recruiteKey;
            this.orderButton = orderButton;
        }

        public void execute(GameEvent gameEvent)
        {
            QuantumModel model = gameEvent.model;
            double cloudRadius = model.cloudRadius;
            General general = model.FindGeneralByTeam(team);
            Vector targetPosition = gameEvent.mousePosition;
            

            if (gameEvent.isButtonPressed(orderButton)) {
                giveOrderToDrones(gameEvent, model, general, targetPosition, cloudRadius);
            }

            if(gameEvent.isButtonPressed(recruiteKey)) 
            {
                recruiteDrones(gameEvent, model, general, cloudRadius);
            }
               
        }

        private Drone findDrone(Outpost outpost, General general, double cloudRadius) {

            int outpostId = outpost.id;

            foreach(Drone drone in general.Drones) {
                if (drone.Order != DroneOrder.MoveToOutpost) continue;
                if (drone.TargetOutpost != outpostId) continue;

                if (Vector.Subtract(outpost.Position, drone.Position).Length < cloudRadius * 1.3)
                {
                    return drone;
                }
            }

            return null;
        }

        public void recruiteDrones(GameEvent gameEvent, QuantumModel model, General general, double cloudRadius)
        {

            double milsForDrone = model.milsPerDronToRecruite;

            int amountOfDroneToRecruite = (int)((recruiteAccumulatedTime + gameEvent.deltaTime) / milsForDrone);
            recruiteAccumulatedTime += gameEvent.deltaTime - amountOfDroneToRecruite * milsForDrone;

            if (amountOfDroneToRecruite <= 0) return;

            Outpost outpost = model.findOutpostByPosition(general.Position, 1.5 * model.cloudRadius);

            if (outpost == null) return;

            while (amountOfDroneToRecruite > 0)
            {

                Drone drone = findDrone(outpost, general, cloudRadius);
                if (drone == null) return;

                drone.Order = DroneOrder.MoveToGeneral;

                amountOfDroneToRecruite--;
            }
        }

        public void giveOrderToDrones(GameEvent gameEvent, QuantumModel model, General general, Vector targetPosition, double cloudRadius)
        {
             Vector distanceToPoint = Vector.Subtract(targetPosition, general.Position);

            double milsForDrone = distanceToPoint.Length * distanceToPoint.Length * distanceToPoint.Length / model.milsOrderFactor;

            int amountOfDroneToSend = (int)((orderAccumulatedTime + gameEvent.deltaTime) / milsForDrone);
            orderAccumulatedTime += gameEvent.deltaTime - amountOfDroneToSend * milsForDrone;

            Outpost outpost = model.findOutpostByPosition(targetPosition, model.cloudRadius);

            foreach (Drone dron in general.Drones)
            {
                if (amountOfDroneToSend <= 0) break;

                if (dron.Order == DroneOrder.MoveToGeneral 
                   && Vector.Subtract(dron.Position, general.Position).Length < cloudRadius * 1.5)
                {
                    amountOfDroneToSend--;

                    if (outpost != null)
                    {
                        dron.TargetOutpost = outpost.id;
                        dron.Order = DroneOrder.MoveToOutpost;
                    }
                    else
                    {
                        dron.TargetPosition = targetPosition;
                        dron.Order = DroneOrder.MoveToPosition;
                    }
                    
                }
            }
        }
    }
}
