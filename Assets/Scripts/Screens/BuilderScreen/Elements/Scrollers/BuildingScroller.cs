using System.Collections.Generic;
using PolyAndCode.UI;
using UnityEngine;

public class BuildingScroller : MonoBehaviour, IRecyclableScrollRectDataSource
{
    [SerializeField] private RecyclableScrollRect _recyclableScrollRect;

    private IReadOnlyList<BuildableEntity> _contactList = new List<BuildableEntity>();
    public void SetData(IReadOnlyList<BuildableEntity> data)
    {
        _contactList = data;
        _recyclableScrollRect.DataSource = this;
        _recyclableScrollRect.ReloadData();
    }

    #region DATA-SOURCE

    public int GetItemCount()
    {
        return _contactList.Count;
    }
    public void SetCell(ICell cell, int index)
    {
        var item = cell as BuildingBlock;
        item.SetInfo(_contactList[index], index);
    }

    #endregion
}
