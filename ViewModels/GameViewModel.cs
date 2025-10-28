// In ViewModels/GameViewModel.cs
using AlchemyByKirill.Models;
using AlchemyByKirill.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Element = AlchemyByKirill.Models.Element; // Используем псевдоним

namespace AlchemyByKirill.ViewModels
{
    public partial class GameViewModel : ObservableObject
    {
        private readonly GameLogicService _gameLogicService;
        private Player _currentPlayer;

        // Инвентарь открытых элементов (для отображения внизу)
        public ObservableCollection<Element> DiscoveredElements { get; } = new ObservableCollection<Element>();

        // Коллекция для элементов на игровом поле
        public ObservableCollection<Element> GameBoardElements { get; } = new ObservableCollection<Element>();

        [ObservableProperty]
        private int _playerScore;

        // Команды
        public ICommand CombineCommand { get; }
        public ICommand SelectElementCommand { get; } // Для выбора касанием (если используется)
        public ICommand ElementDragStartingCommand { get; }
        public ICommand SpawnBaseElementsCommand { get; }

        // Свойство для хранения перетаскиваемого элемента
        [ObservableProperty]
        private Element? _draggedElement;

        // Элементы для комбинации (могут быть заменены логикой Drop зоны)
        private Element? _element1ToCombine;
        private Element? _element2ToCombine;

        public GameViewModel()
        {
            _gameLogicService = new GameLogicService();
            _currentPlayer = new Player();
            LoadInitialGameState(); // Загружает DiscoveredElements

            CombineCommand = new AsyncRelayCommand(TryCombineElements, CanCombine);
            SelectElementCommand = new RelayCommand<Element>(SelectElementForCombination); // Пока оставим
            ElementDragStartingCommand = new RelayCommand<Element>(HandleDragStarting);
            SpawnBaseElementsCommand = new RelayCommand(SpawnBaseElementsOnBoard);
        }

        /// <summary>
        /// Загружает начальное состояние игры (базовые элементы в инвентарь).
        /// </summary>
        private void LoadInitialGameState()
        {
            DiscoveredElements.Clear();
            GameBoardElements.Clear(); // Очищаем и поле
            _currentPlayer.Reset();

            var baseElements = _gameLogicService.GetBaseElements();
            foreach (var element in baseElements)
            {
                // Базовые элементы сразу известны, добавляем в инвентарь
                if (_currentPlayer.DiscoverElement(element.Id))
                {
                    DiscoveredElements.Add(element);
                }
            }
            PlayerScore = _currentPlayer.Score; // Обновляем отображение счета
        }

        /// <summary>
        /// Добавляет базовые элементы на игровое поле по команде (двойной тап).
        /// </summary>
        private void SpawnBaseElementsOnBoard()
        {
            var baseElements = _gameLogicService.GetBaseElements();
            bool added = false;
            foreach (var element in baseElements)
            {
                // Проверяем, есть ли уже такой элемент на поле
                if (!GameBoardElements.Any(boardElement => boardElement.Id == element.Id))
                {
                    GameBoardElements.Add(element);
                    added = true;
                }
            }
            if (added)
            {
                Console.WriteLine("Базовые элементы добавлены на поле.");
            }
            else
            {
                Console.WriteLine("Базовые элементы уже есть на поле.");
            }
        }

        /// <summary>
        /// Вызывается, когда пользователь начинает перетаскивать элемент с игрового поля.
        /// </summary>
        private void HandleDragStarting(Element? elementBeingDragged)
        {
            if (elementBeingDragged != null)
            {
                DraggedElement = elementBeingDragged; // Сохраняем перетаскиваемый элемент
                Console.WriteLine($"Начато перетаскивание: {DraggedElement.Name}");
            }
        }

        /// <summary>
        /// Вызывается при касании элемента в инвентаре (если используется TapGestureRecognizer).
        /// </summary>
        private void SelectElementForCombination(Element? element)
        {
            if (element == null) return;

            if (_element1ToCombine == null)
            {
                _element1ToCombine = element;
                Console.WriteLine($"Выбран первый элемент: {element.Name}");
            }
            else if (_element2ToCombine == null && _element1ToCombine.Id != element.Id)
            {
                _element2ToCombine = element;
                Console.WriteLine($"Выбран второй элемент: {element.Name}");
                (CombineCommand as AsyncRelayCommand)?.NotifyCanExecuteChanged();
            }
            else
            {
                _element1ToCombine = element;
                _element2ToCombine = null;
                Console.WriteLine($"Выбор сброшен. Снова выбран первый элемент: {element.Name}");
                (CombineCommand as AsyncRelayCommand)?.NotifyCanExecuteChanged();
            }
        }

        /// <summary>
        /// Определяет, можно ли выполнить команду CombineCommand (нужны два элемента).
        /// </summary>
        private bool CanCombine()
        {
            // Эта логика может измениться при использовании Drop зон
            return _element1ToCombine != null && _element2ToCombine != null;
        }

        /// <summary>
        /// Пытается скомбинировать _element1ToCombine и _element2ToCombine.
        /// </summary>
        private async Task TryCombineElements()
        {
            if (_element1ToCombine != null && _element2ToCombine != null)
            {
                Console.WriteLine($"Попытка комбинации: {_element1ToCombine.Name} + {_element2ToCombine.Name}");
                Element? result = _gameLogicService.Combine(_element1ToCombine, _element2ToCombine);

                if (result != null)
                {
                    Console.WriteLine($"Успех! Получен: {result.Name}");
                    bool isNew = _currentPlayer.DiscoverElement(result.Id);

                    if (isNew)
                    {
                        Console.WriteLine("Это новый элемент!");
                        // Добавляем и в инвентарь, и на поле? Или только в инвентарь?
                        // Пока добавляем только в инвентарь
                        if (!DiscoveredElements.Any(e => e.Id == result.Id))
                            DiscoveredElements.Add(result);
                        // Можно добавить и на поле, если нужно:
                        // if (!GameBoardElements.Any(e => e.Id == result.Id))
                        //     GameBoardElements.Add(result);

                        int scoreGained = _gameLogicService.CalculateScoreForDiscovery(result);
                        _currentPlayer.AddScore(scoreGained);
                        PlayerScore = _currentPlayer.Score;

                        await Shell.Current.DisplayAlert("Новый элемент!", $"Вы открыли: {result.Name}!", "OK");
                    }
                    else
                    {
                        Console.WriteLine("Элемент уже был открыт.");
                        await Shell.Current.DisplayAlert("Уже открыто", $"Элемент {result.Name} уже есть в вашей коллекции.", "OK");
                    }
                    // TODO: Удалить исходные элементы с поля?
                }
                else
                {
                    Console.WriteLine("Неудачная комбинация.");
                    await Shell.Current.DisplayAlert("Неудача", "Эти элементы не комбинируются.", "OK");
                }

                // Сбрасываем выбранные элементы после попытки
                _element1ToCombine = null;
                _element2ToCombine = null;
                Console.WriteLine("Выбор (касанием) сброшен после комбинации.");
                (CombineCommand as AsyncRelayCommand)?.NotifyCanExecuteChanged();
            }
            else
            {
                Console.WriteLine("Нужно выбрать два элемента для комбинации.");
            }
        }
        // Метод HandleDrop будет добавлен позже
    }
}