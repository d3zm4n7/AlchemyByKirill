using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input; // Добавь это
using AlchemyByKirill.Views; // Убедись, что это добавлено

namespace AlchemyByKirill.ViewModels
{
    public partial class StartViewModel : ObservableObject
    {
        public StartViewModel()
        {
        }

        // Команда для перехода на страницу игры
        [RelayCommand]
        async Task GoToGame()
        {
            await Shell.Current.GoToAsync(nameof(GamePage));
        }
    }
}