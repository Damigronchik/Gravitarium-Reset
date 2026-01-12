# Руководство по использованию InventoryManager в Unity

## Содержание
1. [Обзор системы инвентаря](#обзор-системы-инвентаря)
2. [Настройка InventoryManager в сцене](#настройка-inventorymanager-в-сцене)
3. [Работа с ключ-картами (KeyCard)](#работа-с-ключ-картами-keycard)
4. [Работа с энергетическими ядрами (EnergyCore)](#работа-с-энергетическими-ядрами-energycore)
5. [Примеры использования](#примеры-использования)
6. [API Reference](#api-reference)

---

## Обзор системы инвентаря

`InventoryManager` — это система управления инвентарем игрока, которая хранит собранные предметы (ключ-карты и энергетические ядра) между сценами. Система использует паттерн Singleton и автоматически создается при первом обращении.

### Особенности:
- ✅ Автоматическое создание при первом использовании
- ✅ Сохранение данных между сценами (DontDestroyOnLoad)
- ✅ Хранение уникальных ID предметов
- ✅ Простой API для проверки наличия предметов

---

## Настройка InventoryManager в сцене

### Вариант 1: Автоматическое создание (рекомендуется)

`InventoryManager` автоматически создается при первом обращении к `InventoryManager.Instance`. Никаких дополнительных действий не требуется!

```csharp
// Просто используйте InventoryManager.Instance в любом скрипте
if (InventoryManager.Instance.HasKeyCard("keycard_001"))
{
    // Ключ-карта есть в инвентаре
}
```

### Вариант 2: Ручное создание в сцене

Если вы хотите явно создать `InventoryManager` в сцене:

1. **Создайте пустой GameObject**:
   - В иерархии сцены: `Right Click > Create Empty`
   - Назовите его `InventoryManager`

2. **Добавьте компонент**:
   - Выберите созданный GameObject
   - В Inspector: `Add Component > Inventory Manager`

3. **Готово!** Система будет работать автоматически.

**Примечание**: Если `InventoryManager` уже существует в сцене, автоматическое создание не произойдет. Это предотвращает дублирование.

---

## Работа с ключ-картами (KeyCard)

### Настройка KeyCard в сцене

1. **Создайте GameObject** для ключ-карты:
   - Добавьте 3D модель или примитив (например, Cube)
   - Добавьте Collider (Box Collider или другой)

2. **Добавьте компонент KeyCard**:
   - `Add Component > Key Card`
   - Установите `Key Card Id` (например: `"keycard_001"`)

3. **Настройте визуальные эффекты** (опционально):
   - `Collect Effect` — ParticleSystem для эффекта сбора
   - `Collect Sound` — AudioClip для звука сбора
   - `Visual Object` — GameObject для визуального представления

4. **Настройте анимацию** (опционально):
   - `Rotation Speed` — скорость вращения
   - `Float Amplitude` — амплитуда плавающего движения
   - `Float Speed` — скорость плавающего движения

### Автоматический сбор

Когда игрок взаимодействует с ключ-картой (через `PlayerController`), она автоматически:
- ✅ Добавляется в инвентарь через `InventoryManager.Instance.AddKeyCard(keyCardId)`
- ✅ Вызывает событие `EventBus.InvokeKeyCardCollected`
- ✅ Отключается и уничтожается через 2 секунды

### Проверка наличия ключ-карты в коде

```csharp
using UnityEngine;

public class MyScript : MonoBehaviour
{
    void CheckKeyCard()
    {
        // Проверка наличия ключ-карты
        if (InventoryManager.Instance.HasKeyCard("keycard_001"))
        {
            Debug.Log("У игрока есть ключ-карта keycard_001!");
        }
        else
        {
            Debug.Log("Ключ-карта keycard_001 отсутствует.");
        }
    }
}
```

### Пример использования в Door

```csharp
// В скрипте Door.cs уже реализована проверка:
if (!string.IsNullOrEmpty(requiredKeyCardId))
{
    if (InventoryManager.Instance == null || 
        !InventoryManager.Instance.HasKeyCard(requiredKeyCardId))
    {
        PlayLockedSound();
        Debug.Log($"Door {doorId} is locked: key card {requiredKeyCardId} required");
        return;
    }
}
```

---

## Работа с энергетическими ядрами (EnergyCore)

### Настройка EnergyCore в сцене

1. **Создайте GameObject** для энергетического ядра:
   - Добавьте 3D модель или примитив
   - Добавьте Collider

2. **Добавьте компонент EnergyCore**:
   - `Add Component > Energy Core`
   - Установите `Core Id` (например: `"core_001"`)

3. **Настройте визуальные эффекты** (опционально):
   - `Collect Effect` — ParticleSystem
   - `Collect Sound` — AudioClip
   - `Visual Object` — GameObject

4. **Настройте анимацию** (опционально):
   - `Rotation Speed` — скорость вращения
   - `Float Amplitude` — амплитуда плавающего движения
   - `Float Speed` — скорость плавающего движения

### Автоматический сбор

Когда игрок взаимодействует с энергетическим ядром, оно автоматически:
- ✅ Добавляется в инвентарь через `InventoryManager.Instance.AddEnergyCore(coreId)`
- ✅ Вызывает событие `EventBus.InvokeEnergyCoreCollected`
- ✅ Отключается и уничтожается через 2 секунды

### Проверка наличия энергетического ядра в коде

```csharp
using UnityEngine;

public class MyScript : MonoBehaviour
{
    void CheckEnergyCore()
    {
        // Проверка наличия конкретного ядра
        if (InventoryManager.Instance.HasEnergyCore("core_001"))
        {
            Debug.Log("У игрока есть энергетическое ядро core_001!");
        }

        // Получение количества собранных ядер
        int coreCount = InventoryManager.Instance.GetEnergyCoreCount();
        Debug.Log($"Игрок собрал {coreCount} энергетических ядер");

        // Получение списка всех собранных ядер
        var collectedCores = InventoryManager.Instance.GetCollectedEnergyCores();
        foreach (string coreId in collectedCores)
        {
            Debug.Log($"Собранное ядро: {coreId}");
        }
    }
}
```

---

## Примеры использования

### Пример 1: Проверка предметов для открытия двери

```csharp
using UnityEngine;

public class CustomDoor : MonoBehaviour
{
    [SerializeField] private string requiredKeyCardId = "keycard_001";
    [SerializeField] private int requiredEnergyCores = 3;

    public void TryOpen()
    {
        // Проверяем ключ-карту
        if (!string.IsNullOrEmpty(requiredKeyCardId))
        {
            if (!InventoryManager.Instance.HasKeyCard(requiredKeyCardId))
            {
                Debug.Log("Нужна ключ-карта!");
                return;
            }
        }

        // Проверяем количество энергетических ядер
        int currentCores = InventoryManager.Instance.GetEnergyCoreCount();
        if (currentCores < requiredEnergyCores)
        {
            Debug.Log($"Нужно {requiredEnergyCores} ядер, есть только {currentCores}");
            return;
        }

        // Открываем дверь
        OpenDoor();
    }

    private void OpenDoor()
    {
        Debug.Log("Дверь открыта!");
        // Ваша логика открытия двери
    }
}
```

### Пример 2: Отображение инвентаря в UI

```csharp
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Text keyCardCountText;
    [SerializeField] private Text energyCoreCountText;
    [SerializeField] private Transform keyCardListParent;
    [SerializeField] private Transform energyCoreListParent;
    [SerializeField] private GameObject itemUIPrefab;

    void Update()
    {
        UpdateInventoryDisplay();
    }

    void UpdateInventoryDisplay()
    {
        // Обновляем счетчики
        var keyCards = InventoryManager.Instance.GetCollectedKeyCards();
        keyCardCountText.text = $"Ключ-карты: {keyCards.Count}";

        int coreCount = InventoryManager.Instance.GetEnergyCoreCount();
        energyCoreCountText.text = $"Энергетические ядра: {coreCount}";

        // Обновляем списки (если нужно)
        UpdateItemList(keyCards, keyCardListParent, "KeyCard: ");
        UpdateItemList(
            InventoryManager.Instance.GetCollectedEnergyCores(), 
            energyCoreListParent, 
            "Core: "
        );
    }

    void UpdateItemList(HashSet<string> items, Transform parent, string prefix)
    {
        // Очищаем старые элементы
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }

        // Создаем новые элементы
        foreach (string itemId in items)
        {
            GameObject itemUI = Instantiate(itemUIPrefab, parent);
            Text itemText = itemUI.GetComponentInChildren<Text>();
            if (itemText != null)
            {
                itemText.text = prefix + itemId;
            }
        }
    }
}
```

### Пример 3: Квест на сбор предметов

```csharp
using UnityEngine;

public class CollectionQuest : MonoBehaviour
{
    [SerializeField] private string questId = "collect_items";
    [SerializeField] private string[] requiredKeyCards = { "keycard_001", "keycard_002" };
    [SerializeField] private string[] requiredEnergyCores = { "core_001", "core_002", "core_003" };

    void Update()
    {
        if (IsQuestComplete())
        {
            CompleteQuest();
        }
    }

    bool IsQuestComplete()
    {
        // Проверяем ключ-карты
        foreach (string keyCardId in requiredKeyCards)
        {
            if (!InventoryManager.Instance.HasKeyCard(keyCardId))
            {
                return false;
            }
        }

        // Проверяем энергетические ядра
        foreach (string coreId in requiredEnergyCores)
        {
            if (!InventoryManager.Instance.HasEnergyCore(coreId))
            {
                return false;
            }
        }

        return true;
    }

    void CompleteQuest()
    {
        Debug.Log($"Квест {questId} выполнен!");
        EventBus.InvokeQuestCompleted(questId);
        enabled = false; // Отключаем проверку
    }
}
```

### Пример 4: Использование системы сохранений

```csharp
using UnityEngine;

public class GameSaveExample : MonoBehaviour
{
    void Update()
    {
        // Сохранение игры (F5)
        if (Input.GetKeyDown(KeyCode.F5))
        {
            SaveGame();
        }

        // Загрузка игры (F9)
        if (Input.GetKeyDown(KeyCode.F9))
        {
            LoadGame();
        }
    }

    void SaveGame()
    {
        // Сохраняем игру (инвентарь сохранится автоматически)
        SaveManager.Instance.SaveGame("autosave");
        Debug.Log("Игра сохранена! Инвентарь включен в сохранение.");
    }

    void LoadGame()
    {
        // Загружаем игру (инвентарь загрузится автоматически)
        if (SaveManager.Instance.LoadGame("autosave"))
        {
            Debug.Log("Игра загружена!");
            
            // Проверяем загруженный инвентарь
            int keyCards = InventoryManager.Instance.GetCollectedKeyCards().Count;
            int cores = InventoryManager.Instance.GetEnergyCoreCount();
            Debug.Log($"Загружено: {keyCards} ключ-карт, {cores} энергетических ядер");
        }
        else
        {
            Debug.LogWarning("Сохранение не найдено!");
        }
    }
}
```

---

## Сохранение и загрузка инвентаря

Система инвентаря автоматически интегрирована с системой сохранений игры. `SaveManager` автоматически сохраняет и загружает весь инвентарь (ключ-карты и энергетические ядра) при сохранении/загрузке игры.

### Автоматическое сохранение

При вызове `SaveManager.Instance.SaveGame()`, система автоматически сохраняет:
- ✅ Все собранные ключ-карты
- ✅ Все собранные энергетические ядра
- ✅ Позицию игрока
- ✅ Характеристики игрока (здоровье, энергия)
- ✅ Прогресс по квестам

### Автоматическая загрузка

При вызове `SaveManager.Instance.LoadGame()`, система автоматически:
- ✅ Восстанавливает все ключ-карты в инвентаре
- ✅ Восстанавливает все энергетические ядра в инвентаре
- ✅ Восстанавливает позицию игрока
- ✅ Восстанавливает характеристики игрока
- ✅ Восстанавливает прогресс по квестам

### Пример использования

```csharp
using UnityEngine;

public class SaveLoadExample : MonoBehaviour
{
    void Update()
    {
        // Сохранение игры (F5)
        if (Input.GetKeyDown(KeyCode.F5))
        {
            SaveManager.Instance.SaveGame("slot1");
            Debug.Log("Игра сохранена!");
        }

        // Загрузка игры (F9)
        if (Input.GetKeyDown(KeyCode.F9))
        {
            if (SaveManager.Instance.LoadGame("slot1"))
            {
                Debug.Log("Игра загружена!");
                // Инвентарь автоматически восстановлен
                int keyCardCount = InventoryManager.Instance.GetCollectedKeyCards().Count;
                int coreCount = InventoryManager.Instance.GetEnergyCoreCount();
                Debug.Log($"Загружено: {keyCardCount} ключ-карт, {coreCount} ядер");
            }
            else
            {
                Debug.Log("Сохранение не найдено!");
            }
        }
    }
}
```

### Ручное сохранение/загрузка инвентаря

Если вам нужно вручную сохранить или загрузить только инвентарь (без остальных данных игры):

```csharp
using UnityEngine;
using System.Collections.Generic;

public class ManualInventorySave : MonoBehaviour
{
    // Сохранение инвентаря в отдельный файл
    public void SaveInventoryToFile(string fileName)
    {
        var inventoryData = new InventorySaveData
        {
            keyCards = new List<string>(InventoryManager.Instance.GetCollectedKeyCards()),
            energyCores = new List<string>(InventoryManager.Instance.GetCollectedEnergyCores())
        };

        string json = JsonUtility.ToJson(inventoryData, true);
        string path = System.IO.Path.Combine(Application.persistentDataPath, fileName);
        System.IO.File.WriteAllText(path, json);
        Debug.Log($"Инвентарь сохранен в: {path}");
    }

    // Загрузка инвентаря из файла
    public void LoadInventoryFromFile(string fileName)
    {
        string path = System.IO.Path.Combine(Application.persistentDataPath, fileName);
        
        if (!System.IO.File.Exists(path))
        {
            Debug.LogWarning($"Файл инвентаря не найден: {path}");
            return;
        }

        string json = System.IO.File.ReadAllText(path);
        InventorySaveData inventoryData = JsonUtility.FromJson<InventorySaveData>(json);

        // Очищаем текущий инвентарь
        InventoryManager.Instance.ClearInventory();

        // Загружаем ключ-карты
        foreach (string keyCardId in inventoryData.keyCards)
        {
            InventoryManager.Instance.AddKeyCard(keyCardId);
        }

        // Загружаем энергетические ядра
        foreach (string coreId in inventoryData.energyCores)
        {
            InventoryManager.Instance.AddEnergyCore(coreId);
        }

        Debug.Log($"Инвентарь загружен: {inventoryData.keyCards.Count} ключ-карт, {inventoryData.energyCores.Count} ядер");
    }
}

[System.Serializable]
public class InventorySaveData
{
    public List<string> keyCards = new List<string>();
    public List<string> energyCores = new List<string>();
}
```

### Важные замечания

- ✅ Инвентарь сохраняется автоматически при сохранении игры
- ✅ Инвентарь загружается автоматически при загрузке игры
- ✅ При загрузке игры инвентарь сначала очищается, затем заполняется сохраненными данными
- ✅ Инвентарь сохраняется между сценами благодаря `DontDestroyOnLoad`
- ⚠️ Если вы вручную очищаете инвентарь (`ClearInventory()`), не забудьте сохранить игру, чтобы изменения сохранились

---

## API Reference

### Свойства

#### `InventoryManager.Instance`
Статическое свойство для доступа к единственному экземпляру `InventoryManager`.

```csharp
InventoryManager inventory = InventoryManager.Instance;
```

---

### Методы для работы с ключ-картами

#### `AddKeyCard(string keyCardId)`
Добавляет ключ-карту в инвентарь.

**Параметры:**
- `keyCardId` (string) — уникальный идентификатор ключ-карты

**Пример:**
```csharp
InventoryManager.Instance.AddKeyCard("keycard_001");
```

---

#### `HasKeyCard(string keyCardId) : bool`
Проверяет, есть ли ключ-карта в инвентаре.

**Параметры:**
- `keyCardId` (string) — идентификатор ключ-карты для проверки

**Возвращает:**
- `bool` — `true`, если ключ-карта есть в инвентаре, иначе `false`

**Пример:**
```csharp
if (InventoryManager.Instance.HasKeyCard("keycard_001"))
{
    Debug.Log("Ключ-карта найдена!");
}
```

---

#### `RemoveKeyCard(string keyCardId)`
Удаляет ключ-карту из инвентаря.

**Параметры:**
- `keyCardId` (string) — идентификатор ключ-карты для удаления

**Пример:**
```csharp
InventoryManager.Instance.RemoveKeyCard("keycard_001");
```

---

#### `GetCollectedKeyCards() : HashSet<string>`
Возвращает копию множества всех собранных ключ-карт.

**Возвращает:**
- `HashSet<string>` — множество ID собранных ключ-карт

**Пример:**
```csharp
var keyCards = InventoryManager.Instance.GetCollectedKeyCards();
foreach (string keyCardId in keyCards)
{
    Debug.Log($"Собранная ключ-карта: {keyCardId}");
}
```

---

### Методы для работы с энергетическими ядрами

#### `AddEnergyCore(string coreId)`
Добавляет энергетическое ядро в инвентарь.

**Параметры:**
- `coreId` (string) — уникальный идентификатор энергетического ядра

**Пример:**
```csharp
InventoryManager.Instance.AddEnergyCore("core_001");
```

---

#### `HasEnergyCore(string coreId) : bool`
Проверяет, есть ли энергетическое ядро в инвентаре.

**Параметры:**
- `coreId` (string) — идентификатор энергетического ядра для проверки

**Возвращает:**
- `bool` — `true`, если ядро есть в инвентаре, иначе `false`

**Пример:**
```csharp
if (InventoryManager.Instance.HasEnergyCore("core_001"))
{
    Debug.Log("Энергетическое ядро найдено!");
}
```

---

#### `RemoveEnergyCore(string coreId)`
Удаляет энергетическое ядро из инвентаря.

**Параметры:**
- `coreId` (string) — идентификатор энергетического ядра для удаления

**Пример:**
```csharp
InventoryManager.Instance.RemoveEnergyCore("core_001");
```

---

#### `GetEnergyCoreCount() : int`
Возвращает количество собранных энергетических ядер.

**Возвращает:**
- `int` — количество собранных ядер

**Пример:**
```csharp
int count = InventoryManager.Instance.GetEnergyCoreCount();
Debug.Log($"Собрано ядер: {count}");
```

---

#### `GetCollectedEnergyCores() : HashSet<string>`
Возвращает копию множества всех собранных энергетических ядер.

**Возвращает:**
- `HashSet<string>` — множество ID собранных ядер

**Пример:**
```csharp
var cores = InventoryManager.Instance.GetCollectedEnergyCores();
foreach (string coreId in cores)
{
    Debug.Log($"Собранное ядро: {coreId}");
}
```

---

### Утилиты

#### `ClearInventory()`
Очищает весь инвентарь (удаляет все ключ-карты и энергетические ядра).

**Пример:**
```csharp
InventoryManager.Instance.ClearInventory();
Debug.Log("Инвентарь очищен");
```

---

## Важные замечания

### ID предметов
- ✅ Используйте уникальные ID для каждого предмета
- ✅ Рекомендуется использовать формат: `"keycard_001"`, `"core_001"` и т.д.
- ✅ ID чувствительны к регистру: `"KeyCard_001"` ≠ `"keycard_001"`

### Автоматическое добавление
- ✅ `KeyCard` и `EnergyCore` автоматически добавляются в инвентарь при сборе
- ✅ Не нужно вручную вызывать `AddKeyCard()` или `AddEnergyCore()` при сборе предметов
- ✅ Ручное добавление используется только для загрузки сохранений или тестирования

### Сохранение между сценами
- ✅ `InventoryManager` использует `DontDestroyOnLoad`, поэтому инвентарь сохраняется между сценами
- ✅ При переходе между сценами все собранные предметы остаются в инвентаре

### Производительность
- ✅ Использование `HashSet<string>` обеспечивает быструю проверку наличия предметов (O(1))
- ✅ Система оптимизирована для частых проверок наличия предметов

---

## Часто задаваемые вопросы

**Q: Нужно ли создавать InventoryManager в каждой сцене?**  
A: Нет, система автоматически создает его при первом использовании. Но вы можете создать его вручную в одной из сцен для явного контроля.

**Q: Сохраняется ли инвентарь между сценами?**  
A: Да, `InventoryManager` использует `DontDestroyOnLoad`, поэтому инвентарь сохраняется при переходе между сценами.

**Q: Как очистить инвентарь при начале новой игры?**  
A: Вызовите `InventoryManager.Instance.ClearInventory()` при старте новой игры.

**Q: Можно ли использовать одинаковые ID для разных типов предметов?**  
A: Технически да, но не рекомендуется. Используйте уникальные ID для каждого предмета.

**Q: Как проверить, существует ли InventoryManager?**  
A: Проверьте `InventoryManager.Instance != null`. Если он не существует, система автоматически создаст его.

---

## Поддержка

Если у вас возникли вопросы или проблемы с использованием `InventoryManager`, проверьте:
1. Правильность ID предметов
2. Наличие компонентов `KeyCard` или `EnergyCore` на объектах
3. Настройку коллайдеров для взаимодействия
4. Логи в консоли Unity для отладки

---

*Последнее обновление: 2024*

