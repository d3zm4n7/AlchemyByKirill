using AlchemyByKirill.ViewModels;
using Microsoft.Maui.Controls;
using System;
using AlchemyByKirill.Views;


namespace AlchemyByKirill.Views;

public partial class LibraryPage : ContentPage
{
    public LibraryPage(LibraryViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
