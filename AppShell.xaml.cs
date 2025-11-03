using AlchemyByKirill.Views; // Убедись, что это добавлено

namespace AlchemyByKirill;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Регистрируем пути для навигации
        Routing.RegisterRoute(nameof(StartPage), typeof(StartPage));
        Routing.RegisterRoute(nameof(GamePage), typeof(GamePage));
    }
}