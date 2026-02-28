using Cysharp.Threading.Tasks;
using UnityEngine;


public class StaffScreen : BaseScreen
{
    [SerializeField] private StaffScreenView view = new();
    void Awake()
    {
        view.closeButton.AddListener(HandleCloseButtonClicked);
    }
    void OnDestroy()
    {
        view.closeButton.RemoveListener(HandleCloseButtonClicked);
    }
    public override void PrepareData()
    {
        var data = GetScreenData<StaffScreenData>();
        if (data == null)
        {
            Debug.LogError("StaffScreen: No data found");
            return;
        }
        view.scroller.SetData(data.dataList);
    }
    private async UniTask HandleCloseButtonClicked()
    {
        GamePlugin.BlockInput(true);
        await ScreenPlugin.CloseScreenAsync<StaffScreen>();
        GamePlugin.BlockInput(false);
    }
}
