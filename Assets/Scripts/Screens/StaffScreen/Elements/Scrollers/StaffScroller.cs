using System.Collections.Generic;
using PolyAndCode.UI;
using UnityEngine;

public class StaffScroller : MonoBehaviour, IRecyclableScrollRectDataSource
{
    [SerializeField]
    RecyclableScrollRect _recyclableScrollRect;

    [SerializeField]
    private int _dataLength;

    //Dummy data List
    private List<CharacterData> _contactList = new();

    //Recyclable scroll rect's data source must be assigned in Awake.
    private void Awake()
    {
        InitData();
        _recyclableScrollRect.DataSource = this;
    }

    //Initialising _contactList with dummy data 
    private void InitData()
    {
        if (_contactList != null) _contactList.Clear();
        for (int i = 0; i < _dataLength; i++)
        {
            CharacterData obj = new CharacterData(i + "_Name", Random.Range(18, 60), Random.Range(0, 100), (Sex)Random.Range(1, 3));
            _contactList.Add(obj);
        }
    }

    #region DATA-SOURCE

    /// <summary>
    /// Data source method. return the list length.
    /// </summary>
    public int GetItemCount()
    {
        return _contactList.Count;
    }

    /// <summary>
    /// Data source method. Called for a cell every time it is recycled.
    /// Implement this method to do the necessary cell configuration.
    /// </summary>
    public void SetCell(ICell cell, int index)
    {
        //Casting to the implemented Cell
        var item = cell as StaffBlock;
        item.SetInfo(_contactList[index], index);
    }

    #endregion
}
