using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;

public class GameLoader : MonoBehaviour
{
    [SerializeField] private List<AssetLabelReference> labels = new();
    private Dictionary<string, object> assetCache = new();
    public static GameLoader Instance => _instance;
    private static GameLoader _instance;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    async void Start()
    {
        await LoadLabels();
        await LoadScene(AddressableKeys.GAME_SCENE);
    }
    private async UniTask LoadLabels()
    {
        var uniTasks = new List<UniTask>();
        foreach (var label in labels)
        {
            uniTasks.Add(LoadLabel(label));
        }
        await UniTask.WhenAll(uniTasks);
    }
    public async UniTask LoadLabel(AssetLabelReference label)
    {
        var locationsHandle = Addressables.LoadResourceLocationsAsync(label);

        await locationsHandle.ToUniTask();

        IList<IResourceLocation> locations = locationsHandle.Result;

        foreach (var location in locations)
        {
            string key = location.PrimaryKey;

            var handle = Addressables.LoadAssetAsync<Object>(location);

            var asset = await handle.ToUniTask();

            assetCache[key] = asset;

            Debug.Log($"Loaded asset key: {key}");
        }
    }

    public async UniTask LoadScene(string sceneKey)
    {
        var handle = Addressables.LoadSceneAsync(sceneKey);

        await handle.ToUniTask();

        Debug.Log($"Scene loaded: {sceneKey}");
    }

    public T GetAsset<T>(string key) where T : Object
    {
        if (assetCache.TryGetValue(key, out var asset))
        {
            return asset as T;
        }

        Debug.LogError($"Asset not found: {key}");
        return null;
    }

}
