using UnityEngine;
/// <summary>
/// This class controls all the logic of game. Etc: win, lose,..
/// </summary>
public class GamePlay : MonoBehaviour
{
    [field: SerializeField] public EntityManager EntityManager { get; private set; }
    [field: SerializeField] public CharacterManager CharacterManager { get; private set; }
    [field: SerializeField] public ResourcesManager ResourcesManager { get; private set; }
    [field: SerializeField] public MapManager MapManager { get; private set; }
    [field: SerializeField] public CameraController CameraController { get; private set; }
    [field: SerializeField] public FoodDictionary FoodDictionary { get; private set; }
    [field: SerializeField] public FoodOrderManager FoodOrderManager { get; private set; }
    [field: SerializeField] public TableOrderManager TableOrderManager { get; private set; }
    [field: SerializeField] public StaffManager StaffManager { get; private set; }
    [field: SerializeField] public BuildingSystem BuildingSystem { get; private set; }
}
