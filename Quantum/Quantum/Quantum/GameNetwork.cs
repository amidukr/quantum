using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;

namespace Quantum.Quantum
{
    delegate void Callback();
    class MessageBuffer
    {
        public Socket    socket           {get; private set;}
        public int       NextMessageSize  { get; set; }
        public int       ReadOffset       { get; set; }
        public byte[]    Buffer           { get; private set; }
        public Callback  Callback { get; set; }

        public MessageBuffer(Socket socket, byte[] buffer, Callback callback)
        {
            this.socket     = socket;
            NextMessageSize = -1;
            ReadOffset      = 0;
            this.Buffer     = buffer;
            this.Callback   = callback;
        }
    }

    class GameNetwork
    {
        private Socket socket;
        private List<Socket> clients = new List<Socket>();
        private IFormatter formatter = new BinaryFormatter();

        public List<object> Events { get; private set; }
        private List<Callback> eventHandlers = new List<Callback>();

        public GameNetwork()
        {
            Events = new List<object>();
        }

        public void Listen(int port, Callback callback)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);

            socket.Bind(localEndPoint);
            socket.Listen(2);

            socket.BeginAccept(onAccept, callback);
        }

        public void Join(string host, int port)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(host, port);

            clients.Add(socket);

            Read(socket);
        }

        public void BroadcastMessage(object message)
        {
            byte[] header = new byte[4];
            MemoryStream stream = new MemoryStream();
            stream.Write(header, 0, 4);
            formatter.Serialize(stream, message);

            stream.Seek(0, SeekOrigin.Begin);
            byte[] buffer = stream.GetBuffer();
            int buffLen = (int)stream.Length;
            
            header = BitConverter.GetBytes(buffLen - header.Length);
            buffer[0] = header[0];
            buffer[1] = header[1];
            buffer[2] = header[2];
            buffer[3] = header[3];

            //clients.RemoveAll(m => !m.Connected);

            foreach (Socket client in clients)
            {
                //client.BeginSend(buffer, buffLen, 0, 0, OnSent, client);
                client.Send(buffer, buffLen, 0);
            }
        }

        private void OnSent(IAsyncResult ir)
        {
            Socket clientSocket = (Socket)ir.AsyncState;

            int sent = clientSocket.EndSend(ir);
        }

        public void HandleEvent(Callback callback) {
            eventHandlers.Add(callback);
        }

        public object TakeFirst()
        {
            object first = Events.First();
            Events.RemoveAt(0);
            return first;
        }

        public List<object> ReceiveEvents()
        {
            List<object> result = Events;
            Events = new List<object>();
            return result;
        }

        private void onAccept(IAsyncResult ar)
        {
            Socket client = socket.EndAccept(ar);

            clients.Add(client);

            Read(client);

            Callback callback = (Callback)ar.AsyncState;
            callback();
        }

        private void onClose(IAsyncResult ir)
        {
            Socket client = (Socket)ir.AsyncState;

            client.EndAccept(ir);
        }

        private void Read(Socket client)
        {
            byte[] messageHeader = new byte[4];

            ReceiveBuffer(client, messageHeader, () =>
            {
                int messageSize = BitConverter.ToInt32(messageHeader, 0);
                byte[] messageBuffer = new byte[messageSize];

                ReceiveBuffer(client, messageBuffer, () =>
                {
                    MemoryStream stream = new MemoryStream(messageBuffer);
                    object messageEvent = formatter.Deserialize(stream);

                    Events.Add(messageEvent);
                    List<Callback> listeners = eventHandlers;
                    eventHandlers = new List<Callback>();

                    foreach (Callback callback in listeners)
                    {
                        callback();
                    }

                    Read(client);
                });
            });
        }

        private void ReceiveBuffer(Socket socket, byte[] buffer, Callback callback)
        {
            MessageBuffer messageBuffer = new MessageBuffer(socket, buffer, callback);

            if (!messageBuffer.socket.Connected)
            {
                clients.Remove(messageBuffer.socket);
                return;
            }

            socket.BeginReceive(messageBuffer.Buffer, 0, messageBuffer.Buffer.Length, 0, OnReceiveBuffer, messageBuffer);
        }

        private void OnReceiveBuffer(IAsyncResult ar) {
            MessageBuffer messageBuffer = (MessageBuffer)ar.AsyncState;
            if (!messageBuffer.socket.Connected)
            {
                clients.Remove(messageBuffer.socket);
                return;
            }
            int received = messageBuffer.socket.EndReceive(ar);

            messageBuffer.ReadOffset += received;

            if (messageBuffer.ReadOffset > messageBuffer.Buffer.Length)
            {
                throw new Exception("Extra bytes read");
            }
            else if (messageBuffer.ReadOffset < messageBuffer.Buffer.Length)
            {
                messageBuffer.socket.BeginReceive(messageBuffer.Buffer, messageBuffer.ReadOffset, messageBuffer.Buffer.Length - messageBuffer.ReadOffset, 0, OnReceiveBuffer, messageBuffer);
            }
            else
            {
                messageBuffer.Callback();
            }
        }
    }
}
