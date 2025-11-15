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

        [RelayCommand]
        async Task GoToGame()
        {
            await Shell.Current.GoToAsync(nameof(GamePage));
        }

        // --- ДОБАВЬТЕ ЭТУ КОМАНДУ ---
        [RelayCommand]
        async Task GoToLibrary()
        {
            // Эта навигация сработает, так как LibraryPage
            // уже зарегистрирована в AppShell.xaml.cs
            await Shell.Current.GoToAsync(nameof(LibraryPage));
        }
    }
}