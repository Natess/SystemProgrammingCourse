using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Chat
{
    public class Server : MonoBehaviour
    {
        private const int MAX_CONNECTION = 10;
        private int port = 5805;
        private int hostID;
        private int reliableChannel;
        private bool isStarted = false;
        private byte error;
        ConnectedUsers users = new ConnectedUsers();
        
        public void StartServer()
        {
            NetworkTransport.Init();
            ConnectionConfig conectionConfig = new ConnectionConfig();
            reliableChannel = conectionConfig.AddChannel(QosType.Reliable);
            HostTopology topology = new HostTopology(conectionConfig, MAX_CONNECTION);
            hostID = NetworkTransport.AddHost(topology, port);
            isStarted = true;
        }
        
        public void ShutDownServer()
        {
            if (!isStarted) return;
            NetworkTransport.RemoveHost(hostID);
            NetworkTransport.Shutdown();
            isStarted = false;
        }

        public void SendMessage(string message, int connectionID)
        {
            byte[] buffer = Encoding.Unicode.GetBytes(message);
            NetworkTransport.Send(hostID, connectionID, reliableChannel, buffer, message.Length *
                sizeof(char), out error);
            if ((NetworkError)error != NetworkError.Ok)
                Debug.Log((NetworkError)error);
        }

        public void SendMessageToAll(string message)
        {
            foreach (var id in users.GetAllId())
            {
                SendMessage(message, id);
            }
        }

        void Update()
        {
            if (!isStarted)
                return;
            byte[] recBuffer = new byte[1024];
            int bufferSize = 1024;
            NetworkEventType recData = NetworkTransport.Receive(out int recHostId, out int connectionId,
                out int channelId, recBuffer, bufferSize, out int dataSize, out error);
            
            while (recData != NetworkEventType.Nothing)
            {
                switch (recData)
                {
                    case NetworkEventType.Nothing:
                        break;
                    case NetworkEventType.ConnectEvent:
                        users.Add(connectionId);
                        Debug.Log($"Player {connectionId} has connected.");
                        break;
                    case NetworkEventType.DataEvent:
                        string message = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                        if (users[connectionId].IsFirstMessage)
                        {
                            users[connectionId].Name = message;
                            SendMessageToAll($"Player {users[connectionId].Name} has connected.");
                            Debug.Log($"Player {connectionId}: {message}");
                        }
                        else
                        {
                            SendMessageToAll($"{users[connectionId].Name}: {message}");
                            Debug.Log($"Player {users[connectionId].Name}: {message}");
                        }
                        break;
                    case NetworkEventType.DisconnectEvent:
                        SendMessageToAll($"Player {users[connectionId].Name} has disconnected.");
                        Debug.Log($"Player {users[connectionId].Name} has disconnected.");
                        users.Remove(connectionId);
                        break;
                    case NetworkEventType.BroadcastEvent:
                        break;
                }
                recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer,
                    bufferSize, out dataSize, out error);
            }
        }

    }
}