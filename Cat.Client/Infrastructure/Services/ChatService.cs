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

        //Добавление нового чата с url
        public async Task<bool> AddChatAsync(string name, string url)
        {

            try
            {
                //Обработка url.
                url = _urlHandler.PrepareUrlForSaving(url);

                //Проверка url на наличие после обработки.
                if (string.IsNullOrEmpty(url))
                {
                    _logger.LogError("URL is invalid or empty after preparation.");
                    return false;
                }

                // Поиск чатов, которые имеют такое же имя как у чата, который мы добавляем.
                var listChats = _dbContext.ChatServers
                                            .Where(c => c.ChatName == name)
                                            .Select(c => c.AdditionalId)
                                            .ToList();
                var chat = new ChatServer
                {
                    ChatName = name,
                    //Дополнительный id для того что бы различать чаты с одинаквым именем.
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

        public async Task<List<ChatServer>> GetChats()
        {
            return await _dbContext.ChatServers.ToListAsync();
        }
    }
}
