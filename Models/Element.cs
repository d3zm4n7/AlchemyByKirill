using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlchemyByKirill.Models;

public class Element
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string ImagePath { get; set; } // Путь к иконке, например "fire.svg"

    public Element(int id, string name, string imagePath)
    {
        Id = id;
        Name = name;
        ImagePath = imagePath;
    }
}
