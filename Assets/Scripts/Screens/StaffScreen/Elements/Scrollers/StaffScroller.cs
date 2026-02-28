using System.Collections.Generic;
using PolyAndCode.UI;
using UnityEngine;

public class StaffScroller : MonoBehaviour, IRecyclableScrollRectDataSource
{
    [SerializeField] private RecyclableScrollRect _recyclableScrollRect;

    private IReadOnlyList<CharacterData> _contactList = new List<CharacterData>();
    public void SetData(IReadOnlyList<CharacterData> data)
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
        var item = cell as StaffBlock;
        item.SetInfo(_contactList[index], index);
    }

    #endregion
}
