using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PolyAndCode.UI;
using UnityEngine;

//If you want to add list of gridbutton,
//Make sure add the _LIST at the postfix of the enum
public enum MenuGridType
{
    BUILD = 0,
    EDIT = 1,
    DESTROY = 2,
    BUILD_BACK = 3,
    BUILDING_LIST = 4,
}

public class MenuGridData
{
    public MenuGridType Type { get; set; }
    public Sprite Icon { get; set; }
    public string Name { get; set; }
}

public class MenuGridScroller : MonoBehaviour, IRecyclableScrollRectDataSource
{
    public event Func<MenuGridData, UniTask> OnGridBlockItemClicked;

    [SerializeField]
    private RecyclableScrollRect _recyclableScrollRect;

    private IReadOnlyList<MenuGridData> _contactList = new List<MenuGridData>();

    void Awake()
    {
        _recyclableScrollRect.OnCellCreated += HandleCellCreated;
    }

    void OnDestroy()
    {
        _recyclableScrollRect.OnCellCreated -= HandleCellCreated;
    }

    private void HandleCellCreated(ICell cell)
    {
        var gridBlock = cell as GridBlock;
        gridBlock.AddListener(() =>
        {
            OnGridBlockItemClicked?.Invoke(gridBlock.Data);
        });
    }

    public void SetData(IReadOnlyList<MenuGridData> data)
    {
        _contactList = data;
        _recyclableScrollRect.DataSource = this;
        _recyclableScrollRect.ReloadData();
    }

    public int GetItemCount()
    {
        return _contactList.Count;
    }

    public void SetCell(ICell cell, int index)
    {
        var item = cell as GridBlock;
        var data = _contactList[index];
        item.SetInfo(data);
    }
}
