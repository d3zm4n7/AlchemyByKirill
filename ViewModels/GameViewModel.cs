// In ViewModels/GameViewModel.cs
using AlchemyByKirill.Models;
using AlchemyByKirill.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
// ДОБАВЬ ЭТИ USING:
using System.Diagnostics;
using System.Windows.Input;
using Element = AlchemyByKirill.Models.Element;

namespace AlchemyByKirill.ViewModels
{
    public partial class GameViewModel : ObservableObject
    {
        private readonly GameLogicService _gameLogicService;
        private Player _currentPlayer;

        public ObservableCollection<Element> DiscoveredElements { get; } = new ObservableCollection<Element>();
        public ObservableCollection<Element> GameBoardElements { get; } = new ObservableCollection<Element>();

        [ObservableProperty]
        private int _playerScore;

        // ДОБАВЬ: Свойство для хранения элемента, который мы тащим
        [ObservableProperty]
        private Element? _draggedElement;

        // ОСТАВЬ: Эти команды нужны
        public ICommand ElementDragStartingCommand { get; }
        public ICommand SpawnBaseElementsCommand { get; }


        // ДОБАВЬ: Новые команды для сброса (Drop)
        public ICommand GameBoardDropCommand { get; }
        public ICommand ElementDroppedOnCommand { get; }


        public GameViewModel()
        {
            _gameLogicService = new GameLogicService();
            _currentPlayer = new Player();
            LoadInitialGameState();

            // ИЗМЕНИТЬ: Обновляем конструктор
            ElementDragStartingCommand = new RelayCommand<Element>(ElementDragStarting);
            SpawnBaseElementsCommand = new RelayCommand(SpawnBaseElementsOnBoard);

            // ДОБАВЬ: Инициализация новых команд
            GameBoardDropCommand = new RelayCommand<DropEventArgs>(GameBoardDrop);
            ElementDroppedOnCommand = new RelayCommand<Element>(ElementDroppedOn);

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

        private void SpawnBaseElementsOnBoard()
        {
            var baseElements = _gameLogicService.GetBaseElements();
            foreach (var baseElement in baseElements)
            {
                // Создаем *копию* элемента с уникальными Bounds
                var newElementOnBoard = new Element(baseElement.Id, baseElement.Name, baseElement.ImagePath, baseElement.Bounds);

                // Проверяем, есть ли уже такой элемент на поле
                if (!GameBoardElements.Any(boardElement => boardElement.Id == newElementOnBoard.Id))
                {
                    GameBoardElements.Add(newElementOnBoard);
                }
            }
            Debug.WriteLine("Базовые элементы добавлены на поле.");
        }

        // Этот метод вызывается, когда мы НАЧИНАЕМ тащить элемент
        private void ElementDragStarting(Element? element)
        {
            if (element == null) return;
            DraggedElement = element;
            Debug.WriteLine($"Dragging: {element.Name}");
        }

        // ДОБАВЬ: Метод для ПЕРЕМЕЩЕНИЯ (сброс на пустое поле)
        private void GameBoardDrop(DropEventArgs? e)
        {
            if (DraggedElement == null || e == null) return;

            var position = e.GetPosition(null);
            if (position.HasValue)
            {
                // Обновляем Bounds (X, Y) элемента. UI обновится, так как Element.Bounds - это [ObservableProperty]
                DraggedElement.Bounds = new Rect(position.Value.X, position.Value.Y, DraggedElement.Bounds.Width, DraggedElement.Bounds.Height);
                Debug.WriteLine($"Moved {DraggedElement.Name} to {DraggedElement.Bounds}");
            }
            DraggedElement = null; // Завершаем перетаскивание
        }

        // ДОБАВЬ: Метод для КОМБИНАЦИИ (сброс на другой элемент)
        private void ElementDroppedOn(Element? targetElement)
        {
            // Нельзя сбросить элемент сам на себя или если цель не определена
            if (DraggedElement == null || targetElement == null || DraggedElement == targetElement)
            {
                DraggedElement = null;
                return;
            }

            Debug.WriteLine($"Dropped {DraggedElement.Name} onto {targetElement.Name}");
            TryCombineElements(DraggedElement, targetElement);
            DraggedElement = null; // Завершаем перетаскивание
        }

        // УДАЛИ: Старые методы SelectElementForCombination и CanCombine
        // private void SelectElementForCombination(Element? element) { ... }
        // private bool CanCombine() { ... }

        // ИЗМЕНИТЬ: Метод TryCombineElements теперь принимает 2 элемента
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

                // Создаем новый элемент на месте 'цели' (element2)
                var newElement = new Element(result.Id, result.Name, result.ImagePath, element2.Bounds);
                GameBoardElements.Add(newElement);

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
                // Можно раскомментировать, если нужно сообщение об ошибке
                // await Shell.Current.DisplayAlert("Неудача", "Эти элементы не комбинируются.", "OK");
            }
        }

        // ... (Твои методы ClearBoard и Exit остаются без изменений) ...
        [RelayCommand]
        void ClearBoard()
        {
            GameBoardElements.Clear();
            // Можно также очистить слоты, если они у тебя есть
            // DropZone1Element = null;
            // DropZone2Element = null;
        }

        [RelayCommand]
        async Task Exit()
        {
            // Возвращаемся на предыдущую страницу (на StartPage)
            await Shell.Current.GoToAsync("..");
        }
    }
}