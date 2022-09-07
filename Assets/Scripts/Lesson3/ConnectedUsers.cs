using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat
{
    public class ConnectedUsers
    {
        Dictionary<int, ServerUser> connectedUsers = new Dictionary<int, ServerUser>();

        public ServerUser this[int id]
        {
            get => connectedUsers[id];
        }

        public void Add(int userId)
        {
            connectedUsers[userId] = new ServerUser(userId);
        }

        internal IEnumerable<int> GetAllId()
        {
            return connectedUsers.Keys;
        }

        internal void Remove(int connectionId)
        {
            connectedUsers.Remove(connectionId);
        }
    }
}
