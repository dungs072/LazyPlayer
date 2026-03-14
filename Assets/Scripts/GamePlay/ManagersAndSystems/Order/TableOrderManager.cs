using System;
using System.Collections.Generic;
using UnityEngine;

public class TableOrder
{
    public Character diner;
}
public class TableOrderManager : MonoBehaviour
{
    public List<TableOrder> tableOrders = new();

    public void Initialize1()
    {
        DiningTable.OnAvailableDiningTable += HandleAvailableDiningTable;
        
        EventBus.Subscribe<AddTableOrderEvent>(ev => AddTableOrder(ev.tableOrder));
    }
    void OnDestroy()
    {
        DiningTable.OnAvailableDiningTable -= HandleAvailableDiningTable;
    }
    private void HandleAvailableDiningTable()
    {
        var order = GetOldestTableOrder();
        if (order == null) return;
        order.diner.EnqueueJob(new Diner());
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
