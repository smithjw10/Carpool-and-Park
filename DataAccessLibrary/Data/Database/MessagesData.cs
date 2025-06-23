using DataAccessLibrary.Model;
using DataAccessLibrary.Model.Database_Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLibrary.Data.Database
{
    public class MessagesData : IMessagesData
    {
        private readonly ISQLDataAccess _db;

        public MessagesData(ISQLDataAccess db)
        {
            _db = db;
        }

        public async Task<List<MessagesModel>> GetAllMessages()
        {
            string sql = "SELECT * FROM Messages";
            return await _db.LoadData<MessagesModel, dynamic>(sql, new { });
        }

        public async Task<MessagesModel> GetMessage(int messageId)
        {
            string sql = "SELECT * FROM Messages WHERE MessageID = @MessageId";
            var result = await _db.LoadData<MessagesModel, dynamic>(sql, new { MessageId = messageId });
            return result.FirstOrDefault();
        }

        public async Task UpdateMessage(MessagesModel message)
        {
            string sql = @"UPDATE Messages SET SenderID = @SenderId, ReceiverID = @ReceiverId, MessageText = @MessageText, SentTime = @SentTime, ReadStatus = @ReadStatus WHERE MessageID = @MessageId";
            await _db.SaveData(sql, message);
        }

        public async Task DeleteMessage(int messageId)
        {
            string sql = "DELETE FROM Messages WHERE MessageID = @MessageId";
            await _db.SaveData(sql, new { MessageId = messageId });
        }
    }
}
