using Ganss.Xss;
using Microsoft.Extensions.Logging;

namespace Cat.Client.Infrastructure.Handlers
{
    public class UrlHandler
    {
        private readonly ILogger<UrlHandler> _logger;

        public UrlHandler(ILogger<UrlHandler> logger)
        {
            _logger = logger;
        }



        //Валидация url
        public bool IsValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out _);
        }

        //Нормализация url
        public string NormalizeUrl(string url)
        {
            if(Uri.TryCreate(url, UriKind.Absolute, out var uri))
            {
                return uri.AbsoluteUri;
            }
            return url;
        }

        //Проверка url
        public string SanitizeUrl(string url)
        {
            var sanitizer = new HtmlSanitizer();
            return sanitizer.Sanitize(url);
        }

        //Подготовка url к добавлению 
        public string PrepareUrlForSaving(string url)
        {
            if (!IsValidUrl(url))
            {
                _logger.LogError("Incalid URL: {Url}",url);
            }
            url = NormalizeUrl(url);
            url = SanitizeUrl(url);

            return url;
        }
    }
}
