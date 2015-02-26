using Quantum.Quantum.Controllers;
using Quantum.Quantum.NetworkEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quantum.Quantum.Factory
{
   

    class ServerGameFactory : GameFactory
    {
        private GameNetwork network = new GameNetwork();

        private bool inProgress = false;

        private Team currentTeam;
        private QuantumModel model;

        private Random random = new Random();
        
        public void Listen(int width, int height, OnAsynCreate callback)
        {
            if (inProgress) return;

            inProgress = true;
            network.Listen(7777, () =>
            {
                CreateServerGame(width, height, callback);
            });
        }

        private void CreateServerGame(int width, int height, OnAsynCreate callback)
        {
            currentTeam = (random.Next() % 2 == 0) ? Team.blue : Team.green;

            QuantumMapBuilder mapBuilder = new QuantumMapBuilder();
            
            model = mapBuilder.initializeMap(width, height);

            ShareGameEvent shareGame = new ShareGameEvent();
            shareGame.model = model;

            network.BroadcastMessage(shareGame);

            QuantumGame game = new QuantumGame();
            game.gameNetwork = network;
            game.AddController(new NetworkSync(true));
            game.start(model, width, height);

            callback(game);
        }

        public void create(int screenWidth, int screenHeight, OnAsynCreate callback)
        {
            CreateServerGame(screenWidth, screenHeight, callback);
        }
    }
}
