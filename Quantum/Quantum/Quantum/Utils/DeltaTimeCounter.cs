using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quantum.Quantum.Utils
{
    class DeltaTimeCounter
    {
        private long lastMeasure = DateTime.Now.Ticks;

        public long MarkAndMeasure(){
            long curentTime = DateTime.Now.Ticks;
            long timeChange = curentTime - lastMeasure;
            lastMeasure = curentTime;

            return timeChange;
        }

        internal long PrintAndMeasureDelta(string message)
        {
            long delta = MarkAndMeasure();
            GamePrints.PrintPerformance(message + ": " + (delta / 1000.0) / 1000.0);
            return delta;
        }
    }
}
