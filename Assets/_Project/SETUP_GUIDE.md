# Детальное руководство по сборке проекта Gravitarium: Reset

## Содержание
1. [Подготовка проекта Unity](#подготовка-проекта-unity)
2. [Настройка Input System](#настройка-input-system)
3. [Создание сцен](#создание-сцен)
4. [Настройка менеджеров](#настройка-менеджеров)
5. [Создание игрока](#создание-игрока)
6. [Настройка UI](#настройка-ui)
7. [Создание головоломок](#создание-головоломок)
8. [Создание предметов](#создание-предметов)
9. [Настройка аудио](#настройка-аудио)
10. [Настройка VFX](#настройка-vfx)
11. [Настройка оптимизации](#настройка-оптимизации)
12. [Тестирование](#тестирование)

---

## Подготовка проекта Unity

### 1.1 Проверка версии Unity
- Рекомендуется Unity 2021.3 LTS или новее
- Убедитесь, что проект использует Input System (не Legacy)

### 1.2 Установка необходимых пакетов
Откройте **Window > Package Manager** и установите:

1. **Input System** (если еще не установлен)
   - Window > Package Manager > Unity Registry > Input System

2. **Cinemachine** (для камер и кат-сцен)
   - Window > Package Manager > Unity Registry > Cinemachine

3. **Timeline** (для кат-сцен)
   - Window > Package Manager > Unity Registry > Timeline

4. **Post Processing** (опционально, для постобработки)
   - Window > Package Manager > Unity Registry > Post Processing

### 1.3 Настройка проекта
1. Откройте **Edit > Project Settings**
2. Перейдите в **Player > Other Settings**
3. Убедитесь, что **Active Input Handling** установлен в **Input System Package (New)** или **Both**

### 1.4 Настройка слоев (Layers)
1. Откройте **Edit > Project Settings > Tags and Layers**
2. Создайте новые слои:
   - **Draggable** (для перетаскиваемых предметов)
   - **DropZone** (для зон сброса)
3. Эти слои будут использоваться системой перетаскивания предметов

---

## Настройка Input System

### 2.1 Проверка Input Actions
Файл `Input/InputSystem.inputactions` уже существует. Если нужно добавить действие для переворота гравитации:

1. Откройте `Input/InputSystem.inputactions` в редакторе
2. В Action Map "Player" добавьте новое действие:
   - **Name**: `FlipGravity`
   - **Type**: Button
   - **Bindings**: 
     - Keyboard: `G` (или любая другая клавиша)
     - Gamepad: `Button Y` (или другая кнопка)

3. Сохраните файл (Ctrl+S)
4. Unity автоматически перегенерирует `InputSystem.cs`

**Примечание**: В текущей реализации переворот гравитации привязан к действию "Attack" (левая кнопка мыши / Enter). Это можно изменить в `PlayerController.cs`, метод `OnAttack`.

---

## Создание сцен

### 3.1 Сцена MainMenu

1. **Создайте новую сцену**: File > New Scene > Basic (Built-in)
2. **Сохраните как**: `Assets/_Project/Scenes/MainMenu.unity`

3. **Настройте сцену**:
   - Удалите Directional Light (если есть)
   - Создайте Canvas: GameObject > UI > Canvas
   - Установите Canvas Scaler:
     - UI Scale Mode: Scale With Screen Size
     - Reference Resolution: 1920x1080

4. **Создайте структуру UI**:
   ```
   Canvas
   ├── MainMenuPanel
   │   ├── Title (Text)
   │   ├── StartButton (Button)
   │   ├── LoadButton (Button)
   │   ├── SettingsButton (Button)
   │   └── QuitButton (Button)
   └── EventSystem
   ```

5. **Добавьте компоненты**:
   - На Canvas добавьте `UIManager`
   - На MainMenuPanel добавьте `MainMenu`
   - Привяжите кнопки в инспекторе MainMenu:
     - Start Button → startGameButton
     - Load Button → loadGameButton
     - Settings Button → settingsButton
     - Quit Button → quitButton

6. **Создайте GameManager**:
   - GameObject > Create Empty > назовите "GameManager"
   - Добавьте компонент `GameManager`
   - В инспекторе GameManager привяжите UIManager (перетащите Canvas)

### 3.2 Сцена Level01_StationHub

1. **Создайте новую сцену**: File > New Scene > Basic (Built-in)
2. **Сохраните как**: `Assets/_Project/Scenes/Level01_StationHub.unity`

3. **Базовая настройка**:
   - Создайте плоскость для пола: GameObject > 3D Object > Plane
   - Назовите "Floor"
   - Добавьте тег "Ground" (если нужно для проверки земли)

4. **Создайте GameManager**:
   - GameObject > Create Empty > "GameManager"
   - Добавьте компонент `GameManager`

5. **Создайте менеджеры** (GameObject > Create Empty для каждого):
   - **AudioManager**: добавьте компонент `AudioManager`
   - **SaveManager**: добавьте компонент `SaveManager`
   - **SceneLoader**: добавьте компонент `SceneLoader`
   - **ObjectPoolManager**: добавьте компонент `ObjectPoolManager`
   - **LevelManager**: добавьте компонент `LevelManager`
   - **PuzzleManager**: добавьте компонент `PuzzleManager`
   - **HazardManager**: добавьте компонент `HazardManager`

6. **Привяжите менеджеры к GameManager**:
   - Выберите GameManager
   - В инспекторе перетащите все созданные менеджеры в соответствующие поля

7. **Настройте камеру**:
   - Выберите Main Camera
   - Добавьте простой скрипт камеры или используйте Cinemachine:
     - Создайте Virtual Camera: GameObject > Cinemachine > Virtual Camera
     - Настройте Follow и Look At на игрока (создадим позже)

### 3.3 Сцена Level02_ReactorCore

1. **Создайте аналогично Level01_StationHub**
2. **Сохраните как**: `Assets/_Project/Scenes/Level02_ReactorCore.unity`
3. Повторите шаги 3-7 из Level01_StationHub

### 3.4 Сцена LoadingScreen (опционально)

1. **Создайте новую сцену**: File > New Scene > Basic (Built-in)
2. **Сохраните как**: `Assets/_Project/Scenes/LoadingScreen.unity`

3. **Настройте UI**:
   - Создайте Canvas
   - Создайте панель с прогресс-баром:
     ```
     Canvas
     ├── LoadingPanel
     │   ├── LoadingText (Text)
     │   ├── ProgressBar (Slider)
     │   └── ProgressText (Text)
     └── EventSystem
     ```

4. **Добавьте компоненты**:
   - На LoadingPanel добавьте `LoadingScreen`
   - Привяжите UI элементы в инспекторе

---

## Настройка менеджеров

### 4.1 GameManager

1. Выберите GameManager на сцене
2. В инспекторе убедитесь, что все менеджеры привязаны
3. **Важно**: GameManager должен быть на сцене, которая не выгружается (или используйте DontDestroyOnLoad)

### 4.2 AudioManager

1. Выберите AudioManager
2. **Настройте Audio Clips**:
   - Создайте папку `Assets/_Project/Audio/Music`
   - Создайте папку `Assets/_Project/Audio/SFX`
   - Создайте папку `Assets/_Project/Audio/Footsteps`
   - Импортируйте аудио файлы
   - В инспекторе AudioManager:
     - **Background Music Tracks**: перетащите музыкальные треки (массив)
     - **Gravity Flip Sound**: перетащите звук переворота гравитации
     - **Terminal Activation Sound**: перетащите звук активации терминала
     - **Item Collect Sound**: перетащите звук сбора предмета
     - **Door Open Sound**: перетащите звук открытия двери
     - **Door Locked Sound**: перетащите звук заблокированной двери
     - **Puzzle Solved Sound**: перетащите звук решения головоломки
     - **Metal Footstep**: перетащите один звук шагов по металлу
     - **Concrete Footstep**: перетащите один звук шагов по бетону

3. **Настройте громкость**:
   - Music Volume: 0.7 (по умолчанию)
   - SFX Volume: 1.0 (по умолчанию)

**Важно для звуков шагов**:
- PlayerFootsteps автоматически использует звуки из AudioManager
- Система использует 2 типа поверхностей (Metal и Concrete) с одним клипом на каждый тип
- Разнообразие достигается за счет автоматического изменения тональности (pitch) звука
- Звуки должны быть короткими (0.3-0.8 секунды)
- Убедитесь, что звуки нормализованы (не слишком громкие/тихие)
- Если поверхность не имеет тега "Metal", используется звук бетона

### 4.3 ObjectPoolManager

**ObjectPoolManager** управляет пулом объектов для оптимизации производительности. Все визуальные эффекты (VFX) должны использовать пул объектов вместо прямого создания/уничтожения.

#### Настройка ObjectPoolManager

1. **Выберите ObjectPoolManager** на сцене
2. **Создайте префабы для пула**:
   - Создайте папку `Assets/_Project/Prefabs/VFX`
   - Создайте Particle System для каждого эффекта:
     - Spark Effect (искры)
     - Energy Discharge Effect (энергетический разряд)
     - Gravity Flip Effect (переворот гравитации)
     - Item Collect Effect (сбор предмета)
   - Сохраните как префабы

3. **Настройте пулы в инспекторе**:
   - Нажмите "+" в списке Pools
   - Для каждого эффекта:
     - **Tag**: "Spark", "EnergyDischarge", "GravityFlip", "ItemCollect" (используется для вызова через ObjectPoolManager)
     - **Prefab**: перетащите соответствующий префаб
     - **Size**: 10 (количество объектов в пуле, увеличьте для часто используемых эффектов)

#### Использование ObjectPoolManager в коде

**Получение объекта из пула:**
```csharp
// Получить объект из пула
GameObject effect = ObjectPoolManager.Instance.SpawnFromPool("Spark", position, rotation);

// Объект автоматически активируется и позиционируется
// После завершения эффекта он автоматически вернется в пул
```

**Возврат объекта в пул (обычно не требуется, если объект реализует IPooledObject):**
```csharp
// Вернуть объект в пул
ObjectPoolManager.Instance.ReturnToPool("Spark", effect);
```

**Создание нового пула во время выполнения:**
```csharp
// Создать новый пул
ObjectPoolManager.Instance.CreatePool("NewEffect", prefab, 10);
```

**Интерфейс IPooledObject:**
Если ваш префаб эффекта реализует интерфейс `IPooledObject`, методы `OnObjectSpawn()` и `OnObjectReturn()` будут вызываться автоматически:
```csharp
public class MyEffect : MonoBehaviour, IPooledObject
{
    public void OnObjectSpawn()
    {
        // Вызывается при получении объекта из пула
        // Здесь можно перезапустить Particle System и т.д.
    }

    public void OnObjectReturn()
    {
        // Вызывается при возврате объекта в пул
        // Здесь можно остановить эффекты
    }
}
```

### 4.5 SceneLoader

1. Выберите SceneLoader
2. **Loading Scene Name**: "LoadingScreen" (или оставьте пустым, если используете UI)

### 4.6 LevelManager

1. Выберите LevelManager
2. В инспекторе настройте Level Names:
   - Element 0: "Level01_StationHub"
   - Element 1: "Level02_ReactorCore"

### 4.7 LevelTransitionTrigger (Триггер перехода между уровнями)

**LevelTransitionTrigger** - компонент для автоматической загрузки следующего уровня при входе игрока в зону.

#### Настройка триггера перехода

1. **Создайте GameObject** для триггера:
   - GameObject > Create Empty
   - Назовите "LevelTransitionTrigger" или "NextLevelTrigger"

2. **Добавьте Collider**:
   - Добавьте компонент **Box Collider** (или Sphere/Capsule Collider)
   - Установите **Is Trigger = true**
   - Настройте размер и позицию коллайдера (зона перехода)

3. **Добавьте компонент LevelTransitionTrigger**:
   - Add Component > LevelTransitionTrigger

4. **Настройте параметры**:
   - **Load Next Level**: true (загружает следующий уровень из LevelManager)
   - **Specific Level Name**: оставьте пустым (или укажите конкретный уровень, если Load Next Level = false)
   - **Show Debug Message**: true (показывать сообщение в консоли)
   - **Debug Message**: "Переход на следующий уровень..." (текст сообщения)

5. **Важно**: Убедитесь, что:
   - Игрок имеет тег "Player" или компонент PlayerController
   - LevelManager настроен и содержит список уровней
   - SceneLoader присутствует на сцене

#### Пример использования

**Автоматический переход на следующий уровень:**
- Создайте триггер в конце уровня
- Установите Load Next Level = true
- При входе игрока в зону автоматически:
  1. Сохраняется игра
  2. Загружается следующий уровень

**Переход на конкретный уровень:**
- Установите Load Next Level = false
- Укажите Specific Level Name = "Level02_ReactorCore"
- При входе игрока загрузится указанный уровень

#### Визуализация в редакторе

Триггер отображается зеленым полупрозрачным кубом/сферой в Scene View для удобной настройки.

### 4.8 SaveManager

**SaveManager** управляет сохранением и загрузкой игры. Все данные сохраняются в JSON файлы в папке `Application.persistentDataPath/Saves/`.

#### Настройка SaveManager

1. **Выберите SaveManager** на сцене
2. **Настройте параметры в инспекторе**:
   - **Save File Name**: "savegame.json" (имя файла сохранения, по умолчанию)

#### Что сохраняется

SaveManager автоматически сохраняет:
- **Позицию и состояние игрока** (позиция, поворот, состояние гравитации)
- **Характеристики игрока** (здоровье, энергия, максимальные значения)
- **Инвентарь**:
  - Ключ-карты (ID)
  - Энергетические ядра (ID)
  - Записки (полные данные: ID, заголовок, текст, аудио)
- **Прогресс квестов** (ID квеста, прогресс, статус завершения)
- **Текущий уровень** (название сцены)
- **Метаданные** (дата сохранения, время игры)

#### Использование SaveManager в коде

**Сохранение игры:**
```csharp
// Сохранить игру
SaveManager.Instance.SaveGame();
```

**Загрузка игры:**
```csharp
// Загрузить игру
bool success = SaveManager.Instance.LoadGame();
if (success)
{
    Debug.Log("Игра успешно загружена");
}
else
{
    Debug.Log("Сохранение не найдено");
}
```

**Проверка существования сохранения:**
```csharp
// Проверить, существует ли сохранение
if (SaveManager.Instance.SaveExists())
{
    Debug.Log("Сохранение существует");
}
```

**Удаление сохранения:**
```csharp
// Удалить сохранение
SaveManager.Instance.DeleteSave();
```

#### Пример интеграции в UI

**Добавление кнопок сохранения/загрузки в меню:**

1. **В MainMenu.cs**:
```csharp
public void OnSaveButtonClicked()
{
    SaveManager.Instance.SaveGame();
    // Показать уведомление об успешном сохранении
}

public void OnLoadButtonClicked()
{
    if (SaveManager.Instance.SaveExists())
    {
        SaveManager.Instance.LoadGame();
        // Загрузить сцену или обновить UI
    }
    else
    {
        // Показать сообщение "Сохранение не найдено"
    }
}
```

2. **В PauseMenu.cs**:
```csharp
public void OnSaveGame()
{
    SaveManager.Instance.SaveGame();
    // Показать уведомление
}

public void OnLoadGame()
{
    if (SaveManager.Instance.SaveExists())
    {
        SaveManager.Instance.LoadGame();
        // Возможно, перезагрузить сцену
    }
}
```

#### Автоматическое сохранение

**Важно**: При сохранении данные собираются из всех менеджеров (InventoryManager, PlayerController и т.д.), поэтому убедитесь, что все менеджеры инициализированы.

**Автоматическое сохранение при переходе между уровнями:**
- LevelTransitionTrigger автоматически сохраняет игру перед загрузкой следующего уровня

#### Расположение файлов сохранений

Файлы сохранений находятся в:
- **Windows**: `%USERPROFILE%\AppData\LocalLow\<CompanyName>\<ProductName>\Saves\`
- **Mac**: `~/Library/Application Support/<CompanyName>/<ProductName>/Saves/`
- **Linux**: `~/.config/unity3d/<CompanyName>/<ProductName>/Saves/`

Формат имени файла: `savegame.json` (одна ячейка сохранения)

#### Структура файла сохранения

Пример JSON файла сохранения:
```json
{
    "playerPosition": {"x": 0, "y": 1, "z": 0},
    "playerRotation": {"x": 0, "y": 0, "z": 0, "w": 1},
    "isGravityFlipped": false,
    "playerHealth": 100.0,
    "playerMaxHealth": 100.0,
    "playerEnergy": 100.0,
    "playerMaxEnergy": 100.0,
    "collectedKeyCards": ["keycard_001", "keycard_002"],
    "collectedEnergyCores": ["core_001", "core_002"],
    "collectedNotes": [
        {
            "noteId": "note_001",
            "noteTitle": "Записка",
            "noteText": "Текст записки..."
        }
    ],
    "questProgressData": {
        "entries": [
            {
                "questId": "collect_energy_cores",
                "progress": 0.5,
                "isCompleted": false
            }
        ]
    },
    "currentLevel": "Level01_StationHub",
    "saveDate": "2024-01-15 12:30:45",
    "playTime": 1234.56
}
```

#### Загрузка после смены сцены

При загрузке сохранения, если текущая сцена отличается от сохраненной, SaveManager автоматически загрузит нужную сцену через SceneLoader и применит данные после загрузки.

#### Очистка инвентаря при загрузке

При загрузке сохранения InventoryManager автоматически очищается и заполняется данными из сохранения. Это гарантирует, что состояние инвентаря соответствует сохраненному.

---

## Создание игрока

### 5.1 Создание префаба игрока

1. **Создайте GameObject**: GameObject > 3D Object > Capsule
2. **Назовите**: "Player"
3. **Настройте Transform**:
   - Position: (0, 1, 0)
   - Scale: (1, 1, 1)

4. **Добавьте компоненты**:
   - **Rigidbody** (обязательно для физического движения)
   - **PlayerController**
   - **GravitySystem**
   - **PlayerStats** (создайте как ScriptableObject или используйте компонент)
   - **PlayerFootsteps**
   - **CapsuleCollider** (на дочернем объекте Body или на самом Player)

5. **Настройте Rigidbody**:
   - Mass: 1
   - Drag: 0
   - Angular Drag: 0.05
   - Use Gravity: false (управляется через GravitySystem)
   - Is Kinematic: false
   - Interpolate: Interpolate (для плавного движения)
   - Constraints: Freeze Rotation X, Y, Z (вращение управляется скриптом)

6. **Настройте PlayerController**:
   - Move Speed: 5
   - Sprint Multiplier: 1.5
   - Jump Force: 8
   - Ground Check Distance: 0.1
   - Ground Layer: Default (или создайте слой "Ground")
   - Air Control Multiplier: 0.3 (контроль в воздухе)
   - Mouse Sensitivity: 2
   - Vertical Look Limit: 80 (ограничение вертикального вращения камеры в градусах)
   - Camera Transform: перетащите Main Camera (или оставьте пустым для автоматического поиска)
   - Gravity Flip Rotation Speed: 180
   - Item Drag System: перетащите ItemDragSystem (если есть на сцене)

7. **Настройте GravitySystem**:
   - Gravity Strength: 9.81
   - Gravity Flip Effect: создайте Particle System и привяжите
   - **Важно**: GravitySystem автоматически отключает стандартную гравитацию Unity (useGravity = false)

8. **Настройте PlayerStats**:
   - Max Health: 100
   - Current Health: 100
   - Max Energy: 100
   - Current Energy: 100

9. **Настройте PlayerFootsteps**:
   - Footstep Interval: 0.5 (интервал между шагами в секундах)
   - Surface Check Distance: 0.2 (дистанция проверки поверхности)
   - Pitch Variation: 0.15 (вариация тональности для разнообразия звуков, диапазон 0.1-0.3)
   - Metal Tag: "Metal" (тег для металлических поверхностей)
   - Concrete Tag: "Concrete" (тег для бетонных поверхностей)
   
   **Важно**: 
   - Звуки шагов настраиваются в AudioManager (по одному клипу на каждый тип поверхности)
   - Система автоматически изменяет тональность звука для разнообразия
   - Если поверхность не имеет тега "Metal", используется звук бетона

10. **Сохраните как префаб**: перетащите в `Assets/_Project/Prefabs/Player.prefab`

### 5.2 Размещение игрока на сцене

1. Откройте сцену Level01_StationHub
2. Перетащите префаб Player на сцену
3. Установите позицию: (0, 1, 0)

### 5.3 Настройка камеры

**Вариант 1: Камера как дочерний объект игрока (рекомендуется)**
1. Создайте пустой GameObject как дочерний объект Player
2. Назовите его "CameraPivot" или "CameraHolder"
3. Установите позицию: (0, 1.6, 0) - на уровне глаз
4. Перетащите Main Camera в CameraPivot
5. Установите позицию камеры: (0, 0, 0) относительно CameraPivot
6. В PlayerController привяжите Camera Transform к Main Camera
7. **Важно**: PlayerController автоматически управляет вертикальным вращением камеры

**Вариант 2: Отдельная камера с следованием**
1. Выберите Main Camera
2. Создайте скрипт `CameraFollow.cs`:
```csharp
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 1.6, 0);
    public float smoothSpeed = 0.125f;

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
            // Горизонтальное вращение управляется PlayerController
            // Вертикальное вращение также управляется PlayerController
        }
    }
}
```
3. Добавьте компонент на камеру
4. Привяжите Player в поле Target
5. В PlayerController привяжите Camera Transform к Main Camera

**Вариант 3: Cinemachine**
1. Создайте Virtual Camera: GameObject > Cinemachine > Virtual Camera
2. Настройте:
   - Follow: Player
   - Look At: Player (или CameraPivot)
   - Body: 3rd Person Follow
   - Aim: Composer
3. **Важно**: При использовании Cinemachine отключите автоматическое вращение камеры в PlayerController или настройте интеграцию

**Примечание**: PlayerController автоматически обрабатывает вертикальное вращение камеры с ограничениями (Vertical Look Limit). Горизонтальное вращение применяется к игроку, вертикальное - к камере.

---

## Настройка UI

### 6.1 Главное меню (MainMenu)

1. На сцене MainMenu создайте Canvas (если еще не создан)
2. **Создайте структуру**:
   ```
   Canvas
   ├── MainMenuPanel
   │   ├── Title (Text) - "GRAVITARIUM: RESET"
   │   ├── StartButton (Button) - "Начать игру"
   │   ├── LoadButton (Button) - "Загрузить игру"
   │   ├── SettingsButton (Button) - "Настройки"
   │   └── QuitButton (Button) - "Выход"
   └── EventSystem
   ```

3. **Добавьте компоненты**:
   - На Canvas: `UIManager`
   - На MainMenuPanel: `MainMenu`
   - Привяжите кнопки в MainMenu

4. **В UIManager привяжите**:
   - Main Menu Panel → mainMenuPanel

### 6.2 Меню настроек (SettingsMenu)

1. **Создайте панель настроек**:
   ```
   Canvas
   └── SettingsPanel
       ├── MusicVolumeSlider (Slider)
       ├── MusicVolumeText (Text)
       ├── SFXVolumeSlider (Slider)
       ├── SFXVolumeText (Text)
       ├── QualityDropdown (Dropdown)
       └── BackButton (Button)
   ```

2. **Добавьте компонент**: `SettingsMenu` на SettingsPanel
3. **Привяжите все UI элементы** в инспекторе
4. **В UIManager привяжите**: Settings Panel → settingsPanel

### 6.3 Меню паузы (PauseMenu)

1. **Создайте панель паузы**:
   ```
   Canvas
   └── PauseMenuPanel
       ├── ResumeButton (Button) - "Возобновить"
       ├── SaveGameButton (Button) - "Сохранить игру"
       ├── LoadGameButton (Button) - "Загрузить игру"
       ├── MainMenuButton (Button) - "Главное меню"
       └── (по умолчанию скрыта)
   ```

2. **Добавьте компонент**: `PauseMenu` на PauseMenuPanel
3. **Привяжите кнопки**:
   - Resume Button → resumeButton
   - Save Game Button → saveGameButton
   - Load Game Button → loadGameButton
   - Main Menu Button → mainMenuButton
4. **В UIManager привяжите**: Pause Menu Panel → pauseMenuPanel

**Примечание**: Кнопка сохранения сохраняет игру, но не закрывает меню паузы, чтобы игрок мог продолжить играть.

### 6.4 Экран загрузки (LoadingScreen)

1. **Создайте панель загрузки**:
   ```
   Canvas
   └── LoadingScreenPanel
       ├── LoadingText (Text) - "Загрузка..."
       ├── ProgressBar (Slider)
       └── ProgressText (Text)
   ```

2. **Добавьте компонент**: `LoadingScreen` на LoadingScreenPanel
3. **Привяжите UI элементы**
4. **В UIManager привяжите**: Loading Screen Panel → loadingScreenPanel

### 6.5 Игровой HUD (GameHUD)

1. **Создайте HUD**:
   ```
   Canvas
   └── GameHUDPanel
       ├── HealthBar (Slider)
       ├── HealthText (Text)
       ├── GravityIndicator (Image)
       ├── GravityText (Text)
       ├── EnergyCoresText (Text)
       └── InteractionHint (GameObject)
           └── InteractionHintText (Text)
   ```

2. **Добавьте компонент**: `GameHUD` на GameHUDPanel
3. **Привяжите все UI элементы**
4. **Настройте цвета**:
   - Gravity Indicator Normal: синий
   - Gravity Indicator Flipped: красный
5. **В UIManager привяжите**: Game HUD Panel → gameHUDPanel

### 6.6 Журнал (JournalUI)

1. **Создайте панель журнала**:
   ```
   Canvas
   └── JournalPanel
       ├── NotesList (ScrollView)
       │   └── Content
       │       └── (NoteEntryPrefab будет создаваться динамически)
       ├── NoteTitleText (Text)
       ├── NoteContentText (Text)
       ├── PlayAudioButton (Button)
       ├── StopAudioButton (Button)
       └── CloseButton (Button)
   ```

2. **Создайте префаб записки**:
   - GameObject > UI > Button
   - Назовите "NoteEntryPrefab"
   - Добавьте Text как дочерний объект
   - Сохраните как префаб: `Assets/_Project/Prefabs/UI/NoteEntryPrefab.prefab`

3. **Добавьте компонент**: `JournalUI` на JournalPanel
4. **Привяжите**:
   - Notes Container: Content из ScrollView
   - Note Entry Prefab: созданный префаб
   - Все остальные UI элементы
5. **В UIManager привяжите**: Journal Panel → journalPanel

---

## Создание головоломок

### 7.1 GravityPuzzle

1. **Создайте GameObject**: GameObject > Create Empty > "GravityPuzzle_01"
2. **Добавьте компонент**: `GravityPuzzle`
3. **Настройте**:
   - Puzzle ID: "gravity_puzzle_01"
   - Target Objects: создайте несколько объектов (кубы), которые нужно переместить
   - Target Positions: создайте пустые GameObject'ы в позициях, куда нужно переместить объекты
   - Position Tolerance: 0.5
   - Materials: создайте материалы для правильного/неправильного состояния

4. **Настройте объекты**:
   - Добавьте Rigidbody к объектам (если нужно)
   - Убедитесь, что они реагируют на гравитацию

### 7.2 KeyPuzzle

1. **Создайте GameObject**: GameObject > Create Empty > "KeyPuzzle_01"
2. **Добавьте компонент**: `KeyPuzzle`
3. **Настройте**:
   - Puzzle ID: "key_puzzle_01"
   - Required Energy Cores: 3
   - Key Slots: создайте пустые GameObject'ы для размещения ключей
   - Slot Renderers: добавьте Renderer'ы к слотам
   - Materials: для пустых и заполненных слотов

### 7.3 TerminalHackPuzzle (Процесс согласования подсистемы)

1. **Создайте терминал**:
   - GameObject > 3D Object > Cube
   - Назовите "Terminal_01"
   - Добавьте компонент `Terminal`

2. **Создайте головоломку**:
   - GameObject > Create Empty > "TerminalHackPuzzle_01"
   - Добавьте компонент `TerminalHackPuzzle`
   - Настройте:
     - Terminal Hack UI: будет назначен позже

3. **Создайте Canvas в World Space**:
   - Создайте Canvas: GameObject > UI > Canvas
   - Настройте Canvas:
     - Render Mode: **World Space**
     - Canvas Scaler: отключите (не нужен для World Space)
     - Rect Transform:
       - Width: 1920
       - Height: 1080
       - Scale: (0.01, 0.01, 0.01) - для правильного размера в 3D
   - Создайте структуру:
     ```
     TerminalCanvas (Canvas в World Space)
     ├── TerminalPanel (GameObject)
     │   ├── Background (Image) - темно-синий фон с градиентом
     │   ├── TitleText (TextMeshProUGUI) - "ПРОЦЕСС СОГЛАСОВАНИЯ ПОДСИСТЕМЫ"
     │   ├── ColumnsContainer (RectTransform) - контейнер для 4 колонок
     │   ├── ButtonsContainer (RectTransform) - контейнер для кнопок со стрелками
     │   ├── StatusText (TextMeshProUGUI) - текст статуса
     │   └── CloseButton (Button) - кнопка закрытия
     └── EventSystem
     ```

4. **Создайте префаб колонки (ColumnPrefab)**:
   - GameObject > UI > Panel (или пустой GameObject с RectTransform)
   - Назовите "ColumnPrefab"
   - Настройте RectTransform:
     - Width: 200
     - Height: 400
   - Добавьте Image для фона (опционально)
   - Сохраните как префаб: `Assets/_Project/Prefabs/UI/ColumnPrefab.prefab`

5. **Создайте префаб ячейки значения (ValueCellPrefab)**:
   - GameObject > UI > Panel
   - Назовите "ValueCellPrefab"
   - Настройте RectTransform:
     - Width: 180
     - Height: 80
   - Добавьте дочерний объект:
     - GameObject > UI > Text - TextMeshProUGUI
     - Назовите "ValueText"
     - Настройте TextMeshProUGUI:
       - Font Size: 48
       - Alignment: Center
       - Color: Белый
       - Text: "0"
   - Сохраните как префаб: `Assets/_Project/Prefabs/UI/ValueCellPrefab.prefab`

6. **Создайте префаб кнопки со стрелками (ArrowButtonPrefab)**:
   - GameObject > UI > Panel
   - Назовите "ArrowButtonPrefab"
   - Настройте RectTransform:
     - Width: 150
     - Height: 100
   - Добавьте компонент `ArrowButton`
   - Создайте структуру:
     ```
     ArrowButtonPrefab
     ├── Background (Image) - фон кнопки
     ├── UpArrowButton (Button) - кнопка стрелки вверх
     │   └── UpArrowImage (Image) - зеленая стрелка вверх
     └── DownArrowButton (Button) - кнопка стрелки вниз
         └── DownArrowImage (Image) - красная стрелка вниз
     ```
   - Настройте ArrowButton:
     - Up Arrow Button: перетащите UpArrowButton
     - Down Arrow Button: перетащите DownArrowButton
     - Up Arrow Image: перетащите UpArrowImage (зеленый цвет)
     - Down Arrow Image: перетащите DownArrowImage (красный цвет)
     - Button Background: перетащите Background
   - Сохраните как префаб: `Assets/_Project/Prefabs/UI/ArrowButtonPrefab.prefab`

7. **Настройте TerminalHackUI**:
   - На TerminalCanvas добавьте компонент `TerminalHackUI`
   - Привяжите в инспекторе:
     - Puzzle: TerminalHackPuzzle_01
     - Terminal Canvas: TerminalCanvas
     - Close Button: CloseButton
     - Status Text: StatusText
     - Title Text: TitleText (опционально)
     - Columns Container: ColumnsContainer
     - Buttons Container: ButtonsContainer
     - Column Prefab: созданный ColumnPrefab
     - Value Cell Prefab: созданный ValueCellPrefab
     - Arrow Button Prefab: созданный ArrowButtonPrefab
     - Columns Count: 4 (количество колонок)
     - Values Per Column: 8 (количество значений в колонке)
     - Visible Values: 4 (количество видимых значений)
     - Canvas Distance: 1.5 (расстояние от игрока до Canvas)
     - Canvas Scale: 0.01 (масштаб Canvas в World Space)

8. **Настройте визуальный стиль**:
   - Фон терминала: темно-синий цвет с градиентом (светлее в центре, темнее по краям)
   - Заголовок: белый текст, крупный шрифт
   - Ячейки значений: белые цифры на темном фоне
   - Кнопки: металлический/глянцевый вид
   - Стрелки: зеленая (вверх) и красная (вниз)

9. **Настройте Terminal**:
    - Terminal ID: "terminal_01"
    - Puzzle: перетащите TerminalHackPuzzle_01 (или оставьте пустым, система найдет автоматически)

10. **Свяжите терминал с дверью**:
    - Выберите TerminalHackPuzzle_01
    - В инспекторе найдите поле "Target Door"
    - Перетащите дверь, которая должна открыться при решении головоломки
    - **Важно**: Дверь автоматически откроется при успешном прохождении мини-игры

10. **Механика головоломки**:
    - В каждой из 4 колонок находится 8 значений (7 нулей и одна единица)
    - Видно только 4 значения из 8 (окно видимости)
    - Игрок использует прицел в центре экрана для нажатия на стрелки
    - Стрелки вверх/вниз сдвигают видимую область колонки
    - Цель: выровнять все единицы в одну строку (например, во вторую строку видимой области)
    - При успешном выравнивании головоломка решается

11. **Особенности реализации**:
    - **2D мини-игра в 3D мире**: Canvas работает в World Space и всегда смотрит на камеру
    - **Управление не блокируется**: игрок может двигаться и смотреть во время работы с терминалом
    - **Управление через прицел**: кнопки нажимаются лучом из центра экрана (прицел)
    - **Автоматическая генерация**: начальные позиции единиц генерируются случайно
    - **Визуальная обратная связь**: выделение активной кнопки при наведении
    - **Автоматическое закрытие**: терминал закрывается через 1.5 секунды после успешного решения
    - **Открытие двери**: при успешном решении автоматически открывается связанная дверь (если назначена)

### 7.4 Регистрация головоломок

1. Выберите PuzzleManager на сцене
2. В инспекторе нажмите "+" в списке Puzzles
3. Перетащите все созданные головоломки

### 7.5 PuzzleManager - Детальное описание

**PuzzleManager** - это централизованная система управления всеми головоломками на уровне. Он отвечает за регистрацию, отслеживание прогресса и координацию работы головоломок.

#### Основные функции PuzzleManager:

1. **Автоматическая регистрация головоломок**:
   - При инициализации (`Awake`) PuzzleManager автоматически находит все головоломки на сцене через `FindObjectsOfType<BasePuzzle>()`
   - Если список головоломок пуст в инспекторе, он заполняется автоматически
   - Создается словарь для быстрого доступа к головоломкам по их ID

2. **Отслеживание прогресса**:
   - Подсчитывает общее количество головоломок на уровне (`TotalPuzzles`)
   - Отслеживает количество решенных головоломок (`SolvedPuzzles`)
   - Вычисляет процент прогресса (`Progress = SolvedPuzzles / TotalPuzzles`)
   - Подписывается на событие `EventBus.OnPuzzleSolved` для автоматического обновления счетчика

3. **Управление состоянием головоломок**:
   - **Автоматическая активация**: При инициализации все головоломки со состоянием `NotStarted` автоматически переводятся в состояние `InProgress`, делая их доступными для прохождения
   - **Сброс головоломок**: Метод `ResetAllPuzzles()` сбрасывает все головоломки в начальное состояние (используется при начале новой игры)
   - **Пересчет прогресса**: Метод `RecalculateSolvedCount()` пересчитывает количество решенных головоломок (используется при загрузке сохранения)

4. **Быстрый доступ к головоломкам**:
   - Метод `GetPuzzle(string puzzleId)` позволяет получить головоломку по её уникальному ID
   - Используется другими системами (например, `HazardManager`, `SaveManager`) для работы с конкретными головоломками

5. **Интеграция с другими системами**:
   - **QuestManager**: PuzzleManager не вызывает события напрямую, но QuestManager подписывается на `EventBus.OnPuzzleSolved` для обновления квестов
   - **SaveManager**: Использует PuzzleManager для сохранения и загрузки состояния головоломок
   - **HazardManager**: Использует PuzzleManager для получения головоломок по ID при отключении опасностей

#### Состояния головоломок (PuzzleState):

- **NotStarted**: Головоломка еще не начата (начальное состояние)
- **InProgress**: Головоломка активна и доступна для прохождения (устанавливается автоматически при инициализации)
- **Solved**: Головоломка решена
- **Failed**: Головоломка провалена (не используется в текущей реализации)

#### Автоматическая активация головоломок:

При инициализации PuzzleManager автоматически вызывает `SetAllPuzzlesToInProgress()`, который:
- Проходит по всем зарегистрированным головоломкам
- Если головоломка в состоянии `NotStarted`, переводит её в `InProgress`
- Использует `RestorePuzzleState()` для установки состояния без вызова событий
- Это гарантирует, что все головоломки доступны для прохождения с начала уровня

#### Пример использования в коде:

```csharp
// Получить головоломку по ID
BasePuzzle puzzle = PuzzleManager.Instance.GetPuzzle("key_puzzle_01");

// Получить прогресс прохождения
float progress = PuzzleManager.Instance.Progress; // 0.0 - 1.0
int solved = PuzzleManager.Instance.SolvedPuzzles;
int total = PuzzleManager.Instance.TotalPuzzles;

// Сбросить все головоломки (при начале новой игры)
PuzzleManager.Instance.ResetAllPuzzles();

// Пересчитать прогресс (при загрузке сохранения)
PuzzleManager.Instance.RecalculateSolvedCount();

// Установить все головоломки в InProgress (вызывается автоматически)
PuzzleManager.Instance.SetAllPuzzlesToInProgress();
```

#### Важные замечания:

- **PuzzleManager должен быть на сцене**: Добавьте компонент `PuzzleManager` на GameObject в сцене
- **Автоматическое обнаружение**: Если список головоломок пуст, они будут найдены автоматически
- **Уникальные ID**: Каждая головоломка должна иметь уникальный `PuzzleId`
- **Состояние InProgress**: Все головоломки автоматически переводятся в `InProgress` при инициализации, что делает их доступными для прохождения
- **События**: PuzzleManager подписывается на `EventBus.OnPuzzleSolved` для отслеживания прогресса

---

## Создание предметов

### 8.1 EnergyCore (Энергетическое ядро)

1. **Создайте объект**:
   - GameObject > 3D Object > Sphere
   - Назовите "EnergyCore_01"
   - Масштаб: (0.5, 0.5, 0.5)

2. **Добавьте компоненты**:
   - `EnergyCore`
   - Collider (Sphere Collider, Is Trigger: true)

3. **Настройте EnergyCore**:
   - Core ID: "core_001"
   - Visual Object: создайте дочерний объект с MeshRenderer для визуализации
   - Collect Effect: создайте Particle System и привяжите
   - Collect Sound: перетащите аудио клип

4. **Создайте визуальные эффекты**:
   - Добавьте Particle System для свечения
   - Добавьте Light компонент (опционально)

5. **Сохраните как префаб**: `Assets/_Project/Prefabs/Items/EnergyCore.prefab`

### 8.2 Terminal (Терминал)

1. **Создайте объект**:
   - GameObject > 3D Object > Cube
   - Назовите "Terminal_01"

2. **Добавьте компоненты**:
   - `Terminal`
   - Collider (Box Collider)

3. **Настройте Terminal**:
   - Terminal ID: "terminal_001"
   - Terminal Renderer: MeshRenderer куба
   - Terminal Light: добавьте Light компонент
   - Activation Effect: создайте Particle System
   - Puzzle: привяжите TerminalHackPuzzle (или оставьте пустым для автоматического поиска)

4. **Создайте материалы**:
   - Activated Material: зеленый/синий материал
   - Deactivated Material: серый/красный материал

5. **Сохраните как префаб**: `Assets/_Project/Prefabs/Items/Terminal.prefab`

### 8.3 DraggableItem3D (Перетаскиваемый предмет в 3D)

1. **Создайте объект**:
   - GameObject > 3D Object > Cube (или Sphere, или используйте модель)
   - Назовите "DraggableItem3D_01"

2. **Добавьте компоненты**:
   - `DraggableItem3D`
   - Collider (Box Collider или Sphere Collider)

3. **Настройте DraggableItem3D**:
   - Drag Distance: 2 (расстояние от камеры при перетаскивании)
   - Return Speed: 5 (скорость возврата на место)
   - Drop Zone Layer: выберите слой "DropZone"

4. **Настройте слой**:
   - Создайте новый слой "Draggable" в Project Settings > Tags and Layers
   - Назначьте этот слой объекту

5. **Сохраните как префаб**: `Assets/_Project/Prefabs/Items/DraggableItem3D.prefab`

### 8.4 DropZone3D (Зона сброса в 3D)

1. **Создайте объект**:
   - GameObject > 3D Object > Cube
   - Назовите "DropZone3D_01"
   - Масштаб: (1, 0.1, 1) - плоская платформа

2. **Добавьте компоненты**:
   - `DropZone3D`
   - Collider (Box Collider)

3. **Настройте DropZone3D**:
   - Can Place Multiple: false (или true, если нужно несколько предметов)
   - Max Items: 1

4. **Настройте слой**:
   - Назначьте слой "DropZone" объекту

5. **Добавьте визуализацию**:
   - Создайте Material с прозрачностью или подсветкой
   - Добавьте Light компонент (опционально)

6. **Сохраните как префаб**: `Assets/_Project/Prefabs/Items/DropZone3D.prefab`

### 8.5 NarrativeNote (Записка)

1. **Создайте объект**:
   - GameObject > 3D Object > Quad (или используйте модель записки)
   - Назовите "Note_01"

2. **Добавьте компоненты**:
   - `NarrativeNote`
   - Collider (Box Collider, Is Trigger: true)

3. **Настройте NarrativeNote**:
   - Note ID: "note_001"
   - Note Title: "Записка #1"
   - Note Text: "Текст записки..."
   - Is Audio Log: false (или true, если аудио)
   - Audio Log Clip: перетащите аудио (если аудио-дневник)
   - Visual Object: создайте дочерний объект для визуализации
   - Collect Effect: создайте Particle System

4. **Сохраните как префаб**: `Assets/_Project/Prefabs/Items/NarrativeNote.prefab`

### 8.6 Door (Дверь)

1. **Создайте объект**:
   - GameObject > 3D Object > Cube
   - Назовите "Door_01"
   - Масштаб: (1, 2, 0.2)

2. **Добавьте компоненты**:
   - `Door`
   - Collider (Box Collider)

3. **Настройте Door**:
   - Door ID: "door_001"
   - Required Energy Cores: 1
   - Requires Puzzle: false (или true)
   - Required Puzzle: привяжите головоломку (если нужно)
   - Use Animation: true
   - Open Position: установите позицию открытой двери
   - Door Renderer: MeshRenderer
   - Door Light: добавьте Light компонент

4. **Создайте материалы**:
   - Locked Material: красный материал
   - Unlocked Material: зеленый материал

5. **Сохраните как префаб**: `Assets/_Project/Prefabs/Items/Door.prefab`

### 8.7 EnergyDischarge (Энергетический разряд)

1. **Создайте объект**:
   - GameObject > 3D Object > Sphere (или используйте модель)
   - Назовите "EnergyDischarge_01"
   - Масштаб: (1, 1, 1)

2. **Добавьте компоненты**:
   - `EnergyDischarge`
   - Collider (Sphere Collider, Is Trigger: true)
   - Particle System для визуального эффекта
   - Light компонент (опционально)

3. **Настройте EnergyDischarge**:
   - Damage Per Second: 10 (урон в секунду)
   - Damage Interval: 0.5 (интервал нанесения урона)
   - Discharge Effect: перетащите Particle System
   - Discharge Light: перетащите Light компонент
   - Light Intensity: 2
   - Discharge Sound: перетащите аудио клип
   - Is Active: true (включен по умолчанию)

4. **Настройте визуальные эффекты**:
   - Particle System: синий/фиолетовый цвет, эффект электрического разряда
   - Light: синий цвет, пульсирующая интенсивность

5. **Сохраните как префаб**: `Assets/_Project/Prefabs/Hazards/EnergyDischarge.prefab`

---

## Настройка HazardManager

### 8.8 Настройка связей между головоломками и опасностями

**HazardManager** управляет отключением конкретных энергетических разрядов при решении конкретных головоломок.

#### Настройка HazardManager

1. **Выберите HazardManager** на сцене

2. **Настройте Puzzle-Hazard Links**:
   - В инспекторе найдите список "Puzzle Hazard Links"
   - Нажмите "+" для добавления новой связи

3. **Для каждой связи настройте**:
   - **Puzzle Id**: ID головоломки (например, "key_puzzle_01", "gravity_puzzle_01")
   - **Energy Discharges To Disable**: перетащите все EnergyDischarge, которые должны отключиться при решении этой головоломки

#### Пример настройки

**Сценарий**: При решении головоломки "key_puzzle_01" должны отключиться 3 энергетических разряда.

1. В HazardManager нажмите "+" в списке Puzzle Hazard Links
2. Установите **Puzzle Id**: "key_puzzle_01"
3. В списке **Energy Discharges To Disable** нажмите "+" и перетащите:
   - EnergyDischarge_01
   - EnergyDischarge_02
   - EnergyDischarge_03

4. При решении головоломки с ID "key_puzzle_01" все три разряда автоматически отключатся

#### Автоматическое обнаружение

- HazardManager автоматически находит все EnergyDischarge на сцене при старте
- Список "All Energy Discharges" заполняется автоматически (только для просмотра)

#### Использование в коде

```csharp
// Программное добавление связи
HazardManager.Instance.AddPuzzleHazardLink("puzzle_id", listOfDischarges);

// Отключить все разряды
HazardManager.Instance.DisableAllEnergyDischarges();

// Включить все разряды
HazardManager.Instance.EnableAllEnergyDischarges();
```

#### Важные замечания

- **Puzzle Id должен совпадать** с PuzzleId в компоненте BasePuzzle головоломки
- При решении головоломки автоматически отключаются только те разряды, которые указаны в связи
- Разряды можно включать/отключать вручную через методы HazardManager
- Один разряд может быть связан с несколькими головоломками

---

## Настройка аудио

### 9.1 Создание структуры папок

1. Создайте папки:
   - `Assets/_Project/Audio/Music`
   - `Assets/_Project/Audio/SFX`
   - `Assets/_Project/Audio/Footsteps`

### 9.2 Импорт аудио файлов

1. **Музыка**:
   - Импортируйте фоновые треки в `Audio/Music`
   - Формат: .ogg или .mp3
   - Настройки импорта: Compression Format: Vorbis, Quality: 70%

2. **Звуковые эффекты**:
   - Gravity Flip Sound
   - Terminal Activation Sound
   - Item Collect Sound
   - Door Open Sound
   - Door Locked Sound
   - Puzzle Solved Sound
   - Импортируйте в `Audio/SFX`

3. **Звуки шагов**:
   - Metal Footsteps (массив клипов)
   - Concrete Footsteps (массив клипов)
   - Default Footsteps (массив клипов)
   - Импортируйте в `Audio/Footsteps`

### 9.3 Настройка AudioManager

**AudioManager** управляет всеми звуками в игре через пул AudioSource для оптимизации.

#### Настройка AudioManager

1. **Выберите AudioManager** на сцене
2. **Привяжите аудио клипы**:
   - **Background Music Tracks**: перетащите музыкальные треки (массив)
   - **Gravity Flip Sound**: перетащите звук переворота гравитации
   - **Terminal Activation Sound**: перетащите звук активации терминала
   - **Item Collect Sound**: перетащите звук сбора предмета
   - **Door Open Sound**: перетащите звук открытия двери
   - **Door Locked Sound**: перетащите звук заблокированной двери
   - **Puzzle Solved Sound**: перетащите звук решения головоломки
   - **Footstep Sounds**: перетащите звуки шагов в соответствующие поля:
     - **Metal Footstep**: один звук шагов по металлу
     - **Concrete Footstep**: один звук шагов по бетону
     - **Примечание**: Система автоматически изменяет тональность (pitch) для разнообразия, поэтому достаточно одного клипа на тип
   
   **Важно**: PlayerFootsteps автоматически использует звуки из AudioManager, поэтому настройка звуков в PlayerFootsteps не требуется!

3. **Настройте параметры**:
   - **SFX Pool Size**: 10 (количество AudioSource в пуле для звуковых эффектов)
   - **Music Volume**: 0.7 (громкость музыки, 0-1)
   - **SFX Volume**: 1.0 (громкость звуковых эффектов, 0-1)

#### Использование AudioManager в коде

**Воспроизведение звуковых эффектов (3D):**
```csharp
// Воспроизвести звук в указанной позиции (3D звук)
AudioManager.Instance.PlaySFX(audioClip, position);

// Использование предустановленных звуков
AudioManager.Instance.PlaySFX(
    AudioManager.Instance.GetItemCollectSound(), 
    transform.position
);
```

**Воспроизведение звуковых эффектов (2D):**
```csharp
// Воспроизвести звук без позиции (2D звук, например UI)
AudioManager.Instance.PlaySFX2D(audioClip);
```

**Воспроизведение фоновой музыки:**
```csharp
// Воспроизвести текущий трек
AudioManager.Instance.PlayBackgroundMusic();

// Воспроизвести конкретный трек
AudioManager.Instance.PlayBackgroundMusic(trackIndex);
```

**Управление громкостью:**
```csharp
// Установить громкость музыки (0-1)
AudioManager.Instance.SetMusicVolume(0.5f);

// Установить громкость звуковых эффектов (0-1)
AudioManager.Instance.SetSFXVolume(0.8f);
```

**Получение звуков:**
```csharp
// Получить звук сбора предмета
AudioClip collectSound = AudioManager.Instance.GetItemCollectSound();

// Получить звук открытия двери
AudioClip doorOpenSound = AudioManager.Instance.GetDoorOpenSound();

// Получить звук заблокированной двери
AudioClip doorLockedSound = AudioManager.Instance.GetDoorLockedSound();

// Получить звук решения головоломки
AudioClip puzzleSolvedSound = AudioManager.Instance.GetPuzzleSolvedSound();

// Получить звук переворота гравитации
AudioClip gravityFlipSound = AudioManager.Instance.GetGravityFlipSound();

// Получить звук активации терминала
AudioClip terminalSound = AudioManager.Instance.GetTerminalActivationSound();
```

**Получение звуков шагов:**
```csharp
// Получить массив звуков шагов для поверхности
AudioClip[] footsteps = AudioManager.Instance.GetFootstepClips("Metal");
// Доступные типы: "Metal", "Concrete", "Default"
```

**Автоматические звуки:**
AudioManager автоматически воспроизводит звуки при следующих событиях:
- `EventBus.OnGravityFlipped` → Gravity Flip Sound
- `EventBus.OnTerminalActivated` → Terminal Activation Sound
- `EventBus.OnItemCollected` → Item Collect Sound
- `EventBus.OnPuzzleSolved` → Puzzle Solved Sound

#### Пример использования в скриптах предметов

**Старый способ (deprecated):**
```csharp
[SerializeField] private AudioClip collectSound;

private void Collect()
{
    if (collectSound != null)
    {
        // Старый способ - не рекомендуется
    }
}
```

**Новый способ (рекомендуется):**
```csharp
private void Collect()
{
    // Визуальные эффекты через ObjectPoolManager
    if (ObjectPoolManager.Instance != null)
    {
        ObjectPoolManager.Instance.SpawnFromPool("ItemCollect", transform.position, Quaternion.identity);
    }

    // Звуковые эффекты через AudioManager
    if (AudioManager.Instance != null)
    {
        AudioManager.Instance.PlaySFX(
            AudioManager.Instance.GetItemCollectSound(), 
            transform.position
        );
    }
}
```

---

## Настройка VFX

### 10.1 Создание эффектов

1. **Создайте папку**: `Assets/_Project/Prefabs/VFX`

2. **Spark Effect (Искры)**:
   - GameObject > Effects > Particle System
   - Настройте:
     - Start Lifetime: 0.5-1.0
     - Start Speed: 2-5
     - Start Color: оранжевый/желтый
     - Emission: Rate over Time: 50
   - Сохраните как префаб: `SparkEffect.prefab`

3. **Energy Discharge Effect**:
   - Создайте Particle System
   - Настройте для энергетического разряда (синий/фиолетовый)
   - Сохраните как префаб: `EnergyDischargeEffect.prefab`

4. **Gravity Flip Effect**:
   - Создайте Particle System
   - Настройте для переворота гравитации
   - Сохраните как префаб: `GravityFlipEffect.prefab`

5. **Item Collect Effect**:
   - Создайте Particle System
   - Настройте для сбора предметов
   - Сохраните как префаб: `ItemCollectEffect.prefab`

### 10.2 Настройка ObjectPoolManager

См. подробную инструкцию в разделе [4.4 ObjectPoolManager](#44-objectpoolmanager).

**Важно**: Все префабы эффектов должны быть настроены в ObjectPoolManager перед использованием.

#### Добавление новых эффектов

Если вам нужно добавить новый тип эффекта:

1. **Создайте префаб эффекта**:
   - Создайте Particle System или другой визуальный эффект
   - Сохраните как префаб в `Assets/_Project/Prefabs/VFX`

2. **Добавьте пул в ObjectPoolManager**:
   - Выберите ObjectPoolManager
   - Нажмите "+" в списке Pools
   - Укажите Tag (например, "MyNewEffect")
   - Перетащите префаб
   - Установите Size (количество объектов в пуле)

3. **Используйте в коде напрямую через ObjectPoolManager**:
   ```csharp
   // Воспроизведение эффекта через ObjectPoolManager
   if (ObjectPoolManager.Instance != null)
   {
       ObjectPoolManager.Instance.SpawnFromPool("MyNewEffect", position, Quaternion.identity);
   }
   ```

#### Настройка префабов эффектов

**Для Particle System эффектов:**
- Убедитесь, что Particle System настроен правильно
- Если эффект должен автоматически возвращаться в пул, добавьте компонент с интерфейсом `IPooledObject`:
  ```csharp
  public class ParticleEffectPooled : MonoBehaviour, IPooledObject
  {
      private ParticleSystem particles;

      private void Awake()
      {
          particles = GetComponent<ParticleSystem>();
      }

      public void OnObjectSpawn()
      {
          // Перезапускаем эффект при получении из пула
          if (particles != null)
          {
              particles.Play();
          }
      }

      public void OnObjectReturn()
      {
          // Останавливаем эффект при возврате в пул
          if (particles != null)
          {
              particles.Stop();
          }
      }
  }
  ```

**Автоматический возврат в пул:**
Если эффект должен автоматически возвращаться в пул после завершения, используйте корутину:
```csharp
public class AutoReturnToPool : MonoBehaviour, IPooledObject
{
    [SerializeField] private float lifetime = 2f;
    private string poolTag = "ItemCollect"; // Тег пула

    public void OnObjectSpawn()
    {
        StartCoroutine(ReturnToPoolAfterDelay());
    }

    public void OnObjectReturn()
    {
        StopAllCoroutines();
    }

    private System.Collections.IEnumerator ReturnToPoolAfterDelay()
    {
        yield return new WaitForSeconds(lifetime);
        if (ObjectPoolManager.Instance != null)
        {
            ObjectPoolManager.Instance.ReturnToPool(poolTag, gameObject);
        }
    }
}
```

---

## Настройка оптимизации

### 11.1 Light Baking

1. **Настройте освещение**:
   - Window > Rendering > Lighting
   - Включите Baked Global Illumination
   - Настройте Lightmaps:
     - Resolution: 40-80 (в зависимости от размера уровня)
     - Lightmap Size: 1024 или 2048

2. **Настройте источники света**:
   - Выберите каждый Light
   - Mode: Baked или Mixed
   - Для статического освещения используйте Baked

3. **Пометите объекты как Static**:
   - Выберите статические объекты (стены, пол, потолок)
   - В инспекторе отметьте "Static"

4. **Запеките свет**:
   - Window > Rendering > Lighting
   - Нажмите "Generate Lighting"

### 11.2 LOD Groups

1. **Выберите сложные модели** (механизмы станции, крупные объекты)
2. **Добавьте LOD Group**:
   - Component > Rendering > LOD Group
3. **Настройте LOD уровни**:
   - LOD 0: полная модель (100%)
   - LOD 1: упрощенная модель (50%)
   - LOD 2: очень упрощенная модель (25%)
   - Culled: скрыто (0%)

### 11.3 Occlusion Culling

1. **Настройте Occlusion Culling**:
   - Window > Rendering > Occlusion Culling
   - Вкладка "Bake"
   - Настройте параметры:
     - Smallest Occluder: 0.5
     - Smallest Hole: 0.25
   - Нажмите "Bake"

2. **Пометите объекты**:
   - Статические объекты автоматически участвуют
   - Для динамических объектов используйте Occlusion Area

---

## Тестирование

### 12.1 Базовое тестирование

1. **Запустите игру**:
   - File > Build Settings
   - Добавьте сцены в правильном порядке:
     - MainMenu (индекс 0)
     - Level01_StationHub (индекс 1)
     - Level02_ReactorCore (индекс 2)
   - Нажмите Play

2. **Проверьте**:
   - Главное меню открывается
   - Кнопки работают
   - Игрок появляется на уровне
   - Движение работает (без инерции, мгновенное)
   - Горизонтальный поворот игрока работает (мышь X)
   - Вертикальное вращение камеры работает (мышь Y) с ограничениями
   - Переворот гравитации работает
   - Взаимодействие работает (E)
   - При взаимодействии с терминалом открывается Canvas в World Space
   - Canvas поворачивается к камере
   - Управление НЕ блокируется (игрок может двигаться)
   - Прицел виден в центре экрана
   - Отображаются 4 колонки с значениями (по 4 видимых значения в каждой)
   - Кнопки со стрелками отображаются под колонками
   - Нажатие на стрелки через прицел сдвигает видимую область колонки
   - При выравнивании всех единиц в одну строку головоломка решается
   - UI обновляется

### 12.2 Тестирование систем

1. **Проверьте EventBus**:
   - Откройте Console (Window > General > Console)
   - Убедитесь, что события вызываются

2. **Проверьте сохранения**:
   - Сохраните игру
   - Загрузите сохранение
   - Убедитесь, что позиция и прогресс восстанавливаются

3. **Проверьте головоломки**:
   - Решите каждую головоломку
   - Убедитесь, что события срабатывают

### 12.3 Профилирование

1. **Откройте Profiler**:
   - Window > Analysis > Profiler

2. **Проверьте производительность**:
   - CPU Usage
   - Memory Usage
   - Rendering

3. **Оптимизируйте проблемные места**

---

## Частые проблемы и решения

### Проблема: Игрок не двигается
**Решение**: 
- Проверьте, что Input System включен в Project Settings
- Убедитесь, что Input Actions правильно настроены
- Проверьте, что PlayerController получает входные данные
- Убедитесь, что Rigidbody добавлен и не является Kinematic
- Проверьте, что InputEnabled = true (может быть заблокирован терминалом)

### Проблема: Гравитация не работает
**Решение**:
- Убедитесь, что Rigidbody добавлен и Use Gravity = false
- Проверьте, что GravitySystem правильно применяет гравитацию через AddForce
- Проверьте, что isGrounded правильно определяется через Raycast
- Убедитесь, что Ground Layer правильно настроен

### Проблема: События не срабатывают
**Решение**:
- Убедитесь, что все менеджеры инициализированы
- Проверьте подписки на события в Start/Awake
- Проверьте Console на ошибки

### Проблема: UI не отображается
**Решение**:
- Убедитесь, что Canvas настроен правильно
- Проверьте, что EventSystem присутствует
- Проверьте, что UI элементы активны

### Проблема: Сохранения не работают
**Решение**:
- Проверьте права доступа к папке сохранений
- Убедитесь, что SaveData правильно сериализуется
- Проверьте путь к файлам сохранений

---

## Заключение

После выполнения всех шагов у вас должен быть полностью рабочий проект. Все системы интегрированы и готовы к использованию. Добавьте контент (модели, текстуры, аудио) и настройте уровни согласно вашей концепции.

**Важные напоминания**:
- Все менеджеры должны быть на сцене или использовать DontDestroyOnLoad
- EventBus используется для связи между системами
- Сохраняйте префабы для переиспользования
- Тестируйте каждую систему отдельно перед интеграцией

Удачи в разработке!


