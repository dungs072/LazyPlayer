public class PathNode : IHeapItem<PathNode>
{
    public int row;
    public int col;
    public int gCost = int.MaxValue;
    public int hCost;
    public int fCost => gCost + hCost;

    public PathNode parent;

    public bool opened;
    public bool closed;

    public int HeapIndex { get; set; }

    public PathNode(int r, int c)
    {
        row = r;
        col = c;
    }

    public int CompareTo(PathNode other)
    {
        int compare = fCost.CompareTo(other.fCost);
        return compare == 0 ? hCost.CompareTo(other.hCost) : compare;
    }
}
