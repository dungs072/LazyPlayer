using UnityEngine;

public class GameManager : MonoBehaviour
{
    [field: Header("Game Logic")]
    [field: SerializeField] public EntityManager EntityManager { get; private set; }
    [field: SerializeField] public CharacterManager CharacterManager { get; private set; }
    [field: SerializeField] public ResourcesManager ResourcesManager { get; private set; }
    [field: SerializeField] public MapManager MapManager { get; private set; }
    [field: SerializeField] public CameraController CameraController { get; private set; }
    [field: SerializeField] public FoodDictionary FoodDictionary { get; private set; }
    [field: SerializeField] public FoodOrderManager FoodOrderManager { get; private set; }
    [field: SerializeField] public TableOrderManager TableOrderManager { get; private set; }
    [field: SerializeField] public StaffManager StaffManager { get; private set; }

    [field: Header("UI")]
    [field: SerializeField] public ScreensManager ScreensManager { get; private set; }


    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }
}
