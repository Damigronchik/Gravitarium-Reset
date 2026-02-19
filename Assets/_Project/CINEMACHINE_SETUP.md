# Настройка Cinemachine для Gravitarium

## Предварительные требования

1. **Установите Cinemachine** через Package Manager:
   - `Window > Package Manager > Unity Registry > Cinemachine`
   - Установите последнюю версию

2. **Символ компиляции** (опционально):
   - Если код не компилируется, добавьте символ `CINEMACHINE_EXISTS`:
   - `Edit > Project Settings > Player > Other Settings > Scripting Define Symbols`
   - Добавьте: `CINEMACHINE_EXISTS`
   - **Примечание**: Обычно Unity автоматически определяет наличие Cinemachine

## Иерархия объектов в сцене

Для корректной работы переворота гравитации с Cinemachine используйте следующую иерархию:

```
Scene Root
├── Player (GameObject с PlayerController, GravitySystem, Rigidbody)
│   └── (Другие компоненты игрока)
│
├── CameraPivot (Empty GameObject)
│   └── Position: (0, 1.6, 0) относительно Player
│   └── Rotation: (0, 0, 0)
│
├── CM vcam Player (Cinemachine Virtual Camera)
│   ├── Follow: CameraPivot
│   ├── Look At: CameraPivot (или Player)
│   ├── Body: Do Nothing (или 3rd Person Follow)
│   └── Aim: POV (Point of View)
│
└── Main Camera (Camera с CinemachineBrain)
    └── CinemachineBrain компонент
        └── Default Blend: Cut
```

## Автоматическая настройка (Рекомендуется)

Самый простой способ - использовать автоматическую настройку:

1. **Добавьте компонент `CinemachineAutoSetup`** на любой GameObject в сцене (например, на пустой объект "CameraSetup" или на Player)
2. В инспекторе `CinemachineAutoSetup`:
   - ✅ **Auto Setup On Start**: включено (по умолчанию)
   - ✅ **Create Camera Pivot**: включено
   - ✅ **Create Virtual Camera**: включено
   - ✅ **Setup Cinemachine Brain**: включено
3. Запустите игру - все настроится автоматически!

**Примечание**: Автоматическая настройка запускается:
- При старте сцены (Start)
- При загрузке уровня (OnLevelLoaded событие)

---

## Ручная настройка (Альтернатива)

Если вы хотите настроить все вручную:

## Пошаговая настройка

### Шаг 1: Подготовка Player

1. Убедитесь, что на объекте **Player** есть компоненты:
   - `PlayerController`
   - `GravitySystem`
   - `Rigidbody`
   - `CinemachineCameraController` (добавьте вручную)

2. В инспекторе **PlayerController**:
   - ✅ Включите `Use Cinemachine`
   - Перетащите компонент `CinemachineCameraController` в поле `Cinemachine Controller`
   - Оставьте `Camera Transform` пустым (не используется при Cinemachine)

### Шаг 2: Создание CameraPivot

1. Создайте пустой GameObject: `GameObject > Create Empty`
2. Назовите его **CameraPivot**
3. Сделайте его дочерним объектом **Player**
4. Установите позицию:
   - **Local Position**: `(0, 1.6, 0)` - на уровне глаз игрока
   - **Local Rotation**: `(0, 0, 0)`

**Важно**: CameraPivot будет следовать за игроком и вращаться вместе с ним при перевороте гравитации.

### Шаг 3: Настройка Cinemachine Virtual Camera

1. Создайте Virtual Camera: `GameObject > Cinemachine > Virtual Camera`
2. Назовите его **CM vcam Player**
3. В инспекторе Virtual Camera настройте:

   **General:**
   - **Priority**: 10 (или выше, чем у других камер)
   - **Follow**: Перетащите **CameraPivot**
   - **Look At**: Перетащите **CameraPivot** (или оставьте пустым)

   **Body:**
   - **Body**: `Do Nothing` (рекомендуется) или `3rd Person Follow`
     - Если используете `3rd Person Follow`:
       - **Damping**: `(0, 0, 0)` - для мгновенного следования
       - **Shoulder Offset**: `(0, 0, 0)`
       - **Camera Distance**: `0` - камера на позиции CameraPivot

   **Aim:**
   - **Aim**: `POV (Point of View)`
   - **Horizontal Axis:**
     - **Value**: 0
     - **Speed**: 0 (управляется через PlayerController)
     - **Input Axis Name**: оставьте пустым
   - **Vertical Axis:**
     - **Value**: 0
     - **Speed**: 0 (управляется через PlayerController)
     - **Input Axis Name**: оставьте пустым
     - **Min Value**: -80 (соответствует Vertical Look Limit)
     - **Max Value**: 80

### Шаг 4: Настройка Main Camera

1. Найдите **Main Camera** в сцене
2. Убедитесь, что на ней есть компонент **CinemachineBrain**
   - Если нет: `Component > Cinemachine > Cinemachine Brain`
3. В инспекторе CinemachineBrain:
   - **Default Blend**: `Cut` (мгновенный переход)
   - **Update Method**: `Smart Update` (рекомендуется)

### Шаг 5: Настройка CinemachineCameraController

1. На объекте **Player** найдите компонент **CinemachineCameraController**
2. В инспекторе:
   - **Virtual Camera Component**: Перетащите компонент `CinemachineVirtualCamera` с объекта **CM vcam Player**
   - **Mouse Sensitivity**: `2` (или как в PlayerController)
   - **Vertical Look Limit**: `80` (должно совпадать с PlayerController)

### Шаг 6: Проверка настроек

1. Запустите игру
2. Проверьте:
   - ✅ Камера следует за игроком
   - ✅ Мышь управляет камерой (горизонтально - поворот игрока, вертикально - наклон камеры)
   - ✅ При перевороте гравитации:
     - Игрок плавно переворачивается (1 секунда)
     - После завершения анимации камера мгновенно переворачивается
     - Управление камерой остается интуитивным

## Альтернативная иерархия (без CameraPivot)

Если вы хотите использовать камеру, которая не вращается вместе с игроком:

```
Scene Root
├── Player
│
├── CM vcam Player
│   ├── Follow: Player
│   ├── Look At: Player
│   ├── Body: 3rd Person Follow
│   └── Aim: POV
│
└── Main Camera (с CinemachineBrain)
```

**Примечание**: В этом случае камера будет следовать за игроком, но не будет вращаться вместе с ним при перевороте гравитации. Переворот камеры все равно будет работать через CinemachineCameraController.

## Использование CinemachineAutoSetup

### Базовое использование

1. Создайте пустой GameObject в сцене (например, "CameraSetup")
2. Добавьте компонент `CinemachineAutoSetup`
3. Оставьте настройки по умолчанию
4. Запустите игру - все настроится автоматически!

### Настройка параметров

В инспекторе `CinemachineAutoSetup` вы можете настроить:

- **Auto Setup On Start**: Автоматически настраивать при старте/загрузке уровня
- **Create Camera Pivot**: Создавать CameraPivot, если не существует
- **Create Virtual Camera**: Создавать Virtual Camera, если не существует
- **Setup Cinemachine Brain**: Добавлять CinemachineBrain на Main Camera
- **Camera Pivot Offset**: Позиция CameraPivot относительно Player (по умолчанию: 0, 1.6, 0)
- **Virtual Camera Priority**: Приоритет Virtual Camera (по умолчанию: 10)
- **Vertical Look Limit**: Ограничение вертикального обзора (по умолчанию: 80)

### Что создается автоматически

При автоматической настройке создаются/настраиваются:

1. **CameraPivot** (дочерний объект Player)
   - Позиция: (0, 1.6, 0) относительно Player
   - Используется как Follow и Look At для Virtual Camera

2. **CM vcam Player** (Cinemachine Virtual Camera)
   - Follow: CameraPivot
   - Look At: CameraPivot
   - Body: Transposer (с нулевым offset)
   - Aim: POV (Point of View)
   - Priority: 10

3. **CinemachineBrain** (на Main Camera)
   - Default Blend: Cut (мгновенный переход)

4. **CinemachineCameraController** (на Player)
   - Автоматически добавляется, если не существует
   - Настраивается для работы с Virtual Camera

5. **PlayerController**
   - Автоматически включается `Use Cinemachine`
   - Привязывается к `CinemachineCameraController`

### Ручной запуск настройки

Вы можете запустить настройку вручную:

1. В инспекторе: Правый клик на компоненте > **Setup Cinemachine Camera**
2. Из кода: `GetComponent<CinemachineAutoSetup>().SetupCinemachineCamera();`

### Очистка (для тестирования)

Для удаления созданных объектов:

1. В инспекторе: Правый клик на компоненте > **Cleanup Auto Setup**
2. Из кода: `GetComponent<CinemachineAutoSetup>().CleanupAutoSetup();`

**Внимание**: Очистка удаляет только объекты, созданные автоматически (с именами "CameraPivot" и "CM vcam Player").

---

## Решение проблем

### Камера не следует за игроком
- Проверьте, что **Follow** в Virtual Camera указывает на правильный объект
- Убедитесь, что **CinemachineBrain** активен на Main Camera
- Проверьте **Priority** Virtual Camera (должна быть выше других камер)

### Камера не управляется мышью
- Убедитесь, что `Use Cinemachine` включен в PlayerController
- Проверьте, что CinemachineCameraController правильно настроен
- Убедитесь, что **Aim** в Virtual Camera установлен на **POV**

### Переворот камеры не работает
- Проверьте, что CinemachineCameraController добавлен на Player
- Убедитесь, что Virtual Camera правильно назначен в CinemachineCameraController
- Проверьте, что **Aim** использует **POV** компонент

### Камера переворачивается во время анимации
- Это нормально, если вы используете старую версию кода
- Обновите код до последней версии, где переворот происходит после анимации

## Дополнительные настройки

### Настройка чувствительности мыши
- Измените **Mouse Sensitivity** в PlayerController
- Измените **Mouse Sensitivity** в CinemachineCameraController (должны совпадать)

### Настройка ограничений вертикального обзора
- Измените **Vertical Look Limit** в PlayerController
- Измените **Vertical Look Limit** в CinemachineCameraController (должны совпадать)
- Обновите **Min/Max Value** в **Vertical Axis** Virtual Camera

### Добавление эффектов камеры
- Добавьте **Cinemachine Post Processing** на Virtual Camera для эффектов
- Используйте **Cinemachine Collider** для предотвращения прохождения камеры через стены
- Используйте **Cinemachine Confiner** для ограничения области камеры

