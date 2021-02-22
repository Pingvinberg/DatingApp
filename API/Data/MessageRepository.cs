using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public MessageRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public void AddMessageGroup(Group group)
        {
            _context.Groups.Add(group);
        }

        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Remove(message);
        }

        public async Task<Connection> GetMessageConnection(string connectionId)
        {
            return await _context.Connections.FindAsync(connectionId);
        }

        public async Task<Group> GetMessageGroup(string groupName)
        {
            return await _context.Groups
                .Include(x => x.Connections)
                .FirstOrDefaultAsync(x => x.Name == groupName);

        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages
                .Include(u => u.Sender)
                .Include(u => u.Recipient)
                .SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessagesParams messagesParams)
        {
            var query = _context.Messages
            .OrderByDescending(m => m.MessageSent)
            .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)
            .AsQueryable();
            query = messagesParams.Container switch
            {
                "Inbox" => query.Where(u => u.RecipientUsername == messagesParams.Username 
                    && u.RecipientDeleted == false),
                "Outbox" => query.Where(u => u.SenderUsername == messagesParams.Username 
                    && u.SenderDeleted == false),
                _ => query.Where(u => u.RecipientUsername == messagesParams.Username 
                    && u.RecipientDeleted == false 
                    && u.DateRead == null)
            };

            var messages = query;

            return await PagedList<MessageDto>.CreateAsync(query, messagesParams.PageNumber, messagesParams.PageSize);            
        }

        public async Task<IEnumerable<MessageDto>> GetMessagesThread(string currentUsername, string recipientUsername)
        {
            var messages = await _context.Messages
                .Include(u => u.Sender).ThenInclude(p => p.Photos)
                .Include(u => u.Recipient).ThenInclude(p => p.Photos)
                .Where(m => m.RecipientUsername == currentUsername && m.RecipientDeleted == false
                        && m.SenderUsername == recipientUsername
                        || m.RecipientUsername == recipientUsername
                        && m.SenderUsername == currentUsername && m.SenderDeleted == false
                        )
                .OrderBy(m => m.MessageSent)
                .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
            var unreadMessages = messages.Where(m => m.DateRead == null 
                && m.RecipientUsername == currentUsername)
                .ToList();

            if(unreadMessages.Any())
            {
                foreach(var message in unreadMessages)
                {
                    message.DateRead = DateTime.UtcNow;
                }
                await _context.SaveChangesAsync();
            } 

            return messages;
        }

        public void RemoveMessageConnection(Connection connection)
        {
            _context.Connections.Remove(connection);
        }
        public async Task<Group> GetGroupForConnection(string connectionId)
        {
            return await _context.Groups.Include(c => c.Connections)
                .Where(c => c.Connections.Any(x => x.ConnectionId == connectionId))
                .FirstOrDefaultAsync();
        }
    }
}