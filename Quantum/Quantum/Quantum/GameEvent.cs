using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;

namespace Quantum.Quantum
{
    class GameEvent
    {
        public readonly QuantumModel model;
        public readonly double deltaTime;
        public readonly QuantumGame game;
        public readonly Graphics graphics;
        public readonly Vector mousePosition;
        public readonly int screenWidth, screenHeight;
        public readonly Team winner = Team.neutral;

        public GameEvent(QuantumGame game, double deltaTime, Graphics graphics, int screenWidth, int screenHeight)
        {
            this.game = game;
            this.deltaTime = deltaTime;
            this.graphics = graphics;
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;


            this.model = game.model;
            this.mousePosition = game.mousePosition;
        }

        public Boolean isButtonPressed(Keys key)
        {
            if (winner != Team.neutral) return false;
            return game.isButtonPressed(key);
        }

        public Boolean isButtonPressed(MouseButtons key)
        {
            if (winner != Team.neutral) return false;
            return game.isButtonPressed(key);
        }
    }
}
