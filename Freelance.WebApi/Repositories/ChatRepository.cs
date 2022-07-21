using Dapper;
using Freelance.Repositories.Interfaces;
using Freelance.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using Freelance.WebApi.Contracts.Entities;

namespace Freelance.Repositories
{
    public class ChatRepository : IChatRepository
    {

        private readonly IDapperService _dapper;

        public ChatRepository(IDapperService dapper)
        {
            _dapper = dapper;
        }

        public async Task<Guid> CreateChat(Guid userId)
        {
            var query = @"INSERT INTO chat.chat (created_user_id, created_date) 
                          VALUES (@created_user_id, @createdDate) RETURNING id";

            var parameters = new DynamicParameters();
            parameters.Add("@created_user_id", userId);
            parameters.Add("@createdDate", DateTime.UtcNow);

            return await _dapper.QuerySingle<Guid, DynamicParameters>(query, parameters);
        }

        public async Task CreateParty(List<Party> party)
        {
            var query = @"INSERT INTO chat.party (chat_id, user_id) 
                          VALUES (@ChatId, @UserId)";

            await _dapper.Execute(query, party);
        }

        public async Task CreateMessage(Guid chatId, string text)
        {
            var query = @"INSERT INTO chat.message (chat_id, created_date, text) 
                          VALUES (@chatId, @createdDate, @text)";

            var parameters = new DynamicParameters();
            parameters.Add("@chatId", chatId, DbType.Guid);
            parameters.Add("@createdDate", DateTime.UtcNow, DbType.DateTime);
            parameters.Add("@text", text);

            await _dapper.Execute(query, parameters);
        }

        // TODO: Create Attachment Repository
        public async Task<Guid> CreateAttachment(string filepath)
        {
            var query = @"INSERT INTO chat.attachment (filepath, created_date) 
                          VALUES (@filepath, @createdDate) RETURNING id";

            var parameters = new DynamicParameters();
            parameters.Add("filepath", filepath);
            parameters.Add("createdDate", DateTime.UtcNow, DbType.DateTime);

            return await _dapper.QuerySingle<Guid, DynamicParameters>(query, parameters);
        }

        // TODO: GetChatsByUserId - приселектить последнее сообщение
        public async Task<IEnumerable<Chat>> GetChatsByUserId(Guid userId)
        {
            var query = @"SELECT c.id
	                          ,c.created_date
	                          ,c.created_user_id
	                          ,u.id
	                          ,u.email
	                          ,u.login
	                          ,m.text as last_message
                              ,m.created_date as last_message_date
                          FROM chat.party p
                          JOIN chat.chat c ON c.id = p.chat_id
                          JOIN public.user u ON u.id = p.user_id
                          JOIN chat.message m ON c.id = m.chat_id
                          WHERE c.id in (
	                          SELECT chat_id 
	                          FROM chat.party 
	                          WHERE user_id = @userId
                          )
                          AND p.user_id != @userId
                          AND m.created_date = (
	                          SELECT MAX(created_date)
	                          FROM chat.message
	                          WHERE chat_id = c.id
                          )";

            var parameters = new DynamicParameters();
            parameters.Add("@userId", userId, DbType.Guid);

            return await _dapper.Query<Chat, User, Chat>(query, (c, u) =>
            {
                c.User = u;
                return c;
            }, parameters);
        }

        public async Task<IEnumerable<Message>> GetChatMessages(Guid chatId)
        {
            var query = @"SELECT id, chat_id, sender_id, text, created_date
                          FROM chat.message
                          WHERE chat_id = @chatId";

            var parameters = new DynamicParameters();
            parameters.Add("@chatId", chatId, DbType.Guid);

            return await _dapper.Query<Message>(query, parameters);
        }
    }
}
