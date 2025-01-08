using Cat.Client.Care;
using Cat.Client.DataBase;
using Cat.Client.Infrastructure.Handlers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cat.Client.Infrastructure.Services
{
    public class ChatService
    {
        private readonly CatClientDbContext _dbContext;

        private readonly UrlHandler _urlHandler;

        private readonly ILogger<ChatService> _logger;

        public ChatService(CatClientDbContext dbContext, UrlHandler urlHandler, ILogger<ChatService> logger)
        {
            _dbContext = dbContext;
            _urlHandler = urlHandler;
            _logger = logger;
        }

        // Добавление нового чата с url
        public async Task<bool> AddChatAsync(string name, string url)
        {

            try
            {
                // Обработка url.
                url = _urlHandler.PrepareUrlForSaving(url);

                // Проверка url на наличие после обработки.
                if (string.IsNullOrEmpty(url))
                {
                    _logger.LogError("URL is invalid or empty after preparation.");
                    return false;
                }

                //  Поиск чатов, которые имеют такое же имя как у чата, который мы добавляем.
                var listChats = _dbContext.ChatServers
                                            .Where(c => c.ChatName == name)
                                            .Select(c => c.AdditionalId)
                                            .ToList();
                var chat = new ChatServer
                {
                    ChatName = name,
                    // Дополнительный id для того что бы различать чаты с одинаквым именем.
                    AdditionalId = listChats.DefaultIfEmpty(0).Max() + 1,
                    Url = url
                };

                _dbContext.ChatServers.Add(chat);

                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Chat with URL {Url} added successfully.", url);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the chat. URL: {Url}", url);
                return false;
            }
        }

        public async Task<bool> UpdateChatAsync(string name, int subId, string url)
        {
            try
            {
                // Обработка url.
                url = _urlHandler.PrepareUrlForSaving(url);

                // Проверка url на наличие после обработки.
                if (string.IsNullOrEmpty(url))
                {
                    _logger.LogError("URL is invalid or empty after preparation.");
                    return false;
                }

                //  Поиск чата для обновления
                var chat = _dbContext.ChatServers
                                            .FirstOrDefault(c => c.ChatName == name && c.AdditionalId == subId);

                if (chat == null)
                {
                    _logger.LogError("Chat with name {Name} and subId {SubId} not found.", name, subId);
                    return false;
                }

                // Обновление url чата
                chat.Url = url;

                // Сохренение изменений в базе данных

                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Chat with name {Name} and subId {SubId} updated successfully. New URL: {Url}", name, subId, url);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the chat. Name: {Name}, SubId: {SubId}, URL: {Url}", name, subId, url);

                return false;
            }
        }

        public async Task<bool> DeleteChatAsync(string name, int subId)
        {
            try
            {
                // Поиск чата для удаления
                var chat = _dbContext.ChatServers
                                        .FirstOrDefault(c => c.ChatName == name && c.AdditionalId == subId);
                if (chat == null)
                {
                    _logger.LogError("Chat with name {Name} and subId {SubId} not found.", name, subId);

                    return false;
                }

                // Удаление чата из контекста базы данных
                _dbContext.ChatServers.Remove(chat);

                // Обновление subId у всех остальных чатов с тем же нменем

                var chatsToUppdate = _dbContext.ChatServers
                                               .Where(c => c.ChatName == name && c.AdditionalId > subId)
                                               .OrderBy(c => c.AdditionalId)
                                               .ToList();

                // Если спосок не поустой то обновлеям
                if (chatsToUppdate.Any())
                {
                    foreach (var c in chatsToUppdate)
                    {
                        c.AdditionalId -= 1;
                    }
                }

                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Chat with name {Name} and subId {SubId} deleted successfully. SubIds updated.", name, subId);

                return true;
            } catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the chat. Name: {Name}, SubId: {SubId}", name, subId);

                return false;
            }
        }

        public async Task<List<ChatServer>> GetChats()
        {
            return await _dbContext.ChatServers.ToListAsync();
        }
    }
}
