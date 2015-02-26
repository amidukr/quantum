using Quantum.Quantum.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Quantum.Quantum.Controllers
{
    class OutpostConquestController : GameController
    {
        double usedTime;

        public void execute(GameEvent gameEvent)
        {
            QuantumModel model = gameEvent.model;
            usedTime += gameEvent.deltaTime;

            if (usedTime < model.outpostConquestTime)
            {
                return;
            }

            usedTime -= model.outpostConquestTime;

            foreach(Outpost outpost in gameEvent.model.Outposts) {
                HashSet<General> generalsDroneParticipators = new HashSet<General>();

                foreach(General general in model.Generals) {
                    if(general.FindDroneCloseToOutpost(outpost, model.cloudRadius) != null) {
                        generalsDroneParticipators.Add(general);
                    }
                }

                if (generalsDroneParticipators.Count == 1)
                {
                    outpost.Team = generalsDroneParticipators.First().Team;
                    continue;
                }

                HashSet<General> generalsParticipators = new HashSet<General>();
                foreach (General general in model.Generals)
                {
                    if (Vector.Subtract(general.Position, outpost.Position).Length < model.cloudRadius)
                    {
                        generalsParticipators.Add(general);
                    }
                }

                if (generalsParticipators.Count == 1)
                {
                    outpost.Team = generalsParticipators.First().Team;
                }
            }
        }
    }
}
