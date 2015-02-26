using Quantum.Quantum.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quantum.Quantum.Controllers
{
    class CheckWinCondition: GameController
    {
        public void execute(GameEvent gameEvent)
        {
            QuantumModel model = gameEvent.model;
            HashSet<Team> outpostHolders = new HashSet<Team>();

            foreach (Outpost outpost in model.Outposts)
            {
                outpostHolders.Add(outpost.Team);
            }

            foreach (General general in model.Generals)
            {
                if (general.Drones.Count > 0)
                {
                    outpostHolders.Add(general.Team);
                }
            }

            if (outpostHolders.Count == 1 && outpostHolders.First() != Team.neutral)
            {
                model.Winner = outpostHolders.First();
            }
        }
    }
}
