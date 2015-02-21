using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Quantum.Quantum.Controllers;

namespace Quantum.Quantum
{

    interface GameController
    {
        void execute(GameEvent gameEvent);
    }

    class GameEvent
    {
        public readonly QuantumModel model;
        public readonly double deltaTime;
        public readonly QuantumGame game;
        public readonly Graphics graphics;
        public readonly Point mousePosition;

        public GameEvent(QuantumGame game, double deltaTime, Graphics graphics)
        {
            this.game = game;
            this.deltaTime = deltaTime;
            this.graphics = graphics;

            this.model = game.model;
            this.mousePosition = game.mousePosition;
        }

        public Boolean isButtonPressed(Keys key)
        {
            return game.isButtonPressed(key);
        }

        public Boolean isButtonPressed(MouseButtons key)
        {
            return game.isButtonPressed(key);
        }
    }


    class QuantumGame
    {


        private readonly Dictionary<object, bool> keyTable = new Dictionary<object, bool>();
        public Point mousePosition {get; set;}
        public readonly QuantumModel model = new QuantumModel();

        private long lastExecution;
        private bool firstExecution = true;


        private Brush backgroundBrush = new SolidBrush(Color.FromArgb(unchecked((int)0xff444465)));

        private SampleController sampleController = new SampleController();
        
        private void initialize() {
            
        }

        public void playNext(Graphics g)
        {
            long currentTime = System.DateTime.Today.Ticks;
            long deltaTime   = lastExecution - currentTime;
            lastExecution = currentTime;

            if (firstExecution)
            {
                initialize();
                firstExecution = false;
                return;
            }

            GameEvent gameEvent = new GameEvent(this, deltaTime, g);

            g.FillRectangle(backgroundBrush, g.ClipBounds);

            sampleController.execute(gameEvent);
        }



        public Boolean isButtonPressed(Object key)
        {
            return !keyTable.ContainsKey(key) ? false : keyTable[key];
        }

        public void changeInputState(Object key, Boolean pressed) {
            keyTable[key] = pressed;
        }
    }
}
