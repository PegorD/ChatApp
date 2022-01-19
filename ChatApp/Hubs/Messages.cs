using ChatApp.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.OpenApi.Models;
using SignalRSwaggerGen.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Hubs
{
    public class Messages : Hub
    {
        public const string channelsGroupName = "ChannelsGroup";
        public const string messagesGroupName = "MessagesGroup";
        private readonly IDbContextFactory<ChatAppContext> _contextFactory;

        public Messages(IDbContextFactory<ChatAppContext> factory)
        {
            _contextFactory = factory;
        }

        public async Task SubscribeToChannelsChannel()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, channelsGroupName);
        }

        public async Task<List<Channel>> GetChannels()
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                return await context.Channels.ToListAsync();
            }
        }
        public async Task<Channel> AddChannel(Channel channel)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                context.Channels.Add(channel);
                await context.SaveChangesAsync();
                await Clients.Group(channelsGroupName).SendAsync("channelAdded",channel);
                return channel;
            }
        }

        public async Task RemoveChannel(int channelId)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                var channel = await context.Channels.FindAsync(channelId);
                if (channel != null)
                {
                    context.Channels.Remove(channel);
                    await context.SaveChangesAsync();
                    await Clients.Group(channelsGroupName).SendAsync("channelDeleted", channelId);
                    await Clients.Group(messagesGroupName + channelId).SendAsync("channelDeleted", channelId);
                }
            }
        }
        public async Task SubscribeToMessageChannel(int channelId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, messagesGroupName + channelId);
        }

        public async Task<List<Message>> GetChannelMessages(int channelId)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                return await context.Messages.Where(x => x.ChannelId == channelId).ToListAsync();
            }
        }

        public async Task<Message> AddChannelMessage(int channelId, Message message)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                message.CreatedOn = DateTime.UtcNow;
                context.Messages.Add(message);
                await context.SaveChangesAsync();
                await Clients.GroupExcept(messagesGroupName + channelId, new List<string> { Context.ConnectionId }).SendAsync("messageAdded", message);
                return message;
            }
        }

        public async Task RemoveChannelMessage(int channelId, int messageId)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                var message = await context.Messages.FindAsync(messageId);
                if (message != null)
                {
                    context.Messages.Remove(message);
                    await context.SaveChangesAsync();
                    await Clients.Group(messagesGroupName + channelId).SendAsync("messageDeleted", messageId);
                }
            }
        }
    }
}
