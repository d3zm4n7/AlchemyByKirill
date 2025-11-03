// Добавляем using
using CommunityToolkit.Mvvm.ComponentModel;

namespace AlchemyByKirill.Models;

// 1. Делаем класс 'partial' и наследуем от 'ObservableObject'
public partial class Element : ObservableObject
{
    // 2. Добавляем свойство Bounds. 
    //    Атрибут [ObservableProperty] автоматически создаст свойство 'Bounds'
    [ObservableProperty]
    private Rect _bounds; // Прямоугольник (X, Y, Width, Height)

    public int Id { get; set; }
    public string Name { get; set; }
    public string ImagePath { get; set; } // Путь к иконке, например "fire.svg"

    // 3. Обновляем конструктор, чтобы он принимал 'Bounds'
    public Element(int id, string name, string imagePath, Rect bounds)
    {
        Id = id;
        Name = name;
        ImagePath = imagePath;
        Bounds = bounds; // Устанавливаем свойство
    }
}