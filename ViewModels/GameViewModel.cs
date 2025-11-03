// In ViewModels/GameViewModel.cs

using AlchemyByKirill.Models;
using AlchemyByKirill.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using Element = AlchemyByKirill.Models.Element;

namespace AlchemyByKirill.ViewModels
{
    public partial class GameViewModel : ObservableObject
    {
        private readonly GameLogicService _gameLogicService;
        private Player _currentPlayer;
        private Random _random = new Random();

        public ObservableCollection<Element> DiscoveredElements { get; } = new ObservableCollection<Element>();
        public ObservableCollection<Element> GameBoardElements { get; } = new ObservableCollection<Element>();

        [ObservableProperty]
        private int _playerScore;

        [ObservableProperty]
        private Element? _draggedElement;

        // Команды, которые будут вызваны из XAML
        public ICommand ElementDragStartingCommand { get; }
        public ICommand GameBoardDropCommand { get; }
        // public ICommand ElementDroppedOnCommand { get; } // --- УДАЛЕНО ---
        public ICommand SpawnElementFromInventoryCommand { get; }
        public ICommand DuplicateElementCommand { get; }

        public GameViewModel()
        {
            _gameLogicService = new GameLogicService();
            _currentPlayer = new Player();
            LoadInitialGameState();

            // Инициализация всех команд
            ElementDragStartingCommand = new RelayCommand<Element>(ElementDragStarting);
            GameBoardDropCommand = new RelayCommand<DropEventArgs>(GameBoardDrop); // Этот метод теперь главный
            // ElementDroppedOnCommand = new RelayCommand<Element>(ElementDroppedOn); // --- УДАЛЕНО ---
            SpawnElementFromInventoryCommand = new RelayCommand<Element>(SpawnElementFromInventory);
            DuplicateElementCommand = new RelayCommand<Element>(DuplicateElement);
        }

        private void LoadInitialGameState()
        {
            DiscoveredElements.Clear();
            GameBoardElements.Clear();
            _currentPlayer.Reset();

            var baseElements = _gameLogicService.GetBaseElements();
            foreach (var element in baseElements)
            {
                if (_currentPlayer.DiscoverElement(element.Id))
                {
                    DiscoveredElements.Add(element);
                }
            }
            PlayerScore = _currentPlayer.Score;
        }

        // Вызывается, когда мы НАЧИНАЕМ тащить элемент
        private void ElementDragStarting(Element? element)
        {
            if (element == null) return;
            DraggedElement = element;
            Debug.WriteLine($"Dragging: {element.Name}");
        }

        // --- ЭТОТ МЕТОД ТЕПЕРЬ ОБРАБАТЫВАЕТ И ПЕРЕМЕЩЕНИЕ, И КОМБИНАЦИЮ ---
        private void GameBoardDrop(DropEventArgs? e)
        {
            if (DraggedElement == null || e == null) return;

            var position = e.GetPosition(null); // Это 'Point?'
            if (!position.HasValue)
            {
                DraggedElement = null;
                return;
            }

            // 1. Проверяем, попали ли мы на другой элемент
            // Ищем элемент, который не является перетаскиваемым, но его Bounds (рамки) содержит точку броска
            var targetElement = GameBoardElements.FirstOrDefault(el =>
                el != DraggedElement &&
                el.Bounds.Contains(position.Value));

            if (targetElement != null)
            {
                // 2. ЕСЛИ ПОПАЛИ: Это комбинация
                Debug.WriteLine($"Dropped {DraggedElement.Name} onto {targetElement.Name}");
                TryCombineElements(DraggedElement, targetElement);
            }
            else
            {
                // 3. ЕСЛИ НЕ ПОПАЛИ: Это перемещение
                // Центрируем элемент по курсору (ширина/высота 75, значит смещение 37.5)
                double x = Math.Max(0, position.Value.X - 37.5);
                double y = Math.Max(0, position.Value.Y - 37.5);

                DraggedElement.Bounds = new Rect(x, y, DraggedElement.Bounds.Width, DraggedElement.Bounds.Height);
                Debug.WriteLine($"Moved {DraggedElement.Name} to {DraggedElement.Bounds}");
            }

            DraggedElement = null; // Завершаем перетаскивание в любом случае
        }


        // --- ЭТОТ МЕТОД БОЛЬШЕ НЕ НУЖЕН (логика переехала в GameBoardDrop) ---
        // private void ElementDroppedOn(Element? targetElement)
        // {
        //    ...
        // }
        // -----------------------------------------------------------------

        // --- ДОБАВЛЕНО: Логика для спавна из инвентаря ---
        private void SpawnElementFromInventory(Element? element)
        {
            if (element == null) return;

            // Спавн в случайной позиции в верхней части экрана
            double x = _random.Next(50, 250);
            double y = _random.Next(50, 200);

            var newElement = new Element(element.Id, element.Name, element.ImagePath, new Rect(x, y, 75, 75));
            GameBoardElements.Add(newElement);
            Debug.WriteLine($"Spawned {newElement.Name} from inventory at {x},{y}");
        }

        // --- ДОБАВЛЕНО: Логика для дублирования ---
        private void DuplicateElement(Element? element)
        {
            if (element == null) return;

            // Создаем копию элемента немного со смещением
            var newRect = new Rect(element.Bounds.X + 20, element.Bounds.Y + 20, element.Bounds.Width, element.Bounds.Height);
            var newElement = new Element(element.Id, element.Name, element.ImagePath, newRect);

            GameBoardElements.Add(newElement);
            Debug.WriteLine($"Duplicated {newElement.Name} to {newRect}");
        }
        // ------------------------------------------

        private async void TryCombineElements(Element element1, Element element2)
        {
            Debug.WriteLine($"Попытка комбинации: {element1.Name} + {element2.Name}");
            Element? result = _gameLogicService.Combine(element1, element2);

            if (result != null)
            {
                Debug.WriteLine($"Успех! Получен: {result.Name}");

                // Удаляем старые элементы с поля
                GameBoardElements.Remove(element1);
                GameBoardElements.Remove(element2);

                // Ставим новый элемент на место 'цели' (element2)
                result.Bounds = new Rect(element2.Bounds.X, element2.Bounds.Y, element2.Bounds.Width, element2.Bounds.Height);
                GameBoardElements.Add(result);

                bool isNew = _currentPlayer.DiscoverElement(result.Id);

                if (isNew)
                {
                    Debug.WriteLine("Это новый элемент!");
                    if (!DiscoveredElements.Any(e => e.Id == result.Id))
                        DiscoveredElements.Add(result);

                    int scoreGained = _gameLogicService.CalculateScoreForDiscovery(result);
                    _currentPlayer.AddScore(scoreGained);
                    PlayerScore = _currentPlayer.Score;

                    await Shell.Current.DisplayAlert("Новый элемент!", $"Вы открыли: {result.Name}!", "OK");
                }
                else
                {
                    Debug.WriteLine("Элемент уже был открыт.");
                }
            }
            else
            {
                Debug.WriteLine("Неудачная комбинация.");
            }
        }

        [RelayCommand]
        void ClearBoard()
        {
            GameBoardElements.Clear();
        }

        [RelayCommand]
        async Task Exit()
        {
            await Shell.Current.GoToAsync("..");
        }

    }
}