// In Views/GamePage.xaml.cs
using AlchemyByKirill.ViewModels; // Добавь using

namespace AlchemyByKirill.Views;

public partial class GamePage : ContentPage
{
    // Внедряем ViewModel через конструктор
    public GamePage(GameViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel; // Устанавливаем ViewModel как контекст данных для XAML
    }
}