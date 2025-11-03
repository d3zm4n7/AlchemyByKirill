// In Services/GameLogicService.cs

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
            _allElements = new List<Element>
            {
                // ID 1-4: Базовые элементы
                new Element(1, "Огонь", "fire.png", new Rect(50, 20, 75, 75)), // id1
                new Element(2, "Вода", "droplet.png", new Rect(150, 20, 75, 75)), // id2
                new Element(3, "Воздух", "wind_face.png", new Rect(50, 120, 75, 75)), // id3
                new Element(4, "Земля", "globe_showing_europe_africa.png", new Rect(150, 120, 75, 75)), // id4

                // ID 5-9: Результаты комбинаций
                new Element(5, "Пар", "fog.png", new Rect(0, 0, 75, 75)), // id5
                new Element(6, "Лава", "volcano.png", new Rect(0, 0, 75, 75)), // id6
                new Element(7, "Камень", "rock.png", new Rect(0, 0, 75, 75)), // id7
                new Element(8, "Растение", "seedling.png", new Rect(0, 0, 75, 75)), // id8
                new Element(9, "Пыль", "fog.png", new Rect(0, 0, 75, 75)), // id9
            };

            _allRecipes = new List<Recipe>
            {
                new Recipe(1, 2, 5), // id1(Огонь) + id2(Вода) = id5(Пар)
                new Recipe(1, 4, 6), // id1(Огонь) + id4(Земля) = id6(Лава)
                new Recipe(2, 4, 8), // id2(Вода) + id4(Земля) = id8(Растение)
                new Recipe(3, 4, 9),  // id3(Воздух) + id4(Земля) = id9(Пыль)
                new Recipe(6, 2, 7), // id6(Лава) + id2(Вода) = id7(Камень)
            };
        }

        public Element? GetElementById(int id)
        {
            return _allElements.FirstOrDefault(e => e.Id == id);
        }

        public List<Element> GetBaseElements()
        {
            var baseElements = _allElements.Where(e => e.Id >= 1 && e.Id <= 4).ToList();
            return baseElements.Select(e => new Element(e.Id, e.Name, e.ImagePath, e.Bounds)).ToList();
        }

        public Element? Combine(Element element1, Element element2)
        {
            if (element1 == null || element2 == null)
                return null;

            // Класс Recipe внутри себя упорядочивает ID для поиска, 
            // поэтому порядок element1 и element2 не имеет значения.
            Recipe? foundRecipe = _allRecipes.FirstOrDefault(r => r.Matches(element1, element2));

            if (foundRecipe != null)
            {
                var resultPrototype = GetElementById(foundRecipe.ResultElementId);
                if (resultPrototype != null)
                {
                    // Возвращаем НОВЫЙ экземпляр Element с новым InstanceId
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