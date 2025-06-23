using DataAccessLibrary.Model.Database_Models;

namespace DataAccessLibrary.Data.Database
{
    public interface IMessagesData
    {
        Task DeleteMessage(int messageId);
        Task<List<MessagesModel>> GetAllMessages();
        Task<MessagesModel> GetMessage(int messageId);
        Task UpdateMessage(MessagesModel message);
    }
}