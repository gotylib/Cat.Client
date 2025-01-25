using Serilog;
using Cat.Client.DataBase;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Cat.Client.Infrastructure.Services;
using Cat.Client.Infrastructure.Handlers;
using MudBlazor.Services;



namespace Cat.Client
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddTransient<ChatService>();
            builder.Services.AddTransient<UrlHandler>();

            builder.Services.AddMauiBlazorWebView();

            builder.Services.AddDbContext<CatClientDbContext>();

            builder.Services.AddHttpClient();

            builder.Services.AddMudServices();

            var dbContext = new CatClientDbContext();
            dbContext.Database.EnsureCreated();

            builder.Services.AddBlazorContextMenu();


#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();

            //Настройка логирования в файл
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "logs/log.txt"), rollingInterval: RollingInterval.Day)
                .CreateLogger();

            builder.Logging.AddSerilog();


#endif

            return builder.Build();
        }
    }
}
