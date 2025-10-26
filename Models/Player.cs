// In Models/Player.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlchemyByKirill.Models
{
    internal class Player
    {
        public string Name { get; set; } = "Player"; // Имя по умолчанию
        public int Score { get; private set; } = 0; // Очки, изменять можно только через метод

        // Используем HashSet для быстрого поиска и уникальности ID открытых элементов
        public HashSet<int> DiscoveredElementIds { get; private set; } = new HashSet<int>();

        /// <summary>
        /// Добавляет очки игроку.
        /// </summary>
        public void AddScore(int points)
        {
            if (points > 0)
            {
                Score += points;
                // Здесь можно добавить логику для уведомления об изменении очков, если нужно
            }
        }

        /// <summary>
        /// Добавляет ID элемента в список открытых.
        /// Возвращает true, если элемент был действительно новым (еще не открытым).
        /// </summary>
        public bool DiscoverElement(int elementId)
        {
            return DiscoveredElementIds.Add(elementId); // Метод Add у HashSet возвращает true, если элемент был добавлен (т.е. его не было)
        }

        /// <summary>
        /// Сбрасывает очки и открытые элементы (для новой игры).
        /// </summary>
        public void Reset()
        {
            Score = 0;
            DiscoveredElementIds.Clear();
            // Сюда можно добавить сброс и добавление базовых элементов, если нужно
        }

        // Можно добавить конструктор, если нужно задавать имя при создании
        // public Player(string name)
        // {
        //     Name = name;
        // }
    }
}