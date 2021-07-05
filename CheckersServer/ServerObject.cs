using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Globalization;


namespace CheckersServer
{
    class ServerObject
    {
        public NetworkStream Stream { get; set; }
        static TcpListener tcpListener; //сервер для прослушивания
        public static int connectedUsers = 0; //счетчик подключенных клиентов (в данный момент)

        static List<PlayerObject> players = new List<PlayerObject>(); // все подключения
        // прослушивание входящих подключений
        protected internal void Listen()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, 8888);
                tcpListener.Start();
                Console.WriteLine("Сервер запущен. Ожидание подключений...");

                while (true)
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();
                    PlayerObject playerObject = new PlayerObject(tcpClient, this);
                    playerObject.CheckIn();
                    if (connectedUsers == 2)
                        break;
                }
                ManagerObject managerObject = new ManagerObject();
                Thread managerThread = new Thread(new ThreadStart(managerObject.Process));
                managerThread.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
            }
        }
        protected internal void AddConnection(PlayerObject playerObject)
        {
            players.Add(playerObject);
            ServerObject.connectedUsers++;
        }

        // отключение всех клиентов-
        protected internal void Disconnect()
        {
            tcpListener.Stop(); // остановка сервера
            for (int i = 0; i < players.Count; i++)
            {
                players[i].Close(); // отключение клиента 
            }
            Environment.Exit(0); // завершение процесса
        }

        protected internal void BroadcastMessage(string message, string id)
        {
            for (int i = 0; i < players.Count; i++)
            {
                byte[] data = Encoding.Unicode.GetBytes(message);
                players[i].Stream.Write(data, 0, data.Length); // передача данных
            }
        }









    }
}
