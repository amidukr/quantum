using Quantum.Quantum.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quantum.Quantum
{
    interface GameFactory
    {
        void create(int screenWidth, int screenHeight, OnAsynCreate callback);
    }
}
