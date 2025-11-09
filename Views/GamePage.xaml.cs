// In Views/GamePage.xaml.cs
using AlchemyByKirill.Services;
using AlchemyByKirill.ViewModels; // Добавь using
using ElementModel = AlchemyByKirill.Models.Element;

namespace AlchemyByKirill.Views;

public partial class GamePage : ContentPage
{
    private GameViewModel VM => (GameViewModel)BindingContext;

    public GamePage(GameViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;

        vm.ShowMessage = async (msg) =>
        {
            await DisplayAlert("Алхимия", msg, "OK");
        };
    }

    private void OnDragStarting(object sender, DragStartingEventArgs e)
    {
        if (sender is BindableObject bo && bo.BindingContext is Element element)
            VM.ElementDragStartingCommand.Execute(element);
    }

    private void OnDrop(object sender, DropEventArgs e)
    {
        var position = e.GetPosition(GameBoardLayout);
        if (position.HasValue)
            VM.DropAt(position.Value);
    }

    private void OnBoardDoubleTapped(object sender, TappedEventArgs e)
    {
        if (sender is BindableObject bo && bo.BindingContext is Element element)
            VM.DuplicateElementCommand.Execute(element);
    }

    private void OnInventoryDoubleTapped(object sender, TappedEventArgs e)
    {
        if (sender is BindableObject bo && bo.BindingContext is Element element)
            VM.SpawnElementFromInventoryCommand.Execute(element);
    }


}







