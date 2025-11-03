// ДОБАВИТЬ ЭТОТ USING
using CommunityToolkit.Mvvm.ComponentModel;

namespace AlchemyByKirill.Models;

// ИЗМЕНИТЬ: класс теперь 'partial' и наследует от 'ObservableObject'
public partial class Element : ObservableObject
{
    // ДОБАВИТЬ: Свойство для хранения позиции и размера (X, Y, Width, Height)
    [ObservableProperty]
    private Rect _bounds;

    public int Id { get; set; }
    public string Name { get; set; }
    public string ImagePath { get; set; }

    // ДОБАВИТЬ: Новый конструктор, который принимает 'Bounds'
    public Element(int id, string name, string imagePath, Rect bounds)
    {
        Id = id;
        Name = name;
        ImagePath = imagePath;
        _bounds = bounds; // Назначаем приватное поле
    }

    // Этот старый конструктор можно удалить, но мы его оставим и обновим
    public Element(int id, string name, string imagePath)
    {
        Id = id;
        Name = name;
        ImagePath = imagePath;
        _bounds = new Rect(0, 0, 75, 75); // Задаем размер по умолчанию
    }
}