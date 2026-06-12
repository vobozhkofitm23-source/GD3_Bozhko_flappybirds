# Coding Guidelines

## Namespace

- Simple namespace matching folder name
- No nested namespaces (e.g., `ChickenHunt`, not `Games.ChickenHunt`)

```csharp
namespace ChickenHunt
{
    public class Chicken : MonoBehaviour { }
}
```

---

## Naming Conventions

### Fields

| Type | Prefix | Example |
|------|--------|---------|
| Private serialized | `_` | `[SerializeField] private float _moveSpeed;` |
| Private non-serialized | `_` | `private float _timer;` |
| Public | none | `public bool IsAlive => _isAlive;` |

### Methods

- PascalCase for all methods
- Private methods: `UpdateMovement()`, `HandleInput()`
- Public methods: `Initialize()`, `TakeDamage()`, `Die()`

### Events

- C# events with `Action<T>`
- Prefix with `On`: `OnDeath`, `OnSliced`, `OnWaveStart`

```csharp
public event Action<int> OnDeath;
```

### Properties

- PascalCase, expression-bodied when simple

```csharp
public bool IsAlive => _currentHealth > 0;
public float Range => _range;
```

---

## Class Structure Order

1. **Serialized fields** (grouped by `[Header]`)
2. **Private fields**
3. **Events**
4. **Properties**
5. **Unity lifecycle** (`Awake`, `Start`, `Update`, `OnDestroy`)
6. **Public methods**
7. **Private methods**
8. **Unity callbacks** (`OnTriggerEnter2D`, `OnCollisionEnter2D`)
9. **Gizmos** (`OnDrawGizmos`, `OnDrawGizmosSelected`)

---

## Serialized Fields

### Grouping with Headers

```csharp
[Header("Movement")]
[SerializeField] private float _moveSpeed = 5f;
[SerializeField] private float _rotationSpeed = 10f;

[Header("Health")]
[SerializeField] private int _maxHealth = 100;
[SerializeField] private Slider _hpSlider;

[Header("UI")]
[SerializeField] private TextMeshProUGUI _scoreText;
[SerializeField] private GameObject _gameOverPanel;
```

### Default Values

- Always provide sensible defaults

```csharp
[SerializeField] private float _moveSpeed = 5f;
[SerializeField] private int _maxHealth = 100;
[SerializeField] private float _minSpawnTime = 1f;
[SerializeField] private float _maxSpawnTime = 3f;
```

---

## Events & Communication

### Use C# Events (not UnityEvents)

```csharp
// Declaration
public event Action<int> OnDeath;

// Invocation
OnDeath?.Invoke(_points);

// Subscription
enemy.OnDeath += OnEnemyDeath;

// Unsubscription (in OnDestroy)
enemy.OnDeath -= OnEnemyDeath;
```

### Direct UI Serialization

- Serialize UI components directly, update in methods

```csharp
[SerializeField] private TextMeshProUGUI _scoreText;

private void UpdateScoreUI()
{
    if (_scoreText != null)
        _scoreText.text = $"Score: {_score}";
}
```

---

## Null Checks

- Always null-check serialized references before use

```csharp
if (_spriteRenderer != null)
    _spriteRenderer.flipX = direction.x < 0;

if (_gameOverPanel != null)
    _gameOverPanel.SetActive(true);
```

---

## Architecture Patterns

### Entity Pattern

Entities (Chicken, Fruit, Enemy) have:
- `Initialize()` method
- State properties (`IsAlive`, `IsSliced`)
- Events for death/destruction (`OnDeath`, `OnSliced`)
- Self-contained behavior

```csharp
public class Enemy : MonoBehaviour
{
    public event Action<int> OnDeath;
    public bool IsAlive => _currentHealth > 0;
    
    public void Initialize(Transform player) { }
    public void TakeDamage(float damage) { }
}
```

### SpawnPoint Pattern

SpawnPoints hold prefabs and return spawned entities:

```csharp
public class SpawnPoint : MonoBehaviour
{
    [SerializeField] private GameObject[] _prefabs;
    
    public Enemy Spawn()
    {
        // Instantiate, Initialize, return
    }
}
```

### Manager Pattern

Managers handle:
- SpawnPoint references
- Active entity tracking
- Score/UI updates
- Event subscriptions

```csharp
public class GameManager : MonoBehaviour
{
    [SerializeField] private SpawnPoint[] _spawnPoints;
    [SerializeField] private TextMeshProUGUI _scoreText;
    
    private readonly List<Enemy> _activeEnemies = new();
}
```

---

## Code Style

### No Regions

Do not use `#region` blocks.

### Braces

- Same line for single statements (optional)
- Always use braces for multi-line

```csharp
// OK for single line
if (_player == null) return;

// Use braces for multi-line
if (_player != null)
{
    _player.OnDeath += GameOver;
    _activePlayer = _player;
}
```

### Early Returns

Use early returns to reduce nesting:

```csharp
private void Update()
{
    if (_isGameOver) return;
    if (_player == null) return;
    
    UpdateSpawning();
}
```

### String Interpolation

Use `$""` for string formatting:

```csharp
_scoreText.text = $"Score: {_score}";
_ammoText.text = $"{_currentAmmo}/{_maxAmmo}";
```

---

## File Naming

| Type | Convention | Example |
|------|------------|---------|
| Entity | Singular noun | `Enemy.cs`, `Fruit.cs`, `Tower.cs` |
| Manager | `[Thing]Manager` or `GameManager` | `GameManager.cs`, `FruitsManager.cs` |
| Spawner | `SpawnPoint` | `SpawnPoint.cs` |
| Player components | `Player.cs`, `Weapon.cs`, `Laser.cs` | |
| Projectiles | `Projectile.cs` | |
| UI helpers | Descriptive | `ClickIndicator.cs`, `FruitHalf.cs` |

---

## Folder Structure

```
Assets/Scripts/
├── GameName/
│   ├── Player.cs
│   ├── Enemy.cs
│   ├── SpawnPoint.cs
│   ├── GameManager.cs
│   └── Projectile.cs
```

Each game in its own folder with simple namespace matching folder name.
