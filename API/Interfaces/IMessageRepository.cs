using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface IMessageRepository
    {
        Task<Group> GetGroupForConnection(string connectionId);
        void AddMessageGroup(Group group);
        void RemoveMessageConnection(Connection connection);
        Task<Connection> GetMessageConnection(string connectionId);
        Task<Group> GetMessageGroup(string groupName);
        void AddMessage(Message message);
        void DeleteMessage(Message message);
        Task<Message> GetMessage(int id);
        Task<PagedList<MessageDto>> GetMessagesForUser(MessagesParams messagesParams);
        Task<IEnumerable<MessageDto>> GetMessagesThread(string currentUsername, string recipientUsername);        
    }
}