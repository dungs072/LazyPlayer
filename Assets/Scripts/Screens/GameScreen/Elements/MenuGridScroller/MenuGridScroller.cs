using System.Collections.Generic;
using PolyAndCode.UI;
using UnityEngine;

public class MenuGridData
{
    public int Index { get; set; }
    public Sprite Icon { get; set; }
    public string Name { get; set; }
}

public class MenuGridScroller : MonoBehaviour, IRecyclableScrollRectDataSource
{
    [SerializeField]
    private RecyclableScrollRect _recyclableScrollRect;

    private IReadOnlyList<MenuGridData> _contactList = new List<MenuGridData>();

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
        item.SetInfo(index);
    }
}
