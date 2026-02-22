using System.Collections.Generic;
using UnityEngine;

public class TableOrder
{
    public Diner diner;
}
public class TableOrderManager : MonoBehaviour
{
    public List<TableOrder> tableOrders = new();

    void Awake()
    {
        DiningTable.OnAvailableDiningTable += HandleAvailableDiningTable;
    }
    void OnDestroy()
    {
        DiningTable.OnAvailableDiningTable -= HandleAvailableDiningTable;
    }
    private void HandleAvailableDiningTable()
    {
        var order = GetOldestTableOrder();
        if (order == null) return;
        var diner = order.diner;
        diner.DoJob();
        RemoveTableOrder(order);
    }

    public TableOrder GetOldestTableOrder()
    {
        if (tableOrders.Count > 0)
        {
            var oldestOrder = tableOrders[0];
            return oldestOrder;
        }
        return null;
    }
    public void AddTableOrder(TableOrder tableOrder)
    {
        tableOrders.Add(tableOrder);
    }

    public void RemoveTableOrder(TableOrder tableOrder)
    {
        tableOrders.Remove(tableOrder);
    }

}
