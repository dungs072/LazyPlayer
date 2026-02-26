using UnityEngine;


public class StaffScreen : BaseScreen
{
    [SerializeField] private StaffScreenView view = new();

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
}
