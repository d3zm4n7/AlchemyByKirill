using AlchemyByKirill.Models;
using Element = AlchemyByKirill.Models.Element;
using System.Collections.Generic;
using System.Linq;

namespace AlchemyByKirill.Services
{
    internal class GameLogicService
    {
        private List<Element> _allElements = new List<Element>();
        private List<Recipe> _allRecipes = new List<Recipe>();

        public GameLogicService()
        {
            LoadInitialData();
        }

        private void LoadInitialData()
        {
            // ИЗМЕНИТЬ: Обновляем вызовы конструктора, добавляя Rect(X, Y, Width, Height)
            _allElements = new List<Element>
            {
                // Задаем стартовые позиции для 4 базовых элементов
                new Element(1, "Огонь", "fire.png", new Rect(50, 20, 75, 75)),
                new Element(2, "Вода", "droplet.png", new Rect(150, 20, 75, 75)),
                new Element(3, "Воздух", "wind_face.png", new Rect(50, 120, 75, 75)),
                new Element(4, "Земля", "globe_showing_europe_africa.png", new Rect(150, 120, 75, 75)),

                // Результаты комбинаций (координаты не важны, но размер 75x75 нужен)
                new Element(5, "Пар", "fog.png", new Rect(0, 0, 75, 75)),
                new Element(6, "Лава", "volcano.png", new Rect(0, 0, 75, 75)),
                new Element(7, "Камень", "rock.png", new Rect(0, 0, 75, 75)),
                new Element(8, "Растение", "seedling.png", new Rect(0, 0, 75, 75)),
            };

            _allRecipes = new List<Recipe>
            {
                new Recipe(1, 2, 5), // Огонь + Вода = Пар
                new Recipe(1, 4, 6), // Огонь + Земля = Лава
                new Recipe(6, 2, 7), // Лава + Вода = Камень
                new Recipe(2, 4, 8)  // Вода + Земля = Растение
            };
        }

        public Element? GetElementById(int id)
        {
            return _allElements.FirstOrDefault(e => e.Id == id);
        }

        public List<Element> GetBaseElements()
        {
            // Возвращаем копии, чтобы у каждого элемента на поле были свои Bounds
            var baseElements = _allElements.Where(e => e.Id >= 1 && e.Id <= 4).ToList();
            return baseElements.Select(e => new Element(e.Id, e.Name, e.ImagePath, e.Bounds)).ToList();
        }

        public Element? Combine(Element element1, Element element2)
        {
            if (element1 == null || element2 == null)
                return null;

            Recipe? foundRecipe = _allRecipes.FirstOrDefault(r => r.Matches(element1, element2));

            if (foundRecipe != null)
            {
                // Возвращаем *копию* результата, а не элемент из _allElements
                var resultPrototype = GetElementById(foundRecipe.ResultElementId);
                if (resultPrototype != null)
                {
                    return new Element(resultPrototype.Id, resultPrototype.Name, resultPrototype.ImagePath, resultPrototype.Bounds);
                }
            }

            return null; // Рецепт не найден
        }

        public int CalculateScoreForDiscovery(Element discoveredElement)
        {
            return 1;
        }
    }
}