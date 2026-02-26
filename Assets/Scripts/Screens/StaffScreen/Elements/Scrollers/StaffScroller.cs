using System.Collections.Generic;
using PolyAndCode.UI;
using UnityEngine;

public class StaffScroller : MonoBehaviour, IRecyclableScrollRectDataSource
{
    [SerializeField] private RecyclableScrollRect _recyclableScrollRect;

    private List<CharacterData> _contactList = new();
    public void SetData(List<CharacterData> data)
    {
        _contactList = data;
        _recyclableScrollRect.ReloadData();
    }

    #region DATA-SOURCE

    public int GetItemCount()
    {
        return _contactList.Count;
    }
    public void SetCell(ICell cell, int index)
    {
        var item = cell as StaffBlock;
        item.SetInfo(_contactList[index], index);
    }

    #endregion
}
