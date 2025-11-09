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
            // --- СПИСОК ВСЕХ ЭЛЕМЕНТОВ (54 шт.) ---
            _allElements = new List<Element>
            {
                // ID 1-4: Базовые элементы
                new Element(1, "Огонь", "fire.png", new Rect(50, 20, 75, 75)), // id1
                new Element(2, "Вода", "droplet.png", new Rect(150, 20, 75, 75)), // id2
                new Element(3, "Воздух", "wind_face.png", new Rect(50, 120, 75, 75)), // id3
                new Element(4, "Земля", "globe_showing_europe_africa.png", new Rect(150, 120, 75, 75)), // id4

                // --- ID 5-54: Создаваемые элементы ---
                
                // Геология и Атмосфера
                new Element(5, "Пар", "hot_springs.png", new Rect(0, 0, 75, 75)),
                new Element(6, "Лава", "volcano.png", new Rect(0, 0, 75, 75)),
                new Element(7, "Камень", "rock.png", new Rect(0, 0, 75, 75)),
                new Element(9, "Пыль", "dashing_away.png", new Rect(0, 0, 75, 75)),
                new Element(10, "Песок", "desert.png", new Rect(0, 0, 75, 75)),
                new Element(12, "Дождь", "cloud_with_rain.png", new Rect(0, 0, 75, 75)),
                new Element(13, "Металл", "nut_and_bolt.png", new Rect(0, 0, 75, 75)),
                new Element(14, "Глина", "pile_of_poo.png", new Rect(0, 0, 75, 75)), // (Иконка похожа)
                new Element(15, "Кирпич", "brick.png", new Rect(0, 0, 75, 75)),
                new Element(16, "Энергия", "high_voltage.png", new Rect(0, 0, 75, 75)),
                new Element(17, "Болото", "nauseated_face.png", new Rect(0, 0, 75, 75)), // (Иконка похожа)
                new Element(32, "Облако", "fog.png", new Rect(0, 0, 75, 75)),
                new Element(33, "Шторм", "cloud_with_lightning_and_rain.png", new Rect(0, 0, 75, 75)),
                new Element(54, "Волна", "water_wave.png", new Rect(0, 0, 75, 75)),
                
                // Биология
                new Element(8, "Растение", "seedling.png", new Rect(0, 0, 75, 75)),
                new Element(11, "Дерево", "evergreen_tree.png", new Rect(0, 0, 75, 75)),
                new Element(18, "Жизнь", "baby_angel.png", new Rect(0, 0, 75, 75)),
                new Element(19, "Птица", "bird.png", new Rect(0, 0, 75, 75)),
                new Element(20, "Яйцо", "egg.png", new Rect(0, 0, 75, 75)),
                new Element(28, "Человек", "person.png", new Rect(0, 0, 75, 75)),
                new Element(34, "Зомби", "zombie.png", new Rect(0, 0, 75, 75)),
                new Element(35, "Труп", "skull.png", new Rect(0, 0, 75, 75)),
                new Element(37, "Рыба", "fish.png", new Rect(0, 0, 75, 75)),
                new Element(39, "Зверь", "wolf.png", new Rect(0, 0, 75, 75)),
                new Element(48, "Ящерица", "lizard.png", new Rect(0, 0, 75, 75)),
                new Element(49, "Дракон", "dragon_face.png", new Rect(0, 0, 75, 75)),

                // Цивилизация и Техника
                new Element(21, "Дом", "house.png", new Rect(0, 0, 75, 75)),
                new Element(22, "Деревня", "houses.png", new Rect(0, 0, 75, 75)),
                new Element(23, "Город", "cityscape.png", new Rect(0, 0, 75, 75)),
                new Element(24, "Инструмент", "wrench.png", new Rect(0, 0, 75, 75)),
                new Element(25, "Древесина", "wood.png", new Rect(0, 0, 75, 75)),
                new Element(26, "Лодка", "sailboat.png", new Rect(0, 0, 75, 75)),
                new Element(27, "Корабль", "ship.png", new Rect(0, 0, 75, 75)),
                new Element(29, "Фермер", "man.png", new Rect(0, 0, 75, 75)),
                new Element(30, "Пшеница", "sheaf_of_rice.png", new Rect(0, 0, 75, 75)),
                new Element(31, "Хлеб", "bread.png", new Rect(0, 0, 75, 75)),
                new Element(38, "Воин", "axe.png", new Rect(0, 0, 75, 75)),
                new Element(40, "Доктор", "pill.png", new Rect(0, 0, 75, 75)),
                new Element(41, "Ученый", "microscope.png", new Rect(0, 0, 75, 75)),
                new Element(50, "Компьютер", "laptop.png", new Rect(0, 0, 75, 75)),
                new Element(51, "Робот", "robot.png", new Rect(0, 0, 75, 75)),
                new Element(52, "Локомотив", "locomotive.png", new Rect(0, 0, 75, 75)),
                new Element(53, "Инструменты", "hammer.png", new Rect(0, 0, 75, 75)),

                // Космос
                new Element(36, "Остров", "desert_island.png", new Rect(0, 0, 75, 75)),
                new Element(42, "Ночь", "night_with_stars.png", new Rect(0, 0, 75, 75)),
                new Element(43, "Луна", "full_moon.png", new Rect(0, 0, 75, 75)),
                new Element(44, "Звезды", "milky_way.png", new Rect(0, 0, 75, 75)),
                new Element(45, "Космос", "ringed_planet.png", new Rect(0, 0, 75, 75)),
                new Element(46, "НЛО", "flying_saucer.png", new Rect(0, 0, 75, 75)),
                new Element(47, "Пришелец", "alien.png", new Rect(0, 0, 75, 75)),
            };

            // --- СПИСОК РЕЦЕПТОВ (50 шт.) ---
            _allRecipes = new List<Recipe>
            {
                // Базовые + Геология
                new Recipe(1, 2, 5),    // Огонь + Вода = Пар
                new Recipe(1, 4, 6),    // Огонь + Земля = Лава
                new Recipe(2, 4, 17),   // Вода + Земля = Болото
                new Recipe(3, 4, 9),    // Воздух + Земля = Пыль
                new Recipe(1, 3, 16),   // Огонь + Воздух = Энергия
                new Recipe(6, 2, 7),    // Лава + Вода = Камень
                new Recipe(3, 7, 10),   // Воздух + Камень = Песок
                new Recipe(2, 9, 14),   // Вода + Пыль = Глина
                new Recipe(14, 1, 15),  // Глина + Огонь = Кирпич
                new Recipe(7, 1, 13),   // Камень + Огонь = Металл
                
                // Атмосфера
                new Recipe(5, 3, 32),   // Пар + Воздух = Облако
                new Recipe(32, 2, 12),  // Облако + Вода = Дождь
                new Recipe(2, 3, 54),   // Вода + Воздух = Волна
                new Recipe(54, 54, 33), // Волна + Волна = Шторм
                new Recipe(16, 32, 33), // Энергия + Облако = Шторм
                
                // Биология
                new Recipe(4, 12, 8),   // Земля + Дождь = Растение
                new Recipe(8, 4, 11),   // Растение + Земля = Дерево
                new Recipe(17, 16, 18), // Болото + Энергия = Жизнь
                new Recipe(18, 3, 19),  // Жизнь + Воздух = Птица
                new Recipe(18, 7, 20),  // Жизнь + Камень = Яйцо
                new Recipe(19, 19, 20), // Птица + Птица = Яйцо
                new Recipe(18, 4, 39),  // Жизнь + Земля = Зверь
                new Recipe(18, 17, 48), // Жизнь + Болото = Ящерица
                new Recipe(48, 1, 49),  // Ящерица + Огонь = Дракон
                new Recipe(18, 2, 37),  // Жизнь + Вода = Рыба
                
                // Цивилизация
                new Recipe(18, 14, 28), // Жизнь + Глина = Человек
                new Recipe(7, 13, 53),  // Камень + Металл = Инструменты
                new Recipe(28, 53, 38), // Человек + Инструменты = Воин
                new Recipe(28, 13, 24), // Человек + Металл = Инструмент (Ключ)
                new Recipe(11, 24, 25), // Дерево + Инструмент = Древесина
                new Recipe(15, 15, 21), // Кирпич + Кирпич = Дом
                new Recipe(21, 21, 22), // Дом + Дом = Деревня
                new Recipe(22, 22, 23), // Деревня + Деревня = Город
                new Recipe(28, 4, 29),  // Человек + Земля = Фермер
                new Recipe(29, 8, 30),  // Фермер + Растение = Пшеница
                new Recipe(30, 1, 31),  // Пшеница + Огонь = Хлеб
                new Recipe(25, 2, 26),  // Древесина + Вода = Лодка
                new Recipe(26, 13, 27), // Лодка + Металл = Корабль
                new Recipe(27, 5, 52),  // Корабль + Пар = Локомотив
                new Recipe(4, 7, 36),   // Земля + Камень = Остров (Логично, что лава+вода = камень, а не остров)
                                        // Давайте изменим: Лава(6) + Вода(2) = Остров(36)
                                        // А Камень(7) = Земля(4) + Огонь(1) -> Лава(6)
                                        // *Корректировка рецептов 6 и 39*
                // new Recipe(6, 2, 36),   // Лава + Вода = Остров (ID 36) (Заменяет рецепт 6)
                // new Recipe(4, 1, 6),    // Земля + Огонь = Лава (ID 6) (Уже есть как рецепт 2)
                // *ОК, оставим как было, просто добавим рецепт 39*
                new Recipe(6, 2, 36),   // Лава(6) + Вода(2) = Остров(36) (дополнительно к Камню)


                // Смерть и Наука
                new Recipe(28, 1, 35),  // Человек + Огонь = Труп
                new Recipe(35, 18, 34), // Труп + Жизнь = Зомби
                new Recipe(28, 8, 40),  // Человек + Растение = Доктор
                new Recipe(28, 24, 41), // Человек + Инструмент = Ученый
                new Recipe(41, 13, 50), // Ученый + Металл = Компьютер
                new Recipe(18, 13, 51), // Жизнь + Металл = Робот
                new Recipe(50, 51, 47), // Компьютер + Робот = Пришелец (Логика?)
                                        // *Давайте заменим 44 и 50*
                // new Recipe(50, 18, 51), // Компьютер + Жизнь = Робот (Заменяет 25)

                // Космос
                new Recipe(3, 7, 43),   // Воздух + Камень = Луна
                new Recipe(43, 3, 42),  // Луна + Воздух = Ночь
                new Recipe(42, 43, 44), // Ночь + Луна = Звезды
                new Recipe(44, 13, 45), // Звезды + Металл = Космос
                new Recipe(45, 27, 46), // Космос + Корабль = НЛО
                new Recipe(46, 18, 47), // НЛО + Жизнь = Пришелец
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