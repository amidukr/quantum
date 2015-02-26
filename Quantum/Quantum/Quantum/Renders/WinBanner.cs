using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Quantum.Quantum.Renders
{
    class WinBanner
    {


        private Image greenGameOver = Image.FromFile(@"Resources\green-game-over.png");
        private Image blueGameOver = Image.FromFile(@"Resources\blue-game-over.png");

        public void Draw(GameEvent gameEvent, Graphics g)
        {
            Team winner = gameEvent.model.Winner;

            if (winner != Team.neutral)
            {
                Image gameOverImage = null;
                gameOverImage = (winner == Team.blue) ? greenGameOver : blueGameOver;

                if (gameEvent.graphics != null)
                {
                    g.DrawImage(gameOverImage, (int)(gameEvent.screenWidth  - gameOverImage.Width) / 2, 
                                               (int)(gameEvent.screenHeight - gameOverImage.Height) / 2);
                }
            }
        }
    }
}
