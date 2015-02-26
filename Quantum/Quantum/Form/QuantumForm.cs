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
using System.Windows;
using Quantum.Quantum.Utils;
using Quantum.Quantum.Factory;

namespace Quantum
{
    public partial class QuantumForm : System.Windows.Forms.Form
    {
        private readonly SinglePlayerGameFactory singlePlayerFactory = new SinglePlayerGameFactory();
        private readonly ServerGameFactory serverGameFactory = new ServerGameFactory();
        private readonly ClientGameFactory clientGameFactory = new ClientGameFactory();

        private GameFactory gameFactory = new SinglePlayerGameFactory();

        private int ScreenWidth  = Screen.PrimaryScreen.WorkingArea.Width;
        private int ScreenHeight = Screen.PrimaryScreen.WorkingArea.Height;
        

        private QuantumGame game;
        private BufferedGraphicsContext context;
        private BufferedGraphics grafx;

        public QuantumForm()
        {
            InitializeComponent();

            context = BufferedGraphicsManager.Current;
            grafx = context.Allocate(this.CreateGraphics(),
                new Rectangle(0, 0, this.Width, this.Height));

            ScreenWidth  = this.Width;
            ScreenHeight = this.Height;

            restart();
        }

        private void restart() {
            gameFactory.create(ScreenWidth, ScreenHeight,  game =>
            {
                this.game = game;
                this.ActiveControl = null;
            });
            
        }

        private void onRestart(object sender, EventArgs e)
        {
            restart();
        }

        private void onTimer(object sender, EventArgs e)
        {
            this.Refresh();
        }


        private void onKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 'h')
            {
                serverGameFactory.Listen(Width, Height, newGame =>
                {
                    this.gameFactory = serverGameFactory;
                    this.game = newGame;
                });
                MessageBox.Show("Waiting for second player.\nPlease. press ok to proceeded");

            }

            if (e.KeyChar == 'j')
            {
                JoinWindow joinWindow = new JoinWindow();

                joinWindow.ShowDialog();

                String address = joinWindow.HostAddress;

                clientGameFactory.Join(Width, Height, address, newGame =>
                {
                    this.gameFactory = clientGameFactory;
                    this.game = newGame;
                });
            }

            if (e.KeyChar == 'r')
            {
                DialogResult result = MessageBox.Show("Are you sure want to restart game?", "Game restart", MessageBoxButtons.OKCancel);
                if (result == DialogResult.OK)
                {
                    restart();
                }

            }
        }

        private void QuantumForm_Paint(object sender, PaintEventArgs e)
        {
            if (e.Graphics == null) return;

            restartButton.Visible = game.playNext(e.Graphics, Width, Height);
            restartButton.Enabled = restartButton.Visible;
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            //Disable background paint
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

            Console.WriteLine("Mouse Down: " + e.X + "; " + e.Y);
        }

        private void onMouseUp(object sender, MouseEventArgs e)
        {
            game.changeInputState(e.Button, false);
        }

        private void onMouseMove(object sender, MouseEventArgs e)
        {
            game.mousePosition = new Vector(e.X, e.Y);
        }
    }
}
