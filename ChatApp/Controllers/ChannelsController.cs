using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChatApp.Models;
using Microsoft.AspNetCore.SignalR;
using ChatApp.Hubs;

namespace ChatApp.Controllers
{
    [Route("channels")]
    [ApiController]
    public class ChannelsController : ControllerBase
    {
        private readonly ChatAppContext _context;
        private readonly IHubContext<Messages> _hubContext;
        public ChannelsController(ChatAppContext context, IHubContext<Messages> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        // GET: api/Channels
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Channel>>> GetChannels()
        {
            return await _context.Channels.ToListAsync();
        }

        // POST: api/Channels
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Channel>> PostChannels(Channel channel)
        {
            _context.Channels.Add(channel);
            await _context.SaveChangesAsync();
            await _hubContext.Clients.Groups(Messages.channelsGroupName).SendAsync("channelAdded",channel);
            return Ok(channel);            
        }

        // DELETE: api/Channels/5
        [HttpDelete("{channelId}")]
        public async Task<IActionResult> DeleteChannel(int channelId)
        {
            var channel = await _context.Channels.FindAsync(channelId);
            if (channel == null)
            {
                return NotFound();
            }

            _context.Channels.Remove(channel);
            await _context.SaveChangesAsync();
            await _hubContext.Clients.Group(Messages.channelsGroupName).SendAsync("channelDeleted", channelId);
            await _hubContext.Clients.Group(Messages.messagesGroupName + channelId).SendAsync("channelDeleted", channelId);
            return Ok();
        }

        // GET: api/Channels/5
        [HttpGet("{channelId}/messages")]
        public async Task<ActionResult<List<Message>>> GetChannelMessages(int channelId)
        {
            var messages = await _context.Messages.Where(x => x.ChannelId == channelId).ToListAsync();

            return Ok(messages);
        }


        // POST: api/Channels
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("{channelId}/messages")]
        public async Task<ActionResult<Message>> PostMessage(int channelId,Message message)
        {
            if (!ChannelExists(channelId))
            {
                return NotFound();
            }
            message.ChannelId = channelId;
            message.CreatedOn = DateTime.UtcNow;
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            await _hubContext.Clients.Groups(Messages.messagesGroupName + channelId).SendAsync("messageAdded", message);

            return Ok(message);
        }

        // DELETE: api/Channels/5
        [HttpDelete("{channelId}/messages/{messageId}")]
        public async Task<IActionResult> DeleteMessage(int channelId,int messageId)
        {
            var message = await _context.Messages.FindAsync(messageId);
            if (message == null)
            {
                return NotFound();
            }

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();
            await _hubContext.Clients.Group(Messages.messagesGroupName + channelId).SendAsync("messageDeleted", messageId);
            return Ok();
        }


        private bool ChannelExists(int id)
        {
            return _context.Channels.Any(e => e.ChannelId == id);
        }
    }
}
