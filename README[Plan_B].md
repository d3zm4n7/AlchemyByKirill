# 🧪 Alchemy By Kirill  
Интерактивная игра, созданная на **.NET MAUI**, основанная на принципах **объектно-ориентированного программирования (ООП)**.  
Игрок комбинирует элементы (огонь, вода, земля, воздух и т. д.), чтобы создавать новые вещества и открывать алхимические рецепты.

---

## 🎯 Цель проекта
Показать применение ООП, UI-взаимодействий и систем тем в практическом MAUI-приложении.

Игрок:
- исследует комбинации элементов,  
- зарабатывает очки,  
- открывает рецепты,  
- выбирает визуальную тему интерфейса,  
- наблюдает анимации и звуки алхимии.

---

## 🧱 Основные особенности

| Компонент | Описание |
|------------|-----------|
| 🧩 **ООП-архитектура** | Используются классы `Game`, `Player`, `Theme`, `Element`, `Recipe`, `Inventory` и др. |
| 🎨 **Система тем** | 3 стиля интерфейса: `LightTheme`, `DarkTheme`, `ArcaneTheme` (цвета, фон, шрифт). |
| 🎮 **Интерактивность** | Реализованы жесты: **Tap**, **Drag & Drop**, анимации при крафте. |
| 🔊 **Звуковое сопровождение** | Эффекты кликов, успеха и фона (`MediaManager`). |
| 💾 **Сохранение данных** | Результаты сохраняются через `Preferences` (история рекордов, имя игрока). |
| 🧠 **Рецепты и элементы** | Хранятся в JSON-файлах `elements.json` и `recipes.json`. |
| 🧑‍💻 **Паттерны и принципы** | Инкапсуляция, наследование, абстракции, DI и MVVM. |

---

## 📂 Структура проекта

```
AlchemyByKirill/                       // Корень решения (.sln рядом)
├─ AlchemyByKirill/                    // Основной MAUI-проект (.csproj)
│  ├─ App.xaml                         // Глобальные ресурсы (DynamicResource ключи тем, стили)
│  ├─ App.xaml.cs                      // Запуск, смена темы (переключение ResourceDictionary)
│  ├─ MauiProgram.cs                   // DI: сервисы, ViewModels
│  │
│  ├─ Models/                          // Чистые ООП-классы (без UI)
│  │  ├─ Player.cs                     // Имя, очки, лучшие результаты (инкапсуляция, методы AddScore)
│  │  ├─ Theme/
│  │  │  ├─ Theme.cs                   // Базовый класс/интерфейс ITheme: цвета, шрифт, Apply()
│  │  │  ├─ LightTheme.cs              // Светлая тема
│  │  │  ├─ DarkTheme.cs               // Тёмная тема
│  │  │  ├─ ArcaneTheme.cs             // «Алхимическая» (фиолет/неон, кастомный шрифт)
│  │  ├─ Game/
│  │  │  ├─ GameBase.cs                // Абстракция: Start/Stop/Reset, IsRunning, Score, Time
│  │  │  ├─ AlchemyGame.cs             // Логика Алхимии: тик-таймер, обработка матчей/крафта
│  │  │  ├─ Board.cs                   // Игровое поле/сцена (ячейки/слоты верстака)
│  │  │  ├─ Cell.cs                    // Ячейка поля (позиция, содержимое)
│  │  │  ├─ Element.cs                 // Базовый элемент (Id, Name, Rarity, SpriteKey)
│  │  │  ├─ Recipe.cs                  // Рецепт: входы -> выход, проверка совместимости
│  │  │  ├─ Inventory.cs               // Рюкзак/колода доступных элементов
│  │  │  ├─ CraftingSystem.cs          // Комбинаторика: TryCombine(Element a,b[,c]) -> Element?
│  │  │  ├─ ScoreSystem.cs             // Подсчёт очков/комбо/временных бонусов
│  │  │  ├─ Timer.cs                   // Внутриигровой таймер (отсчёт времени раунда)
│  │  ├─ DTO/
│  │  │  ├─ ElementDto.cs              // Для загрузки из JSON
│  │  │  ├─ RecipeDto.cs
│  │
│  ├─ Services/                        // Логика вокруг платформы/хранения/звука
│  │  ├─ IAudioService.cs              // PlayClick/PlaySuccess/PlayFail/Music
│  │  ├─ AudioService.cs               // Реализация через MediaManager/IAudio (кроссплатформенно)
│  │  ├─ IStorageService.cs            // Чтение/запись JSON, Preferences
│  │  ├─ StorageService.cs             // Реализация (FileSystem + Preferences)
│  │  ├─ IThemeService.cs              // Текущая тема, список тем, смена темы
│  │  ├─ ThemeService.cs               // Хранение/применение тем, Raise ThemeChanged
│  │  ├─ IRecipeProvider.cs            // Загрузка рецептов/элементов
│  │  ├─ JsonRecipeProvider.cs         // Читает из Resources/Raw/*.json
│  │  ├─ ILeaderboardService.cs        // Топ-результаты
│  │  ├─ LeaderboardService.cs         // Хранение в Preferences/файле
│  │
│  ├─ ViewModels/                      // MVVM: биндинги для XAML
│  │  ├─ MainViewModel.cs              // Приветствие, ввод имени, навигация «Играть»
│  │  ├─ GameViewModel.cs              // Текущее состояние: очки/время/инвентарь/Drag&Drop команды
│  │  ├─ SettingsViewModel.cs          // Смена темы, громкость, язык
│  │  ├─ LeaderboardViewModel.cs       // Загрузка/очистка рейтинга
│  │
│  ├─ Views/                           // XAML-страницы (UI)
│  │  ├─ MainPage.xaml                 // Экран «Старт» (ввод имени, выбор темы, кнопка «Играть»)
│  │  ├─ MainPage.xaml.cs
│  │  ├─ GamePage.xaml                 // Основная сцена (поле, инвентарь, котёл)
│  │  ├─ GamePage.xaml.cs              // Жесты: Tap/Drag&Drop, анимации Combine
│  │  ├─ SettingsPage.xaml             // Выбор тем (3+), шрифт, фон (DynamicResource)
│  │  ├─ SettingsPage.xaml.cs
│  │  ├─ LeaderboardPage.xaml          // Таблица рекордов
│  │  ├─ LeaderboardPage.xaml.cs
│  │  ├─ AboutPage.xaml                // Коротко об игре/авторе
│  │  ├─ AboutPage.xaml.cs
│  │
│  ├─ Controls/                        // Переиспользуемые UI-контролы
│  │  ├─ ElementView.xaml              // Визуал элемента (иконка+текст), эффекты при drag
│  │  ├─ ElementView.xaml.cs
│  │  ├─ CauldronView.xaml             // «Котёл» — зона drop, мигает при валидном комбо
│  │  ├─ CauldronView.xaml.cs
│  │  ├─ SlotView.xaml                 // Слот/ячейка на поле
│  │  ├─ SlotView.xaml.cs
│  │
│  ├─ Converters/
│  │  ├─ RarityToColorConverter.cs     // Привязка редкости к цвету
│  │  ├─ BoolToOpacityConverter.cs     // Для подсветки активных зон
│  │
│  ├─ Behaviors/
│  │  ├─ DragBehavior.cs               // Реализация drag для ElementView
│  │  ├─ DropBehavior.cs               // Реализация drop для Cauldron/Slot
│  │  ├─ TapBehavior.cs                // Tap по элементам/кнопкам
│  │
│  ├─ Animations/
│  │  ├─ AnimationHelpers.cs           // Сборник: Pulse, Shake, FloatUp, FadeInOut
│  │
│  ├─ Resources/
│  │  ├─ Fonts/
│  │  │  ├─ Arcane.ttf                 // Шрифт темы Arcane
│  │  │  └─ Inter-Regular.ttf
│  │  ├─ Images/
│  │  │  ├─ elements/                  // Спрайты элементов (png/svg)
│  │  │  │  ├─ fire.png
│  │  │  │  ├─ water.png
│  │  │  │  ├─ earth.png
│  │  │  │  ├─ air.png
│  │  │  │  ├─ metal.png
│  │  │  │  └─ ... (иконки крафтов)
│  │  │  ├─ ui/
│  │  │  │  ├─ cauldron.png
│  │  │  │  ├─ slot_empty.png
│  │  │  │  └─ bg_arcane.jpg           // Фоны тем
│  │  │  ├─ appicon.svg                // Иконка приложения
│  │  │  └─ splash.svg                 // Экран загрузки (в стиле иконки)
│  │  ├─ Raw/
│  │  │  ├─ elements.json              // Начальные элементы (id,name,rarity,sprite)
│  │  │  ├─ recipes.json               // Рецепты (входы -> выход)
│  │  │  └─ tips.json                  // Подсказки/фразы
│  │  ├─ Styles/
│  │  │  ├─ Colors.xaml                // Ключи цветов: Primary, Text, Bg, Accent
│  │  │  ├─ Styles.xaml                // Кнопки, Label, Frame, ElementView
│  │  │  └─ Themes.xaml                // ResourceDictionary для трёх тем (DynamicResource)
│  │  ├─ Sounds/
│  │  │  ├─ click.mp3
│  │  │  ├─ success.mp3
│  │  │  ├─ fail.mp3
│  │  │  └─ music_loop.mp3
│  │
│  ├─ Platforms/
│  │  ├─ Android/
│  │  │  ├─ AndroidManifest.xml
│  │  │  └─ Resources/                 // mipmap-* (генерится MAUI), настройки иконки/сплэша
│  │  ├─ iOS/
│  │  │  └─ Info.plist                 // App Icon/Splash через MAUI assets
│  │
│  ├─ Helpers/
│  │  ├─ ResultDialog.cs               // Унифицированный DisplayAlert об окончании игры
│  │  ├─ Extensions.cs                 // Утилиты (e.g. WithSafeFireAndForget)
│  │
│  ├─ Localization/
│  │  ├─ Strings.resx                  // en
│  │  ├─ Strings.ru.resx               // ru
│  │  └─ Strings.et.resx               // et
│  │
│  ├─ Constants/
│  │  ├─ ResourceKeys.cs               // Строковые ключи DynamicResource (цвета/стили)
│  │  └─ GameConstants.cs              // Тайминги, лимиты, стартовые очки
│  │
│  └─ README.dev.md                    // Как собрать/где править темы, где JSON
│
└─ tests/                               // (Необязательно) юнит-тесты ядра
   ├─ AlchemyByKirill.Tests/
   │  ├─ CraftingSystemTests.cs        // Проверка рецептов/комбинаций
   │  ├─ ScoreSystemTests.cs
   │  └─ TimerTests.cs
```
Как это покрывает требования

ООП: Models/Game/* + Theme/* + Player + инкапсуляция + отдельные файлы.

Три темы: LightTheme, DarkTheme, ArcaneTheme + Themes.xaml + ThemeService.

UI + взаимодействия: GamePage (XAML) + Behaviors (DragBehavior, DropBehavior, TapBehavior) + AnimationHelpers.

Результаты и статистика: ScoreSystem, Timer, LeaderboardService, ResultDialog.

Сохранения: StorageService (Preferences/файлы), LeaderboardService.

Звуки: AudioService + Resources/Sounds.

Данные игры: elements.json, recipes.json (гибко расширять контент без перекомпиляции).

Мини-гайды по ключевым точкам

Применение темы (ООП)
Theme.Apply(ContentPage page) меняет Application.Current.Resources.MergedDictionaries или выставляет DynamicResource-ключи из Themes.xaml. ThemeService хранит текущую тему, нотифицирует VM.

Drag & Drop
ElementView → прикрепляем DragBehavior, DropBehavior на CauldronView/SlotView. В drop-обработчике VM вызывает CraftingSystem.TryCombine(...), при успехе: звук + анимация + апдейт инвентаря/очков.

Animation
AnimationHelpers.Pulse(view), Shake(view), FloatUp(view) — вызываются из GamePage.xaml.cs после событий VM.

Связь XAML ↔ VM
Все страницы используют BindingContext из DI (MauiProgram), команды и свойства в VM. Для цветов/шрифтов — DynamicResource ключи (ResourceKeys.cs).
