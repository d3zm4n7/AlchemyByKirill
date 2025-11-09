using AlchemyByKirill.Models;
using AlchemyByKirill.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using Element = AlchemyByKirill.Models.Element;
using Microsoft.Maui.Controls;
using AlchemyByKirill.Views;
using AlchemyByKirill.Helpers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Maui.Storage;

namespace AlchemyByKirill.ViewModels
{
    public partial class GameViewModel : ObservableObject
    {
        private readonly GameLogicService _gameLogicService;
        private Player _currentPlayer;
        private Random _random = new Random();
        private const string SaveKey_Discovered = "discovered_elements";
        private const string SaveKey_Board = "board_elements";
        public Action<string>? ShowMessage;

        // --- ИСПРАВЛЕНИЕ: Мы удалили [ObservableProperty] ---
        // Это приватное поле, которое не должно обнуляться
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
            GameBoardDropCommand = new RelayCommand<(DropEventArgs, AbsoluteLayout)>(GameBoardDrop);
            SpawnElementFromInventoryCommand = new RelayCommand<Element>(SpawnElementFromInventory);
            DuplicateElementCommand = new RelayCommand<Element>(DuplicateElement);
            DiscoveredElements.CollectionChanged += (_, __) => SaveGame();
            GameBoardElements.CollectionChanged += (_, __) => SaveGame();
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

        // --- DragStarting: Кэшируем ССЫЛКУ НА ОБЪЕКТ в приватное поле ---
        private void ElementDragStarting(Element? element)
        {
            if (element == null) return;

            // ИСПРАВЛЕНИЕ: Используем приватное поле _draggedElement
            _draggedElement = element;

            Debug.WriteLine($"Drag STARTED: {element.Name} (InstanceId: {element.InstanceId})");
        }

        // --- Drop: Используем кэшированную ссылку из приватного поля ---
        private async void GameBoardDrop((DropEventArgs e, AbsoluteLayout layout) args)
        {
            var e = args.e;
            var layout = args.layout;

            if (_draggedElement == null)
                return;

            var position = e.GetPosition(layout);
            if (!position.HasValue)
                return;

            var live = GameBoardElements.FirstOrDefault(el => el.InstanceId == _draggedElement.InstanceId);
            if (live == null)
                return;

            // Проверка на комбинацию
            var target = GameBoardElements.FirstOrDefault(el =>
                el.InstanceId != live.InstanceId &&
                el.Bounds.Contains(position.Value));

            if (target != null)
            {
                var result = _gameLogicService.Combine(live, target);

                GameBoardElements.Remove(live);
                GameBoardElements.Remove(target);

                if (result != null)
                {
                    await HandleSuccessfulCombination(result, target);
                }
                return;
            }

            // ✅ Перемещение (исправлено!)
            live.Bounds = new Rect(
                position.Value.X - live.Bounds.Width / 2,
                position.Value.Y - live.Bounds.Height / 2,
                live.Bounds.Width,
                live.Bounds.Height
            );

            _draggedElement = null;
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
        public void SaveGame()
        {
            var discoveredJson = JsonSerializer.Serialize(DiscoveredElements);
            Preferences.Set("discovered", discoveredJson);

            var boardJson = JsonSerializer.Serialize(GameBoardElements);
            Preferences.Set("board", boardJson);
        }

        public void LoadGame()
        {
            if (Preferences.ContainsKey("discovered"))
            {
                var json = Preferences.Get("discovered", "");
                var list = JsonSerializer.Deserialize<ObservableCollection<Element>>(json);
                if (list != null)
                {
                    DiscoveredElements.Clear();
                    foreach (var e in list) DiscoveredElements.Add(e);
                }
            }

            if (Preferences.ContainsKey("board"))
            {
                var json = Preferences.Get("board", "");
                var list = JsonSerializer.Deserialize<ObservableCollection<Element>>(json);
                if (list != null)
                {
                    GameBoardElements.Clear();
                    foreach (var e in list) GameBoardElements.Add(e);
                }
            }
        }

        private void SpawnElementFromInventory(Element? element)
        {
            if (element == null) return;
            double x = _random.Next(50, 250);
            double y = _random.Next(50, 200);
            var newElement = new Element(element.Id, element.Name, element.ImagePath, new Rect(x, y, 75, 75));
            GameBoardElements.Add(newElement);
        }

        private void DuplicateElement(Element? element)
        {
            if (element == null) return;
            var newRect = new Rect(element.Bounds.X + 20, element.Bounds.Y + 20, element.Bounds.Width, element.Bounds.Height);
            var newElement = new Element(element.Id, element.Name, element.ImagePath, newRect);
            GameBoardElements.Add(newElement);
        }

        public void OnDrop(Point point)
        {
            if (_draggedElement == null)
                return;

            var live = GameBoardElements.FirstOrDefault(el => el.InstanceId == _draggedElement.InstanceId);
            if (live == null)
                return;

            live.Bounds = new Rect(
                point.X - live.Bounds.Width / 2,
                point.Y - live.Bounds.Height / 2,
                live.Bounds.Width,
                live.Bounds.Height
            );

            _draggedElement = null;
        }

        public void DropAt(Point point)
        {
            if (_draggedElement == null)
                return;

            var live = GameBoardElements.FirstOrDefault(el => el.InstanceId == _draggedElement.InstanceId);
            if (live == null)
            {
                _draggedElement = null;
                return;
            }

            // Проверка на комбинацию — попали на другой элемент?
            var target = GameBoardElements.FirstOrDefault(el =>
                el.InstanceId != live.InstanceId &&
                el.Bounds.Contains(point));

            if (target != null)
            {
                // Пробуем создать новый элемент
                var result = _gameLogicService.Combine(live, target);

                if (result == null)
                {
                    // ❗ Комбинации нет → элементы НЕ удаляем
                    ShowMessage?.Invoke("Комбинация пока в разработке ✨");
                    _draggedElement = null;
                    return;
                }

                // ✅ Комбинация есть → создаём новый элемент
                GameBoardElements.Remove(live);
                GameBoardElements.Remove(target);

                // Новый элемент появляется на месте второго (как в алхимии Little Alchemy)
                result.Bounds = new Rect(target.Bounds.X, target.Bounds.Y, target.Bounds.Width, target.Bounds.Height);
                GameBoardElements.Add(result);

                bool isNew = _currentPlayer.DiscoverElement(result.Id);
                if (isNew)
                {
                    if (!DiscoveredElements.Any(e => e.Id == result.Id))
                        DiscoveredElements.Add(result);

                    int scoreGained = _gameLogicService.CalculateScoreForDiscovery(result);
                    _currentPlayer.AddScore(scoreGained);
                    PlayerScore = _currentPlayer.Score;
                }

                _draggedElement = null;
                return;
            }

            // Если не попали ни в какой элемент → просто перемещаем
            live.Bounds = new Rect(
                point.X - live.Bounds.Width / 2,
                point.Y - live.Bounds.Height / 2,
                live.Bounds.Width,
                live.Bounds.Height
            );

            _draggedElement = null;
        }



        [RelayCommand]
        private void ClearBoard()
        {
            GameBoardElements.Clear();
            SaveGame();
        }

        [RelayCommand]
        private async Task Exit()
        {
            SaveGame();
            await Shell.Current.GoToAsync("..");
        }
        [RelayCommand]
        private async Task OpenLibrary()
        {
            var vm = ServiceHelper.GetService<LibraryViewModel>();
            vm.Load(DiscoveredElements);
            await Shell.Current.GoToAsync(nameof(LibraryPage));
        }
    }
}