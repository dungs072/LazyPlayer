using System.Collections.Generic;

public interface IHeapItem<T> : System.IComparable<T>
{
    int HeapIndex { get; set; }
}

public class MinHeap<T>
    where T : IHeapItem<T>
{
    private List<T> items = new List<T>();

    public int Count => items.Count;

    public void Add(T item)
    {
        item.HeapIndex = items.Count;
        items.Add(item);
        SortUp(item);
    }

    public T RemoveFirst()
    {
        T firstItem = items[0];
        int lastIndex = items.Count - 1;

        items[0] = items[lastIndex];
        items[0].HeapIndex = 0;

        items.RemoveAt(lastIndex);
        SortDown(items[0]);

        return firstItem;
    }

    public void UpdateItem(T item)
    {
        SortUp(item);
    }

    private void SortUp(T item)
    {
        while (true)
        {
            int parentIndex = (item.HeapIndex - 1) / 2;
            if (parentIndex < 0)
                break;

            T parent = items[parentIndex];

            if (item.CompareTo(parent) < 0)
            {
                Swap(item, parent);
            }
            else
                break;
        }
    }

    private void SortDown(T item)
    {
        while (true)
        {
            int left = item.HeapIndex * 2 + 1;
            int right = item.HeapIndex * 2 + 2;
            int smallest = item.HeapIndex;

            if (left < items.Count && items[left].CompareTo(items[smallest]) < 0)
                smallest = left;

            if (right < items.Count && items[right].CompareTo(items[smallest]) < 0)
                smallest = right;

            if (smallest != item.HeapIndex)
                Swap(items[smallest], item);
            else
                break;
        }
    }

    private void Swap(T a, T b)
    {
        items[a.HeapIndex] = b;
        items[b.HeapIndex] = a;

        int temp = a.HeapIndex;
        a.HeapIndex = b.HeapIndex;
        b.HeapIndex = temp;
    }
}
