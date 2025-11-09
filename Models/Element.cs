using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Graphics;
using System.Text.Json.Serialization;

namespace AlchemyByKirill.Models;

public partial class Element : ObservableObject
{
    // СТАНДАРТНЫЕ ДАННЫЕ
    public int Id { get; set; }
    public string Name { get; set; }
    public string ImagePath { get; set; }

    // УНИКАЛЬНЫЙ ID ЭКЗЕМПЛЯРА (не сохраняем!)
    [JsonIgnore]
    public Guid InstanceId { get; private set; } = Guid.NewGuid();

    // ---- СЕРИАЛИЗУЕМЫЕ ПОЛЯ КООРДИНАТ ----
    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; } = 75;
    public double Height { get; set; } = 75;

    // ---- НЕ сериализуемое свойство Bounds ----
    [JsonIgnore]
    public Rect Bounds
    {
        get => new Rect(X, Y, Width, Height);
        set
        {
            X = value.X;
            Y = value.Y;
            Width = value.Width;
            Height = value.Height;
            OnPropertyChanged();
        }
    }

    // Конструктор ПОСЛЕ загрузки сохранения
    [JsonConstructor]
    public Element(int id, string name, string imagePath, double x, double y, double width, double height)
    {
        Id = id;
        Name = name;
        ImagePath = imagePath;
        X = x;
        Y = y;
        Width = width;
        Height = height;
        InstanceId = Guid.NewGuid(); // ID генерируем заново — это нормально
    }

    public Element(int id, string name, string imagePath, Rect bounds)
    {
        Id = id;
        Name = name;
        ImagePath = imagePath;
        X = bounds.X;
        Y = bounds.Y;
        Width = bounds.Width;
        Height = bounds.Height;
        InstanceId = Guid.NewGuid();
    }

    public Element(int id, string name, string imagePath)
    {
        Id = id;
        Name = name;
        ImagePath = imagePath;
        InstanceId = Guid.NewGuid();
    }
}
