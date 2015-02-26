using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using Quantum.Quantum.Utils;
using System.Windows;
using Quantum.Quantum.Model;

namespace Quantum.Quantum
{
    class GlobalRender : GameController
    {
        private Image background = Image.FromFile(@"Resources\Background.jpg");
        private Image blueGeneralImage = Image.FromFile(@"Resources\blue-general.png");
        private Image greenGeneralImage = Image.FromFile(@"Resources\green-general.png");
        private Image blueDroneImage = Image.FromFile(@"Resources\blue-drone.png");
        private Image blueOutpostImage = Image.FromFile(@"Resources\blue-outpost.png");
        private Image greenDroneImage = Image.FromFile(@"Resources\green-drone.png");
        private Image greenOutpostImage = Image.FromFile(@"Resources\green-outpost.png");
        private Image greyOutpostImage = Image.FromFile(@"Resources\grey-outpost.png");

        private Pen grayPen = new Pen(Color.Gray, 3);

        private Pen whitePen = new Pen(Color.White, 1);

        private Pen greenWidePen      = new Pen(Color.Yellow, 3);
        private Pen greenWideHelpPen  = new Pen(Color.DarkOrange, 5);

        private Pen blueWidePen       = new Pen(Color.Red, 3);
        private Pen blueWideHelpPen   = new Pen(Color.DarkRed, 5);
        private float floaXShift = 0;

        private Random random = new Random();

        public void execute(GameEvent gameEvent)
        {
            if (gameEvent.graphics == null) return;

            gameEvent.graphics.DrawImage(background, 0, 0, (float)gameEvent.model.mapWidth,
                                                           (float)gameEvent.model.mapHeight);

            drawOutposts(gameEvent);

            List<General> generals = gameEvent.model.Generals;
            GeneralsDronesCache dronesCache = gameEvent.game.smallCache;

            foreach (General general in generals)
            {
                drawGeneral(gameEvent, general);
            }

            foreach (General general in generals)
            {
                drawDrone(gameEvent, general, dronesCache.getTeamCache(general.Team));
            }

            drawBeam(gameEvent);
        }

        private void drawGeneral(GameEvent gameEvent, General general)
        {
            Image generalImage;

            if (general.Team == Team.green)
                generalImage = greenGeneralImage;
            else
                generalImage = blueGeneralImage;
            RotateImage(generalImage, general, gameEvent);
        }

        private void drawOutposts(GameEvent gameEvent)
        {
            List<Outpost> Outposts = gameEvent.model.Outposts;
            foreach (Outpost outpost in Outposts)
            {
                if (outpost.Team == Team.neutral)
                    gameEvent.graphics.DrawImage(greyOutpostImage, (int)outpost.Position.X - 80, (int)outpost.Position.Y - 80, 160, 160);
                else if (outpost.Team == Team.blue)
                    gameEvent.graphics.DrawImage(blueOutpostImage, (int)outpost.Position.X - 50, (int)outpost.Position.Y - 50, 100, 100);
                else if (outpost.Team == Team.green)
                    gameEvent.graphics.DrawImage(greenOutpostImage, (int)outpost.Position.X - 50, (int)outpost.Position.Y - 50, 100, 100);
            }
        }

        public void drawDrone(GameEvent gameEvent, General general, DronesCache cache)
        {
            QuantumModel model = gameEvent.model;
            
            Image droneImage;
            
            if (general.Team == Team.green)
                droneImage = greenDroneImage;
            else
                droneImage = blueDroneImage;

            int densityBlock = (int)cache.frameCacheSize;

            cache.findDrones(new Vector(0, 0),
                             new Vector(model.mapWidth, model.mapHeight),
            drones =>
            {
                drawDrones(gameEvent.graphics, droneImage, drones, (densityBlock * densityBlock) / 100);
            }); 
        }

        public void drawDrones(Graphics g, Image droneImage, IEnumerable<Drone> drones, int maxDrones)
        {
            int scale = 10, doubleScale = scale * 2;
            int i = 0;

            maxDrones += 3;

            foreach (Drone drone in drones)
            {
                if (i++ > maxDrones) return;

                g.DrawImage(droneImage, (int)drone.Position.X - scale, (int)drone.Position.Y - scale, doubleScale, doubleScale);
            }
        }

        private void RotateImage(Image image, General general, GameEvent gameEvent)
        {
            Matrix mat = new Matrix();
            double angle = Math.Atan2(general.PrevSpeed.Y, general.PrevSpeed.X);
            mat.RotateAt(90 + (float)(180 * angle / Math.PI), new PointF((float)general.Position.X, (float)general.Position.Y));
            mat.Translate(-16, -28);
            gameEvent.graphics.Transform = mat;
            gameEvent.graphics.DrawImage(image, (int)general.Position.X, (int)general.Position.Y, 32, 55);
            gameEvent.graphics.Transform = new Matrix();
        }

        private void drawBeam(GameEvent gameEvent) 
        {
            List<Beam> beams = gameEvent.model.Beams;

            foreach (Beam beam in gameEvent.model.Beams)
            {

                Pen whitePen = this.whitePen;

                Pen mainPen = null;
                Pen helpPen = null;

                if (beam.team == Team.blue)
                {
                    mainPen  = this.blueWidePen;
                    helpPen  = this.blueWideHelpPen;
                    this.floaXShift = -2;
                }
                else if (beam.team == Team.green)
                {
                    mainPen = this.greenWidePen;
                    helpPen = this.greenWideHelpPen;
                    this.floaXShift = 2;
                }

                gameEvent.graphics.DrawLine(helpPen, (float)beam.position1.X + floaXShift, (float)beam.position1.Y, (float)beam.position2.X, (float)beam.position2.Y);
                gameEvent.graphics.DrawLine(mainPen, (float)beam.position1.X + floaXShift, (float)beam.position1.Y, (float)beam.position2.X, (float)beam.position2.Y);
                gameEvent.graphics.DrawLine(whitePen, (float)beam.position1.X + floaXShift, (float)beam.position1.Y, (float)beam.position2.X, (float)beam.position2.Y);
            }

            gameEvent.model.Beams.Clear();
        }

    }
}
