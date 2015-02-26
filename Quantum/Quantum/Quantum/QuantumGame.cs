using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Quantum.Quantum.Controllers;
using Quantum.Quantum;
using System.Windows;
using Quantum.Quantum.Utils;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Quantum.Quantum.Renders;
using Quantum.Quantum.Model;

namespace Quantum.Quantum
{

    interface GameController
    {
        void execute(GameEvent gameEvent);
    }

    class QuantumGame
    {

        public readonly Dictionary<object, bool> keyTable = new Dictionary<object, bool>();
        public Vector mousePosition {get; set;}
        public QuantumModel model {get; set;}
        public GameNetwork gameNetwork { get; set; }

        private MotionBlurFilter motionBlurFilter;
        private readonly WinBanner   winBanner   = new WinBanner();

        
        public readonly GeneralsDronesCache smallCache = new GeneralsDronesCache(20);
        public readonly GeneralsDronesCache largeCache = new GeneralsDronesCache(200);

        private DeltaTimeCounter deltaTimeCounter;
        private bool enabled = false;
        
        private List<GameController> controllers = new List<GameController>();

        private QuantumMapBuilder mapBuilder = new QuantumMapBuilder();

        private Image mainImage;

        public void AddController(GameController controller)
        {
            controllers.Add(controller);
        }

        public void start(QuantumModel model, int screenWidth, int screenHeight)
        {
            motionBlurFilter = new MotionBlurFilter((int)model.mapWidth, (int)model.mapHeight);
            mainImage = new Bitmap((int)model.mapWidth, (int)model.mapHeight);

            controllers.Add(new CheckWinCondition());
            controllers.Add(new GeneralController(Keys.W,  Keys.S,    Keys.A,    Keys.D,     Team.green));
            controllers.Add(new GeneralController(Keys.Up, Keys.Down, Keys.Left, Keys.Right, Team.blue));
            controllers.Add(new DroneOrderingController(Team.green, Keys.Q, MouseButtons.Left));
            controllers.Add(new DroneOrderingController(Team.blue, Keys.ShiftKey, MouseButtons.Right));
            controllers.Add(new DroneController());
            controllers.Add(new DrownRespawnerController());
            controllers.Add(new DronsFighting());
            controllers.Add(new OutpostConquestController());
            controllers.Add(new GlobalRender());

            this.model = model;

            deltaTimeCounter = new DeltaTimeCounter();
            enabled = true;
        }

        public bool playNext(Graphics g, int screenWidth, int screenHeight)
        {
            try
            {
                if (!enabled) return false;

                DeltaTimeCounter performanceCounter = new DeltaTimeCounter();
                
                long deltaTime = deltaTimeCounter.PrintAndMeasureDelta("Seconds per frame");
                GamePrints.Toggle(deltaTime);

                smallCache.cacheModel(model);
                largeCache.cacheModel(model);
                performanceCounter.PrintAndMeasureDelta("Cache drones");

                Graphics mainG = Graphics.FromImage(mainImage);
                GameEvent gameEvent = new GameEvent(this, deltaTime / 100000.0, mainG, screenWidth, screenHeight);

                foreach (GameController controller in controllers)
                {
                    controller.execute(gameEvent);
                    performanceCounter.PrintAndMeasureDelta(controller.ToString());
                }

                motionBlurFilter.Draw(gameEvent, g, mainImage);
                performanceCounter.PrintAndMeasureDelta("Motion blur");

                winBanner.Draw(gameEvent, g);
                performanceCounter.PrintAndMeasureDelta("Win banner");
                

                performanceCounter.PrintAndMeasureDelta("Draw win banner");

                return model.Winner != Team.neutral;
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return true;
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
