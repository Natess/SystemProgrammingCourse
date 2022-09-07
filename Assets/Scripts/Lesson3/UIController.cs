using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Chat
{
    internal class UIController : MonoBehaviour
    {
        [SerializeField] private Button buttonStartServer;
        [SerializeField] private Button buttonShutDownServer;
        [SerializeField] private Button buttonConnectClient;
        [SerializeField] private Button buttonDisconnectClient;
        [SerializeField] private Button buttonSendMessage;
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private TextField textField;
        [SerializeField] private TMP_InputField nameField;
        [SerializeField] private Server server;
        [SerializeField] private Client client;

        private void Start()
        {
            buttonStartServer.onClick.AddListener(() => StartServer());
            buttonShutDownServer.onClick.AddListener(() => ShutDownServer());
            buttonConnectClient.onClick.AddListener(() => Connect());
            buttonDisconnectClient.onClick.AddListener(() => Disconnect());
            buttonSendMessage.onClick.AddListener(() => SendMessage());
            client.onMessageReceive += ReceiveMessage;
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
                SendMessage();
        }

        private void StartServer()
        {
            server.StartServer();
        }

        private void ShutDownServer()
        {
            server.ShutDownServer();
        }

        private void Connect()
        {
            if (nameField?.text == null || nameField?.text?.Length == 0)
            {
                textField.ReceiveMessage("Input your name");
                return;
            }

            client.Connect(nameField.text);
        }

        private void Disconnect()
        {
            client.Disconnect();
        }

        private void SendMessage()
        {
            if(inputField?.text == null || inputField?.text?.Length == 0)
                return;
            client.SendMessage(inputField.text);
            inputField.text = "";
        }

        public void ReceiveMessage(object message)
        {
            textField.ReceiveMessage(message);
        }
    }
}

