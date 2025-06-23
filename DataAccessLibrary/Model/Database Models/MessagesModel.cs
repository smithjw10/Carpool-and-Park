using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Model.Database_Models
{
    public class MessagesModel
    {
        public int MessageId { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string MessageText { get; set; }
        public DateTime SentTime { get; set; }
        public bool ReadStatus { get; set; }

        public MessagesModel() { }

        public MessagesModel(int messageId, int senderId, int receiverId, string messageText, DateTime sentTime, bool readStatus)
        {
            MessageId = messageId;
            SenderId = senderId;
            ReceiverId = receiverId;
            MessageText = messageText;
            SentTime = sentTime;
            ReadStatus = readStatus;
        }
    }

}
