# C# Coding Style Guide

Coding conventions for the **LazyPlayer** Unity project.

---

## Naming Conventions

| Element | Style | Example |
|---|---|---|
| Classes / Structs | PascalCase | `GameManager`, `MouseClickEvent` |
| Methods | PascalCase | `PrepareData()`, `OpenAsync()` |
| Event handlers | `Handle` prefix | `HandleManageButtonClicked()` |
| Query methods | `Get` prefix | `GetScreenData<T>()` |
| Private fields | camelCase | `private float originalWidth` |
| Protected fields | camelCase | `protected ScreenData screenData` |
| Public auto-properties | PascalCase with abbreviation | `public GamePlay GP { get; private set; }` |
| Constants | UPPER_SNAKE_CASE | `public const string DEFAULT = "Default"` |
| Enum values | UPPER_SNAKE_CASE | `MANAGEMENT, SHOP, GALLERY` |
| Event types | PascalCase + `Event` suffix | `ResourceAmountChangedEvent` |
| Query types | PascalCase + `Query` suffix | `GetCenterCameraPositionQuery` |

---

## Namespaces

Update more in future

## Access Modifiers & Fields

- Default to **private** for fields.
- Use `[SerializeField]` to expose private fields to the Inspector.
- Use `[field: SerializeField]` for auto-properties with a private setter.
- Group serialized fields with `[Header("...")]` attributes.

```csharp
[field: SerializeField]
public GamePlay GamePlay { get; private set; }

[Header("Tab 1")]
[SerializeField]
private MagicButtonWithIcon manageButton;

[Header("Tab 2")]
[SerializeField]
private MagicButtonWithIcon staffButton;
```

---

## Code Formatting

- **Indentation**: 4 spaces (no tabs).
- **Braces**: Allman style — opening brace on its own line.
- **Trailing commas** in multi-line initializers and enum values.
- Spaces around operators and after commas.

```csharp
public enum ButtonTab1Type
{
    MANAGEMENT,
    SHOP,
    GALLERY,
    SETTING,
}

tab1Buttons = new List<MagicButtonWithIcon>
{
    manageButton,
    shopButton,
    galleryButton,
    settingButton,
};
```

---

## Async / UniTask

Use **UniTask** (not `Task`) for all async operations.

```csharp
// Async method
public async UniTask OpenAsync(ScreenData screenData = null)
{
    PrepareData();
    PrepareFadeIn();
    await FadeInAsync();
}

// Fire-and-forget
ScreenPlugin.OpenScreenAsync<GameScreen>(data).Forget();

// Parallel await
await UniTask.WhenAll(new UniTask[] { task1, task2, task3 });

// Frame skip
await UniTask.NextFrame();
```

---

## Event & Query Bus

### Events

- Define events as **structs** (stack-allocated, no GC pressure).
- Use `EventBus.Subscribe` / `EventBus.Unsubscribe` for sync events.
- Use `EventBus.SubscribeAsync` / `EventBus.UnsubscribeAsync` for async events.
- Always **unsubscribe in `OnDestroy`**.

```csharp
// Define
public struct AddResourceEvent
{
    public InventoryItemId inventoryItemId;
    public int amount;

    public AddResourceEvent(InventoryItemId inventoryItemId, int amount)
    {
        this.inventoryItemId = inventoryItemId;
        this.amount = amount;
    }
}

// Subscribe
public override void Initialize1()
{
    EventBus.Subscribe<ResourceAmountChangedEvent>(view.SetResourcesAmount);
}

// Unsubscribe
private void OnDestroy()
{
    EventBus.Unsubscribe<ResourceAmountChangedEvent>(view.SetResourcesAmount);
}
```

### Queries

- Define query structs implementing `IQueryResult<TResult>`.
- Use `QueryBus.Subscribe` to register handlers, `QueryBus.Query` to invoke.

```csharp
public struct GetStaffDataListQuery : IQueryResult<IReadOnlyList<CharacterData>> { }

// Subscribe
QueryBus.Subscribe<GetStaffDataListQuery, IReadOnlyList<CharacterData>>(q => GetStaffDataList());

// Query
var staffList = QueryBus.Query(new GetStaffDataListQuery());
```

---

## Screen Architecture

### Lifecycle

Screens inherit from `BaseScreen` and follow a template-method lifecycle:

```
Initialize1()  →  PrepareData()  →  PrepareFadeIn()  →  FadeInAsync()  →  FadeOutAsync()
```

```csharp
public class GameScreen : BaseScreen
{
    public override void Initialize1()
    {
        // Subscribe events, wire up buttons
    }

    public override void PrepareData()
    {
        // Set up data before screen opens
    }

    public override void PrepareFadeIn()
    {
        // Prepare visual state
    }

    public override async UniTask FadeInAsync()
    {
        await view.FadeInAsync();
    }

    public override UniTask FadeOutAsync()
    {
        return UniTask.CompletedTask;
    }
}
```

### Two-Phase Initialization

```csharp
// Phase 1 — Awake: Setup references and subscriptions
GamePlay.Initialize1();
ScreensManager.Initialize1();

// Phase 2 — Start: Runtime logic
GamePlay.Initialize2();
ScreensManager.OpenScreenAsync<GameScreen>(data).Forget();
```

---

## Serializable Data Classes

Use `[Serializable]` for nested classes that appear in the Inspector:

```csharp
[Serializable]
public class ButtonTab2Panel
{
    public ButtonTab1Type parentTab1Type;
    public GameObject panel;
}
```

Use `ScreenData` as the base class for data passed between screens:

```csharp
public class GameScreenData : ScreenData
{
    public IReadOnlyList<MenuGridData> dataList;
}
```

---

## Comments

| Prefix | Purpose | Example |
|---|---|---|
| `///` | XML doc comments for public APIs | `/// <summary>Prepare screen data</summary>` |
| `//!` | Critical notes / warnings | `//! Must check` |
| `//TODO` | Planned changes | `//TODO move to subscriber file` |
| `//?` | Assumptions or caveats | `//? Only works if pivot is center` |
| `#region` | Group related methods | `#region Animations ... #endregion` |
| `// Section label` | Inline field grouping | `//Runtime state`, `//UI data` |

---

## File & Folder Organization

```
Assets/Scripts/
├── GameManager.cs
├── Screens/
│   ├── BaseScreen.cs
│   ├── ScreensManager.cs
│   ├── ScreenData.cs
│   ├── Data/
│   │   └── GameScreenData.cs
│   └── GameScreen/
│       ├── GameScreen.cs
│       ├── GameScreenView.cs
│       └── Elements/
├── GamePlay/
│   ├── GamePlay.cs
│   ├── ManagersAndSystems/
│   └── GameObjects/
├── Communications/
│   ├── EventBus/
│   └── QueryBus/
├── GamePlugin/
├── Constants/
├── Utils/
├── Loader/
└── Editors/
```

- One primary class per file (small helper types like enums or `[Serializable]` classes may live alongside the class that uses them).
- Folder name matches the feature or screen name.
- Screens get their own subfolder with `Elements/` for UI sub-components.

---

## Patterns Summary

| Pattern | Usage |
|---|---|
| EventBus (static, generic) | Decoupled communication between systems |
| QueryBus (static, generic) | Request-response across systems |
| Template Method (`BaseScreen`) | Consistent screen lifecycle |
| Singleton (`GameLoader`) | Scene-persistent services |
| Service Wrapper (`ScreenPlugin`, `GamePlugin`) | Static access to MonoBehaviour services |
| Constructor Injection | Wiring service wrappers |
| Struct Events | Zero-allocation event payloads |
