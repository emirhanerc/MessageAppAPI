using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MessageAppAPI.Dtos.Message;
using MessageAppAPI.Entities;
using MessageAppAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MessageAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IMessageRepository _messageRepository;
        private readonly UserManager<AppUser> _userManager;

        public AdminController(IMessageRepository messageRepository, UserManager<AppUser> userManager)
        {
            _messageRepository = messageRepository;
            _userManager = userManager;
        }

        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUsersWithEmailAndId()
        {
            var users = await _userManager.Users.Select(user => new
            {
                user.Id,
                user.Email
            }).ToListAsync();

            return Ok(users);
        }
        
        [HttpGet("GetAllMessages")]
        public async Task<IActionResult> GetAllMessages()
        {
            var messages = await _messageRepository.GetAll().ToListAsync();
            return Ok(messages);
        }

        [HttpPost("SendMessageToUser")]
        public async Task<IActionResult> SendMessageToUser(AddMessageByUserDto dto)
        {
            var senderUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (senderUserId == null)
            {
                return Unauthorized("Kullanıcı kimliği bulunamadı.");
            }

            var senderUser = await _userManager.FindByIdAsync(senderUserId);
            if (senderUser == null)
            {
                return NotFound("Gönderen kullanıcı bulunamadı.");
            }

            var receiverUser = await _userManager.FindByIdAsync(dto.ReceiverUserId);
            if (receiverUser == null)
            {
                return NotFound("Alıcı kullanıcı bulunamadı.");
            }

            var message = new Message
            {
                SenderId = senderUserId,
                Sender = senderUser,
                ReceiverId = dto.ReceiverUserId,
                Receiver = receiverUser,
                Description = dto.MessageContent,
            };

            await _messageRepository.AddAsync(message);
            await _messageRepository.SaveAsync();

            return Ok("Mesaj başarıyla gönderildi.");
        }
        

        [HttpPost("SendBulkMessage")]
        public async Task<IActionResult> SendBulkMessage(SendBulkMessageDto dto)
        {
            var senderUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (senderUserId == null)
            {
                return Unauthorized("Kullanıcı kimliği bulunamadı.");
            }

            var senderUser = await _userManager.FindByIdAsync(senderUserId);
            if (senderUser == null)
            {
                return NotFound("Gönderen kullanıcı bulunamadı.");
            }

            var usersInRole = await _userManager.GetUsersInRoleAsync("User");

            var messages = usersInRole.Select(user => new Message
            {
                SenderId = senderUserId,
                Sender = senderUser,
                ReceiverId = user.Id,
                Receiver = user,
                Description = dto.MessageContent
            }).ToList();

            await _messageRepository.AddRangeAsync(messages);
            await _messageRepository.SaveAsync();

            return Ok("Toplu mesajlar başarıyla gönderildi.");
        }
        [HttpPut("UpdateMessage/{messageId}")]
        public async Task<IActionResult> UpdateMessage(UpdateMessageDto dto)
        {
            var message = await _messageRepository.GetByIdAsync(dto.MessageId);
            if (message == null)
            {
                return NotFound("Mesaj bulunamadı.");
            }

            message.Description = dto.MessageContent;

            _messageRepository.Update(message);
            await _messageRepository.SaveAsync();

            return Ok("Mesaj başarıyla güncellendi.");
        }

        
        [HttpDelete("DeleteMessage/{messageId}")]
        public async Task<IActionResult> DeleteMessage(string messageId)
        {
            var message = await _messageRepository.GetByIdAsync(messageId);
            if (message == null)
            {
                return NotFound("Mesaj bulunamadı.");
            }

            _messageRepository.Remove(message);
            await _messageRepository.SaveAsync();

            return Ok("Mesaj başarıyla silindi.");
        }
    }
}
