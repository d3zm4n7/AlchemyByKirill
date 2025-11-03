// In ViewModels/GameViewModel.cs

// --- УБЕДИСЬ, ЧТО ЭТИ USING ДОБАВЛЕНЫ ---
using AlchemyByKirill.Models;
using AlchemyByKirill.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input; // <--- НУЖЕН ДЛЯ [RelayCommand]
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input; // <--- НУЖЕН ДЛЯ ICommand
using Element = AlchemyByKirill.Models.Element;
// -----------------------------------------

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

        [ObservableProperty]
        private Element? _draggedElement;

        // Команды, которые будут вызваны из XAML
        public ICommand ElementDragStartingCommand { get; }
        public ICommand SpawnBaseElementsCommand { get; }
        public ICommand GameBoardDropCommand { get; }
        public ICommand ElementDroppedOnCommand { get; }



        public GameViewModel()
        {
            _gameLogicService = new GameLogicService();
            _currentPlayer = new Player();
            LoadInitialGameState();

            // Инициализация всех команд
            ElementDragStartingCommand = new RelayCommand<Element>(ElementDragStarting);
            SpawnBaseElementsCommand = new RelayCommand(SpawnBaseElementsOnBoard);
            // 'DropEventArgs' теперь должен найтись из-за 'using Microsoft.Maui.Controls;' (он обычно глобальный)
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
                // Проверяем, есть ли уже такой ID на поле
                if (!GameBoardElements.Any(boardElement => boardElement.Id == baseElement.Id))
                {
                    GameBoardElements.Add(baseElement);
                }
            }
            Debug.WriteLine("Базовые элементы добавлены на поле.");
        }

        // Вызывается, когда мы НАЧИНАЕМ тащить элемент
        private void ElementDragStarting(Element? element)
        {
            if (element == null) return;
            DraggedElement = element;
            Debug.WriteLine($"Dragging: {element.Name}");
        }

        // Вызывается, когда мы сбрасываем элемент на ПУСТОЕ ПОЛЕ (для перемещения)
        private void GameBoardDrop(DropEventArgs? e)
        {
            if (DraggedElement == null || e == null) return;

            var position = e.GetPosition(null);
            if (position.HasValue)
            {
                // Обновляем Bounds (X, Y) элемента.
                DraggedElement.Bounds = new Rect(position.Value.X, position.Value.Y, DraggedElement.Bounds.Width, DraggedElement.Bounds.Height);
                Debug.WriteLine($"Moved {DraggedElement.Name} to {DraggedElement.Bounds}");
            }
            DraggedElement = null; // Завершаем перетаскивание
        }

        // Вызывается, когда мы сбрасываем элемент НА ДРУГОЙ ЭЛЕМЕНТ (для комбинации)
        private void ElementDroppedOn(Element? targetElement)
        {
            // Нельзя сбросить элемент сам на себя
            if (DraggedElement == null || targetElement == null || DraggedElement == targetElement)
            {
                DraggedElement = null;
                return;
            }

            Debug.WriteLine($"Dropped {DraggedElement.Name} onto {targetElement.Name}");
            TryCombineElements(DraggedElement, targetElement);
            DraggedElement = null; // Завершаем перетаскивание
        }

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

        // --- ЭТИ МЕТОДЫ ДОЛЖНЫ БЫТЬ ВНУТРИ КЛАССА ---
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

    } // <--- ЭТО ПОСЛЕДНЯЯ СКОБКА КЛАССА
} // <--- ЭТО ПОСЛЕДНЯЯ СКОБКА NAMESPACE