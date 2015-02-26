using Quantum.Quantum.NetworkEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Quantum.Quantum.Controllers
{
    class NetworkSync: GameController
    {
        private bool server;

        public NetworkSync(bool server)
        {
            this.server = server;
        }

        public void execute(GameEvent gameEvent)
        {
            GameNetwork network = gameEvent.game.gameNetwork;
            if (server)
            {
                ShareGameEvent shareEvent = new ShareGameEvent();
                shareEvent.model = gameEvent.model;
                network.BroadcastMessage(shareEvent);

                List<object> events = network.ReceiveEvents();

                foreach (object message in events)
                {
                    if (message is SendKeysEvent)
                    {
                        SendKeysEvent keyEvent = ((SendKeysEvent)message);

                        Dictionary<Keys, Keys> keyTransfer = new Dictionary<Keys, Keys>();
                        keyTransfer.Add(Keys.W, Keys.Up);
                        keyTransfer.Add(Keys.S, Keys.Down);
                        keyTransfer.Add(Keys.A, Keys.Left);
                        keyTransfer.Add(Keys.D, Keys.Right);
                        keyTransfer.Add(Keys.Q, Keys.ShiftKey);

                        foreach (var pair in keyTransfer)
                        {
                            gameEvent.game.changeInputState(pair.Value, keyEvent.keyTable.ContainsKey(pair.Key) && keyEvent.keyTable[pair.Key]);
                        }
                    }
                }
            }
            else
            {
                SendKeysEvent sendKeys = new SendKeysEvent();
                sendKeys.keyTable = gameEvent.game.keyTable;
                network.BroadcastMessage(sendKeys);

                List<object> events = network.ReceiveEvents();

                foreach (object message in events)
                {
                    if (message is ShareGameEvent)
                    {
                        gameEvent.game.model = ((ShareGameEvent)message).model;
                    }
                }
            }
        }
    }
}
