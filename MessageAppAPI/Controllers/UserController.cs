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
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IMessageRepository _messageRepository;
        private readonly UserManager<AppUser> _userManager;

        public UserController(IMessageRepository messageRepository, UserManager<AppUser> userManager)
        {
            _messageRepository = messageRepository;
            _userManager = userManager;
        }

        [HttpPost("SendMessageToAdmin")]
        public async Task<IActionResult> SendMessageToAdmin(SendMessageToAdminDto dto)
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

            var adminRole = "Admin";
            var admins = await _userManager.GetUsersInRoleAsync(adminRole);

            if (!admins.Any())
            {
                return NotFound($"Sistemde {adminRole} rolünde kullanıcı bulunamadı.");
            }

            var adminUser = admins.First();

            var message = new Message
            {
                SenderId = senderUserId,
                Sender = senderUser,
                ReceiverId = adminUser.Id,
                Receiver = adminUser,
                Description = dto.MessageContent,
            };

            await _messageRepository.AddAsync(message);
            await _messageRepository.SaveAsync();

            return Ok("Admin kullanıcıya mesaj başarıyla gönderildi.");
        }

        [HttpGet("GetUserMessages")]
        public async Task<IActionResult> GetUserMessages()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized("Kullanıcı kimliği bulunamadı.");
            }

            var messages = await _messageRepository
                .GetAll()
                .Where(m => m.SenderId == userId)
                .ToListAsync();

            return Ok(messages);
        }

        [HttpGet("GetReceivedMessages")]
        public async Task<IActionResult> GetReceivedMessages()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized("Kullanıcı kimliği bulunamadı.");
            }

            var messages = await _messageRepository
                .GetAll()
                .Where(m => m.ReceiverId == userId)
                .ToListAsync();

            return Ok(messages);
        }

        [HttpPut("UpdateUserMessage/{messageId}")]
        public async Task<IActionResult> UpdateUserMessage(UpdateMessageDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized("Kullanıcı kimliği bulunamadı.");
            }

            var message = await _messageRepository.GetByIdAsync(dto.MessageId);
            if (message == null)
            {
                return NotFound("Mesaj bulunamadı.");
            }

            if (message.SenderId != userId)
            {
                return Forbid("Bu mesajı düzenleme izniniz yok.");
            }

            message.Description = dto.MessageContent;

            _messageRepository.Update(message);
            await _messageRepository.SaveAsync();

            return Ok("Mesaj başarıyla güncellendi.");
        }

        [HttpDelete("DeleteUserMessage/{messageId}")]
        public async Task<IActionResult> DeleteUserMessage(string messageId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized("Kullanıcı kimliği bulunamadı.");
            }

            var message = await _messageRepository.GetByIdAsync(messageId);
            if (message == null)
            {
                return NotFound("Mesaj bulunamadı.");
            }

            if (message.SenderId != userId)
            {
                return Forbid("Bu mesajı silme izniniz yok.");
            }

            _messageRepository.Remove(message);
            await _messageRepository.SaveAsync();

            return Ok("Mesaj başarıyla silindi.");
        }
    }
}
