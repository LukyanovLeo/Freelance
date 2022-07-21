using Freelance.WebApi.Contracts.Entities;
using Freelance.WebApi.Contracts.Responses;
using Freelance.QueryHandlers.Interfaces;
using Freelance.Repositories.Interfaces;
using System;
using System.Collections.Generic;

using System.Threading.Tasks;

namespace Freelance.QueryHandlers
{
    public class ChatQueryHandler : IChatQueryHandler
    {
        private readonly IChatRepository _chatRepository;

        public ChatQueryHandler(IChatRepository chatRepository)
        {
            _chatRepository = chatRepository;
        }

        public async Task CreateChat(Guid companionId, Guid userId)
        {
            var chatId = await _chatRepository.CreateChat(userId);
            var party = new List<Party> {
                new Party
                {
                    ChatId=chatId,
                    UserId = userId
                },
                new Party
                {
                    ChatId=chatId,
                    UserId = companionId
                }
            };

            await _chatRepository.CreateParty(party);
        }

        public async Task CreateMessage(Guid chatId, string text)
        {
            await _chatRepository.CreateMessage(chatId, text);
        }

        public async Task<Guid> CreateAttachment(string filepath)
        {
            return await _chatRepository.CreateAttachment(filepath);
        }

        // TODO: GetAllChatsOfUser - доделать 
        public async Task<GetChatsResponse> GetAllChatsOfUser(Guid userId)
        {
            var userChatsRaw = await _chatRepository.GetChatsByUserId(userId);
            var userChats = new List<Contracts.DTO.Chat>();
            foreach (var chat in userChatsRaw)
            {
                userChats.Add(new Contracts.DTO.Chat
                {
                    Id = chat.Id,
                    LastMessage = chat.LastMessage,
                    LastMessageDate = chat.LastMessageDate,
                    User = new Contracts.DTO.User
                    {
                        Email = chat.User.Email,
                        Id = chat.User.Id,
                    },
                });
            }
            return new GetChatsResponse
            {
                Chats = userChats
            };
        }

        public async Task<GetChatMessagesResponse> GetChatMessages(Guid chatId)
        {
            var userChatMessagesRaw = await _chatRepository.GetChatMessages(chatId);
            var userChatMessages = new List<Contracts.DTO.Message>();
            foreach (var message in userChatMessagesRaw)
            {
                userChatMessages.Add(new Contracts.DTO.Message
                {
                    Id = message.Id,
                    ChatId = message.ChatId,
                    SenderId = message.SenderId,
                    CreatedDate = message.CreatedDate,
                    Text = message.Text,
                });
            }
            return new GetChatMessagesResponse
            {
                Messages = userChatMessages
            };
        }
    }
}
