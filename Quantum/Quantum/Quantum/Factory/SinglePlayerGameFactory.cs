using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quantum.Quantum.Factory
{
    class SinglePlayerGameFactory : GameFactory
    {
        public void create(int screenWidth, int screenHeight, OnAsynCreate callback)
        {
            QuantumGame game = new QuantumGame();
            QuantumMapBuilder mapBuilder = new QuantumMapBuilder();
            game.start(mapBuilder.initializeMap(screenWidth, screenHeight), screenWidth, screenHeight);

            callback(game);
        }
    }
}
