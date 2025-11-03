// In ViewModels/GameViewModel.cs

using AlchemyByKirill.Models;
using AlchemyByKirill.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using Element = AlchemyByKirill.Models.Element;
using Microsoft.Maui.Controls;

namespace AlchemyByKirill.ViewModels
{
    public partial class GameViewModel : ObservableObject
    {
        private readonly GameLogicService _gameLogicService;
        private Player _currentPlayer;
        private Random _random = new Random();

        // --- ВОЗВРАЩАЕМ ЭТО СВОЙСТВО ДЛЯ КЭШИРОВАНИЯ ОБЪЕКТА ---
        [ObservableProperty]
        private Element? _draggedElement;

        public ObservableCollection<Element> DiscoveredElements { get; } = new ObservableCollection<Element>();
        public ObservableCollection<Element> GameBoardElements { get; } = new ObservableCollection<Element>();

        [ObservableProperty]
        private int _playerScore;

        public ICommand ElementDragStartingCommand { get; }
        public ICommand GameBoardDropCommand { get; }
        public ICommand SpawnElementFromInventoryCommand { get; }
        public ICommand DuplicateElementCommand { get; }

        public GameViewModel()
        {
            _gameLogicService = new GameLogicService();
            _currentPlayer = new Player();
            LoadInitialGameState();

            ElementDragStartingCommand = new RelayCommand<Element>(ElementDragStarting);
            GameBoardDropCommand = new RelayCommand<DropEventArgs>(GameBoardDrop);
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

        // --- DragStarting: Кэшируем ССЫЛКУ НА ОБЪЕКТ ---
        private void ElementDragStarting(Element? element)
        {
            if (element == null) return;

            // Кэшируем живую ссылку на объект
            DraggedElement = element;

            Debug.WriteLine($"Drag STARTED: {element.Name} (InstanceId: {element.InstanceId})");
        }

        // --- Drop: Используем кэшированную ссылку ---
        private async void GameBoardDrop(DropEventArgs? e)
        {
            // Используем свойство _draggedElement
            Element? draggedElement = this.DraggedElement;

            if (draggedElement == null || e == null)
            {
                // Если элемент потерян, просто выходим
                Debug.WriteLine("Drop FAILED: Dragged element object is null.");
                return;
            }

            var position = e.GetPosition(null);
            if (!position.HasValue)
            {
                this.DraggedElement = null; // Очищаем кэш
                return;
            }

            // Находим живую ссылку на доске (необходимо, чтобы убедиться, что мы работаем с объектом из ObservableCollection)
            var liveDraggedElement = GameBoardElements.FirstOrDefault(el => el.InstanceId == draggedElement.InstanceId);

            if (liveDraggedElement == null)
            {
                Debug.WriteLine("Drop FAILED: Live element instance not found on board.");
                this.DraggedElement = null;
                return;
            }

            Debug.WriteLine($"Drop RECEIVED for {liveDraggedElement.Name} at X={position.Value.X}, Y={position.Value.Y}");

            // 1. Проверяем, попали ли мы на другой элемент (КОМБИНАЦИЯ)
            var targetElement = GameBoardElements.FirstOrDefault(el =>
                el.InstanceId != liveDraggedElement.InstanceId && // Нельзя комбинировать сам с собой
                el.Bounds.Contains(position.Value));

            if (targetElement != null)
            {
                // Попытка комбинации
                Element? result = _gameLogicService.Combine(liveDraggedElement, targetElement);

                if (result != null)
                {
                    await HandleSuccessfulCombination(result, targetElement);
                }
                else
                {
                    // НЕУДАЧНАЯ КОМБИНАЦИЯ
                    await Shell.Current.DisplayAlert("Алхимия", "new element coming soon, wait for update", "OK");
                }

                // Удаляем обе живые ссылки с доски
                GameBoardElements.Remove(liveDraggedElement);
                GameBoardElements.Remove(targetElement);
            }
            else
            {
                // 2. ПЕРЕМЕЩЕНИЕ
                double x = Math.Max(0, position.Value.X - 37.5);
                double y = Math.Max(0, position.Value.Y - 37.5);

                Debug.WriteLine($"Drop ACTION: Moving {liveDraggedElement.Name} to X={x}, Y={y}");

                // Обновляем Bounds, что автоматически перемещает элемент на AbsoluteLayout
                liveDraggedElement.Bounds = new Rect(x, y, liveDraggedElement.Bounds.Width, liveDraggedElement.Bounds.Height);
            }

            this.DraggedElement = null; // Очищаем кэш
        }

        // --- Вспомогательные методы (для работы функций) ---
        private async Task HandleSuccessfulCombination(Element result, Element targetElement)
        {
            Debug.WriteLine($"Успех! Получен: {result.Name} (ID: {result.Id})");

            result.Bounds = new Rect(targetElement.Bounds.X, targetElement.Bounds.Y, targetElement.Bounds.Width, targetElement.Bounds.Height);
            GameBoardElements.Add(result);

            bool isNew = _currentPlayer.DiscoverElement(result.Id);

            if (isNew)
            {
                if (!DiscoveredElements.Any(e => e.Id == result.Id))
                    DiscoveredElements.Add(result);

                int scoreGained = _gameLogicService.CalculateScoreForDiscovery(result);
                _currentPlayer.AddScore(scoreGained);
                PlayerScore = _currentPlayer.Score;

                await Shell.Current.DisplayAlert("Новый элемент!", $"Вы открыли: {result.Name}!", "OK");
            }
        }

        private void SpawnElementFromInventory(Element? element)
        {
            if (element == null) return;

            double x = _random.Next(50, 250);
            double y = _random.Next(50, 200);

            // Создаем НОВЫЙ элемент с новым InstanceId
            var newElement = new Element(element.Id, element.Name, element.ImagePath, new Rect(x, y, 75, 75));
            GameBoardElements.Add(newElement);
        }

        private void DuplicateElement(Element? element)
        {
            if (element == null) return;

            var newRect = new Rect(element.Bounds.X + 20, element.Bounds.Y + 20, element.Bounds.Width, element.Bounds.Height);
            // Создаем НОВЫЙ элемент с новым InstanceId
            var newElement = new Element(element.Id, element.Name, element.ImagePath, newRect);

            GameBoardElements.Add(newElement);
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