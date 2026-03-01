using BaseEngine;
using Cysharp.Threading.Tasks;
using PolyAndCode.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingBlock : MonoBehaviour, ICell
{
    [SerializeField] private Image displayImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private MagicButton buildButton;

    void Awake()
    {
        buildButton.AddListener(HandleBuildButtonClicked);
    }
    void OnDestroy()
    {
        buildButton.RemoveListener(HandleBuildButtonClicked);
    }

    public void SetInfo(BuildableEntity data, int index)
    {
        nameText.text = data.EntityName;
    }

    private async UniTask HandleBuildButtonClicked()
    {
        GamePlugin.BlockInput(true);
        EventBus.Publish(new SpawnEntityEvent
        {
            entityName = nameText.text,
        });
        await UniTask.WhenAll(new[] {
            ScreenPlugin.OpenScreenAsync<BuildingEditorScreen>(),
            ScreenPlugin.CloseScreenAsync<BuilderScreen>()
        });
        GamePlugin.BlockInput(false);
    }

}
