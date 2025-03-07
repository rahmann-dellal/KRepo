using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KFP.Messages
{
    public class UserAddedMessage
    {
        public int UserId { get; set; }
        public UserAddedMessage(int userId)
        {
            UserId = userId; 
        }
    }
}
