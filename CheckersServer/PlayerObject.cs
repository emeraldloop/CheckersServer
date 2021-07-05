using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace CheckersServer
{
    class PlayerObject
    {
        protected internal string Id { get; private set; }
        string userName;
        TcpClient client;
        ServerObject server;
        public NetworkStream Stream;
        string team;
        public PlayerObject(TcpClient tcpClient, ServerObject serverObject)
        {   
            
            client = tcpClient;
            server = serverObject;
            serverObject.AddConnection(this);
            Id = ServerObject.connectedUsers.ToString();
            
        }

        public void CheckIn()
        {
            try
            {
                Stream = client.GetStream();
                string message = GetMessage();
                if (Id == "1")
                    team = " (Black team)";
                userName = message + team;
                if (Id == "2")
                    team = " (White team)";
                userName = message + team;
                message = userName + " was connected";
                server.BroadcastMessage(message, Id);
                Console.WriteLine(message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private string GetMessage()
        {
            try
            {
                byte[] data = new byte[64]; //буфер для получаемых данных
                StringBuilder builder = new StringBuilder();
                int bytes = 0;

                do              // получаем сообщение
                {
                    bytes = Stream.Read(data, 0, data.Length);
                    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                }
                while (Stream.DataAvailable);

                return builder.ToString();
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        // закрытие подключения
        protected internal void Close()
        {
            if (Stream != null)
                Stream.Close();
            if (client != null)
                client.Close();
        }
    }
}
