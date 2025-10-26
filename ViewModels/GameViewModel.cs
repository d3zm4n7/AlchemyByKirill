// In ViewModels/GameViewModel.cs
using AlchemyByKirill.Models;
using AlchemyByKirill.Services;
using Element = AlchemyByKirill.Models.Element;
using CommunityToolkit.Mvvm.ComponentModel; // Для ObservableObject
using CommunityToolkit.Mvvm.Input; // Для AsyncRelayCommand
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel; // Для ObservableCollection
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input; // Для ICommand

namespace AlchemyByKirill.ViewModels
{
    // Наследуемся от ObservableObject для упрощения INotifyPropertyChanged
    internal partial class GameViewModel : ObservableObject
    {
        private readonly GameLogicService _gameLogicService;
        private Player _currentPlayer; // Ссылка на текущего игрока

        // Коллекция элементов, доступных игроку (отображается в инвентаре)
        // ObservableCollection автоматически уведомляет UI об изменениях
        public ObservableCollection<Element> DiscoveredElements { get; } = new ObservableCollection<Element>();

        // Свойство для хранения очков игрока с уведомлением UI
        [ObservableProperty]
        private int _playerScore; // CommunityToolkit сгенерирует свойство PlayerScore

        // Команда для комбинации элементов (пока без параметров для простоты)
        public ICommand CombineCommand { get; }

        // Элементы, выбранные для комбинации (упрощенно, пока без Drag&Drop)
        private Element? _element1ToCombine;
        private Element? _element2ToCombine;

        public GameViewModel()
        {
            _gameLogicService = new GameLogicService(); // Создаем сервис (позже через DI)
            _currentPlayer = new Player(); // Создаем нового игрока

            LoadInitialGameState();

            // Создаем команду. Метод TryCombineElements будет вызван при выполнении команды
            CombineCommand = new AsyncRelayCommand(TryCombineElements);
        }

        /// <summary>
        /// Загружает начальное состояние игры (базовые элементы).
        /// </summary>
        private void LoadInitialGameState()
        {
            DiscoveredElements.Clear();
            _currentPlayer.Reset(); // Сбрасываем игрока

            // Добавляем базовые элементы в инвентарь и в список открытых у игрока
            var baseElements = _gameLogicService.GetBaseElements();
            foreach (var element in baseElements)
            {
                DiscoveredElements.Add(element);
                _currentPlayer.DiscoverElement(element.Id); // Отмечаем как открытый
            }
            PlayerScore = _currentPlayer.Score; // Обновляем отображение счета
        }

        // Временный метод для выбора элементов (заменится логикой Drag&Drop)
        public void SelectElementForCombination(Element element)
        {
            if (_element1ToCombine == null)
            {
                _element1ToCombine = element;
                Console.WriteLine($"Выбран первый элемент: {element.Name}");
            }
            else if (_element2ToCombine == null && _element1ToCombine != element)
            {
                _element2ToCombine = element;
                Console.WriteLine($"Выбран второй элемент: {element.Name}");
                // Автоматически пытаемся скомбинировать, когда выбраны два
                if (CombineCommand.CanExecute(null))
                {
                    CombineCommand.Execute(null);
                }
            }
            // Если выбраны одинаковые или уже есть два, сбрасываем
            else
            {
                _element1ToCombine = element;
                _element2ToCombine = null;
                Console.WriteLine($"Выбор сброшен. Выбран первый элемент: {element.Name}");
            }
        }


        /// <summary>
        /// Логика, выполняемая при вызове CombineCommand.
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
                    // Пытаемся добавить элемент к игроку
                    bool isNew = _currentPlayer.DiscoverElement(result.Id);

                    if (isNew)
                    {
                        Console.WriteLine("Это новый элемент!");
                        DiscoveredElements.Add(result); // Добавляем в видимый список
                        int scoreGained = _gameLogicService.CalculateScoreForDiscovery(result);
                        _currentPlayer.AddScore(scoreGained);
                        PlayerScore = _currentPlayer.Score; // Обновляем счет в UI

                        // TODO: Показать какое-то уведомление/анимацию успеха
                        await Shell.Current.DisplayAlert("Новый элемент!", $"Вы открыли: {result.Name}!", "OK");
                    }
                    else
                    {
                        Console.WriteLine("Элемент уже был открыт.");
                        // TODO: Показать уведомление, что элемент уже есть
                        await Shell.Current.DisplayAlert("Уже открыто", $"Элемент {result.Name} уже есть в вашей коллекции.", "OK");
                    }
                }
                else
                {
                    Console.WriteLine("Неудачная комбинация.");
                    // TODO: Показать уведомление/анимацию неудачи
                    await Shell.Current.DisplayAlert("Неудача", "Эти элементы не комбинируются.", "OK");
                }

                // Сбрасываем выбранные элементы после попытки
                _element1ToCombine = null;
                _element2ToCombine = null;
                Console.WriteLine("Выбор сброшен.");
            }
            else
            {
                Console.WriteLine("Нужно выбрать два элемента для комбинации.");
            }
        }
    }
}