// In Views/GamePage.xaml.cs
using AlchemyByKirill.ViewModels; // ������ using

namespace AlchemyByKirill.Views;

public partial class GamePage : ContentPage
{
    // �������� ViewModel ����� �����������
    public GamePage(GameViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel; // ������������� ViewModel ��� �������� ������ ��� XAML
    }
}