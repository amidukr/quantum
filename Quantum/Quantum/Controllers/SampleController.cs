using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Quantum.Quantum.Controllers
{
    class SampleController: GameController
    {
        private Pen blackPen = new Pen(Color.Black, 3);

        public void execute(GameEvent gameEvent)
        {
            if (gameEvent.isButtonPressed(Keys.W))            Console.WriteLine("Keyboard button will be W pressed");
            if (gameEvent.isButtonPressed(MouseButtons.Left)) Console.WriteLine("Left Mouse Button Pressed");



            if (!gameEvent.isButtonPressed(MouseButtons.Left))
            {
                gameEvent.graphics.DrawLine(blackPen, new Point(0, 0), gameEvent.mousePosition);
            }
        }
    }
}
