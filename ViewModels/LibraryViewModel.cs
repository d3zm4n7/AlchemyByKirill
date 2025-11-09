using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using AlchemyByKirill.Models;
using Element = AlchemyByKirill.Models.Element;

namespace AlchemyByKirill.ViewModels;

public partial class LibraryViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Element> elements = new();

    public void Load(ObservableCollection<Element> discovered)
    {
        Elements = discovered;
    }
}