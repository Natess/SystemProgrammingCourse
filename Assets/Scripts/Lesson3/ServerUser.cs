using System;

namespace Chat
{
    public class ServerUser
    { 
        public Guid Id { get; }
        
        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                IsFirstMessage = false;
                name = value;
            }
        }
        
        public bool IsFirstMessage { get; private set; } = true;

        public ServerUser(int userId)
        {
            Id = new Guid();
        }

    }
}