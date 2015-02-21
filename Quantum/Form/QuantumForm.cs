using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Quantum.Quantum;

namespace Quantum
{
    public partial class QuantumForm : Form
    {
        private QuantumGame game = new QuantumGame();

        public QuantumForm()
        {
            InitializeComponent();
        }

        private void onTimer(object sender, EventArgs e)
        {
            using(Graphics graphics = this.CreateGraphics()) {
                game.playNext(graphics);
            }
        }

        private void onKeyDown(object sender, KeyEventArgs e)
        {
            game.changeInputState(e.KeyCode, true);
        }

        private void onKeyUp(object sender, KeyEventArgs e)
        {
            game.changeInputState(e.KeyCode, false);
        }

        private void onMouseDown(object sender, MouseEventArgs e)
        {
            game.changeInputState(e.Button, true);
        }

        private void onMouseUp(object sender, MouseEventArgs e)
        {
            game.changeInputState(e.Button, false);
        }

        private void onMouseMove(object sender, MouseEventArgs e)
        {
            game.mousePosition = new Point(e.X, e.Y);
        }

        private void QuantumForm_Paint(object sender, PaintEventArgs e)
        {
            game.playNext(e.Graphics);
        }
    }
}
