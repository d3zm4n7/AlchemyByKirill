// In Models/Recipe.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlchemyByKirill.Models
{
    internal class Recipe
    {
        // ID первого элемента-ингредиента
        public int Ingredient1Id { get; set; }

        // ID второго элемента-ингредиента
        public int Ingredient2Id { get; set; }

        // ID элемента, получаемого в результате комбинации
        public int ResultElementId { get; set; }

        // Конструктор для удобного создания рецепта
        public Recipe(int ingredient1Id, int ingredient2Id, int resultElementId)
        {
            // Сразу упорядочиваем ID, чтобы не зависеть от порядка
            // передачи ингредиентов при проверке.md]
            if (ingredient1Id < ingredient2Id)
            {
                Ingredient1Id = ingredient1Id;
                Ingredient2Id = ingredient2Id;
            }
            else
            {
                Ingredient1Id = ingredient2Id;
                Ingredient2Id = ingredient1Id;
            }
            ResultElementId = resultElementId;
        }

        /// <summary>
        /// Проверяет, соответствуют ли два элемента этому рецепту.
        /// Учитывает, что порядок элементов не важен.
        /// </summary>
        public bool Matches(Element element1, Element element2)
        {
            return (element1.Id == Ingredient1Id && element2.Id == Ingredient2Id) ||
                   (element1.Id == Ingredient2Id && element2.Id == Ingredient1Id);
        }

        /// <summary>
        /// Проверяет, соответствуют ли ID двух элементов этому рецепту.
        /// Учитывает, что порядок ID не важен.
        /// </summary>
        public bool Matches(int element1Id, int element2Id)
        {
            // ID в конструкторе уже упорядочены, поэтому сравниваем напрямую
            if (element1Id < element2Id)
            {
                return element1Id == Ingredient1Id && element2Id == Ingredient2Id;
            }
            else
            {
                return element2Id == Ingredient1Id && element1Id == Ingredient2Id;
            }
        }
    }
}