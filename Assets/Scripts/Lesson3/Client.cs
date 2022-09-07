using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Chat
{
    public class Client : MonoBehaviour
    {
        public delegate void OnMessageReceive(object message);
        public event OnMessageReceive onMessageReceive;
        private const int MAX_CONNECTION = 10;
        private int port = 0;
        private int serverPort = 5805;
        private int hostID;
        private int reliableChannel;
        private int connectionID;
        private bool isConnected = false;
        private byte error;
        string name;
        private bool repeatDisconnectMessage;

        public void Connect(string name)
        {
            if (isConnected)
                return;
            NetworkTransport.Init();
            ConnectionConfig conectionConfig = new ConnectionConfig();
            reliableChannel = conectionConfig.AddChannel(QosType.Reliable);
            HostTopology topology = new HostTopology(conectionConfig, MAX_CONNECTION);
            hostID = NetworkTransport.AddHost(topology, port);
            connectionID = NetworkTransport.Connect(hostID, "127.0.0.1", serverPort, 0, out error);
            if ((NetworkError)error == NetworkError.Ok)
            {
                isConnected = true;
                this.name = name;
            }
            else
                Debug.Log((NetworkError)error);
        }
        public void Disconnect()
        {
            if (!isConnected)
                return;
            NetworkTransport.Disconnect(hostID, connectionID, out error);
            isConnected = false;
            onMessageReceive?.Invoke($"You have been disconnected from server.");
            repeatDisconnectMessage = true;
        }

        void Update()
        {
            if (!isConnected) return;
            byte[] recBuffer = new byte[1024];
            int bufferSize = 1024;
            NetworkEventType recData = NetworkTransport.Receive(out int recHostId, out int connectionId, out
                int channelId, recBuffer, bufferSize, out int dataSize, out error);

            while (recData != NetworkEventType.Nothing)
            {
                switch (recData)
                {
                    case NetworkEventType.Nothing:
                        break;
                    case NetworkEventType.ConnectEvent:
                        onMessageReceive?.Invoke($"You have been connected to server.");
                        Debug.Log($"You have been connected to server.");
                        SendMessage(name);
                        break;
                    case NetworkEventType.DataEvent:
                        string message = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                        onMessageReceive?.Invoke(message);
                        Debug.Log(message);
                        break;
                    case NetworkEventType.DisconnectEvent:
                        if (!isConnected) return;
                        if (repeatDisconnectMessage)
                        {
                            repeatDisconnectMessage = false;
                            return;
                        }
                        isConnected = false;
                        onMessageReceive?.Invoke($"You have been disconnected from server.");
                        Debug.Log($"You have been disconnected from server.");
                        break;
                    case NetworkEventType.BroadcastEvent:
                        break;
                }
                recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer,
                bufferSize, out dataSize, out error);
            }
        }

        public void SendMessage(string message)
        {
            byte[] buffer = Encoding.Unicode.GetBytes(message);
            NetworkTransport.Send(hostID, connectionID, reliableChannel, buffer, message.Length *
                sizeof(char), out error);
            if ((NetworkError)error != NetworkError.Ok) 
                Debug.Log((NetworkError)error);
        }
    }
}