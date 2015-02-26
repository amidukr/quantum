using Quantum.Quantum.Controllers;
using Quantum.Quantum.NetworkEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quantum.Quantum.Factory
{
    delegate void OnAsynCreate(QuantumGame game);

    class ClientGameFactory: GameFactory
    {
        private GameNetwork network = new GameNetwork();

        private bool inProgress = false;

        public void Join(int width, int height, String host, OnAsynCreate callback)
        {
            if (inProgress) return;

            inProgress = true;
            network.Join(host, 7777);

            network.HandleEvent(() =>
            {
                OnEvent(width, height, callback);
            });
        }

        private void CreateClientGame(QuantumModel model, int width, int height, OnAsynCreate callback)
        {
            QuantumGame game = new QuantumGame();

            game.gameNetwork = network;

            game.AddController(new NetworkSync(false));

            game.start(model, width, height);
            callback(game);
        }

        private void OnEvent(int width, int height, OnAsynCreate callback)
        {
            object message = network.TakeFirst();

            if (message is ShareGameEvent)
            {
                ShareGameEvent shareMessage = (ShareGameEvent)message;

                CreateClientGame(shareMessage.model, width, height, callback);

                return;
            }

            network.HandleEvent(() =>
            {
                OnEvent(width, height, callback);
            });
        }

        public void create(int screenWidth, int screenHeight, OnAsynCreate callback)
        {
            
        }
    }
}
