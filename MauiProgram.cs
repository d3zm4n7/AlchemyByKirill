// In MauiProgram.cs
using Microsoft.Extensions.Logging;
using AlchemyByKirill.ViewModels; // Добавь using для ViewModel
using AlchemyByKirill.Views;    // Добавь using для Views

namespace AlchemyByKirill
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
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
                

#if DEBUG
            builder.Logging.AddDebug();
#endif
            // --- Регистрация ViewModel и Страницы ---
            builder.Services.AddSingleton<GameViewModel>(); // Регистрируем ViewModel как Singleton (один экземпляр на все приложение)
            builder.Services.AddSingleton<GamePage>();      // Регистрируем страницу GamePage
            builder.Services.AddSingleton<LibraryViewModel>();
            builder.Services.AddSingleton<LibraryPage>();

            // Позже добавим StartViewModel и StartPage
            builder.Services.AddSingleton<StartViewModel>();
            builder.Services.AddTransient<StartPage>(); // Страницы обычно Transient

            return builder.Build();
        }
    }
}