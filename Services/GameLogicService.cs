// In Services/GameLogicService.cs
using AlchemyByKirill.Models; // Добавляем using для доступа к моделям
using Element = AlchemyByKirill.Models.Element;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlchemyByKirill.Services
{
    internal class GameLogicService
    {
        // Список всех доступных в игре элементов (пока что жестко закодирован)
        private List<Element> _allElements = new List<Element>();
        // Список всех рецептов (пока что жестко закодирован)
        private List<Recipe> _allRecipes = new List<Recipe>();

        public GameLogicService()
        {
            LoadInitialData(); // Загружаем элементы и рецепты при создании сервиса
        }

        /// <summary>
        /// Инициализация базовых элементов и рецептов (заглушка).
        /// Позже это будет загружаться из JSON..md]
        /// </summary>
        private void LoadInitialData()
        {
            _allElements = new List<Element>
            {
                new Element(1, "Огонь", "fire.png"),       // 🔥
                new Element(2, "Вода", "droplet.png"),      // 💧
                new Element(3, "Воздух", "wind_face.png"),   // 🌬️
                new Element(4, "Земля", "globe_showing_europe-africa.png"), // 🌍
                new Element(5, "Пар", "fog.png"),          // 🌫️ (результат)
                new Element(6, "Лава", "volcano.png"),       // 🌋 (результат)
                new Element(7, "Камень", "rock.png"),       // 🪨 (результат)
                new Element(8, "Растение", "seedling.png"), // 🌱 (результат)
                // Добавь сюда остальные элементы по мере необходимости
            };

            _allRecipes = new List<Recipe>
            {
                new Recipe(1, 2, 5), // Огонь + Вода = Пар.md]
                new Recipe(1, 4, 6), // Огонь + Земля = Лава.md] (примерно)
                new Recipe(6, 2, 7), // Лава + Вода = Камень (упрощенно).md] (примерно)
                new Recipe(2, 4, 8)  // Вода + Земля = Растение.md] (примерно)
                // Добавь сюда остальные 5 рецептов из твоего плана
            };
        }

        /// <summary>
        /// Получить элемент по его ID.
        /// </summary>
        public Element? GetElementById(int id)
        {
            return _allElements.FirstOrDefault(e => e.Id == id);
        }

        /// <summary>
        /// Получить все базовые элементы (Огонь, Вода, Воздух, Земля).
        /// </summary>
        public List<Element> GetBaseElements()
        {
            // Предположим, что базовые элементы имеют ID с 1 по 4
            return _allElements.Where(e => e.Id >= 1 && e.Id <= 4).ToList();
        }


        /// <summary>
        /// Пытается скомбинировать два элемента.
        /// Возвращает результирующий элемент, если комбинация успешна, иначе null.
        ///
        /// </summary>
        public Element? Combine(Element element1, Element element2)
        {
            if (element1 == null || element2 == null)
                return null;

            // Ищем подходящий рецепт
            Recipe? foundRecipe = _allRecipes.FirstOrDefault(r => r.Matches(element1, element2));

            if (foundRecipe != null)
            {
                // Находим результирующий элемент по ID из рецепта
                return GetElementById(foundRecipe.ResultElementId);
            }

            return null; // Рецепт не найден
        }

        // --- Методы для очков (пока простые) ---

        /// <summary>
        /// Рассчитывает очки за открытие нового элемента.
        /// </summary>
        public int CalculateScoreForDiscovery(Element discoveredElement)
        {
            // Простая логика: +1 очко за каждый новый элемент
            // Позже можно усложнить (зависимость от редкости и т.д..md])
            return 1;
        }
    }
}