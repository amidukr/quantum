using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows;
using Quantum.Quantum.Model;

namespace Quantum.Quantum.Controllers
{
    class GeneralController : GameController
    {
        private readonly Keys upButton;
        private readonly Keys downButton;
        private readonly Keys leftButton;
        private readonly Keys rightButton;
        private readonly Team team;

        public GeneralController(Keys upButton, Keys downButton, Keys leftButton, Keys rightButton, Team team)
        {
            this.upButton    = upButton;
            this.downButton  = downButton;
            this.leftButton  = leftButton;
            this.rightButton = rightButton;
            this.team = team;
        }

        public void execute(GameEvent gameEvent)
        {
            QuantumModel model = gameEvent.model;
            General general = model.FindGeneralByTeam(team);

            double angle = -10;
            
            if (gameEvent.isButtonPressed(upButton) && gameEvent.isButtonPressed(rightButton))
            {
                angle = -Math.PI/4;
            }
            else if (gameEvent.isButtonPressed(downButton) && gameEvent.isButtonPressed(rightButton))
            {
                angle = Math.PI/4;
            }
            else if (gameEvent.isButtonPressed(downButton) && gameEvent.isButtonPressed(leftButton))
            {   
                angle = 3*Math.PI/4;
            }
            else if (gameEvent.isButtonPressed(leftButton) && gameEvent.isButtonPressed(upButton))
            {
                angle = -(3*Math.PI/4);
            }
            else if (gameEvent.isButtonPressed(upButton))
            {
                angle = -Math.PI/2;
            }
            else if (gameEvent.isButtonPressed(rightButton))
            {
                angle = 0;
            }
            else if (gameEvent.isButtonPressed(downButton))
            {
                angle = Math.PI/2;
            }
            else if (gameEvent.isButtonPressed(leftButton))
            {
                angle = -Math.PI;
            }

            if (angle != -10)
            {
                
                Vector newVelocity = new Vector(Math.Cos(angle) * gameEvent.model.speedConstant, 
                                                Math.Sin(angle) * gameEvent.model.speedConstant);

                

                general.Velocity = Vector.Add(Vector.Multiply(0.75, general.Velocity), 
                                              Vector.Multiply(0.25, newVelocity));
                general.PrevSpeed = general.Velocity;
                

            }
            else
            {
                general.Velocity = new Vector(0, 0);
            }


            general.Position = new Vector(general.Position.X + (general.Velocity.X * gameEvent.deltaTime),
                                          general.Position.Y + (general.Velocity.Y * gameEvent.deltaTime));
        }
        private enum Direction { up, right_up, right, right_down, down, left_down, left, left_up, noChange}
    }
}