using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class MessagesController : BaseApiController
    {        
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public MessagesController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;            
        }

        /*[HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            var username = User.GetUserName();

            if (username == createMessageDto.RecipientUsername.ToLower())
                return BadRequest("You cannot send messages to yourself");

            var sender = await _userRepository.GetUserByUsernameAsync(username);
            var recipient = await _userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

            if (recipient == null) return NotFound();

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDto.Content
            };

            _unitOfWork.MessageRepository.AddMessage(message);

            if (await _unitOfWork.MessageRepository.SaveAllAsync()) return Ok(_mapper.Map<MessageDto>(message));

            return BadRequest("Failed to send message");

        }*/

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser([FromQuery] MessagesParams messagesParams)
        {
            messagesParams.Username = User.GetUserName();

            var messages = await _unitOfWork.MessageRepository.GetMessagesForUser(messagesParams);

            Response.AddPaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages);

            return messages;
        }

        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
        {
            var currentUsername = User.GetUserName();
            return Ok(await _unitOfWork.MessageRepository.GetMessagesThread(currentUsername, username));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var username = User.GetUserName();
            var messageToDelete = await _unitOfWork.MessageRepository.GetMessage(id);
            if (messageToDelete.Sender.UserName != username && messageToDelete.Recipient.UserName != username)
            {
                return Unauthorized();
            }

            if (messageToDelete.Sender.UserName == username)
            {
                messageToDelete.SenderDeleted = true;
            }
            if (messageToDelete.Recipient.UserName == username)
            {
                messageToDelete.RecipientDeleted = true;
            }
            if (messageToDelete.SenderDeleted && messageToDelete.RecipientDeleted)
            {
                _unitOfWork.MessageRepository.DeleteMessage(messageToDelete);
            }

            if (await _unitOfWork.Complete()) return Ok();
            return BadRequest("Failed to delete message");
        }


    }
}