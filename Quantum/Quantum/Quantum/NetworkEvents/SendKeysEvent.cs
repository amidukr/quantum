using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quantum.Quantum.NetworkEvents
{
    [Serializable]
    class SendKeysEvent
    {
        public Dictionary<object, bool> keyTable = new Dictionary<object, bool>();
    }
}
