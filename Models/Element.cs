// In Models/Element.cs
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Graphics;

namespace AlchemyByKirill.Models;

public partial class Element : ObservableObject
{
    [ObservableProperty]
    private Rect _bounds;

    public int Id { get; set; }
    public string Name { get; set; }
    public string ImagePath { get; set; }
    public Guid InstanceId { get; private set; } // Уникальный ID для каждой копии элемента

    // Главный конструктор, который генерирует новый InstanceId
    public Element(int id, string name, string imagePath, Rect bounds)
    {
        Id = id;
        Name = name;
        ImagePath = imagePath;
        _bounds = bounds;
        InstanceId = Guid.NewGuid(); // Генерируем уникальный ID
    }

    public Element(int id, string name, string imagePath)
    {
        Id = id;
        Name = name;
        ImagePath = imagePath;
        _bounds = new Rect(0, 0, 75, 75);
        InstanceId = Guid.NewGuid();
    }
}