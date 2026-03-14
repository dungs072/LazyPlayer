using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public struct CellGrid
{
    public int Row;
    public int Column;
    public int OccupiedByEntityId; // 0 means unoccupied, otherwise holds the instanceId of the occupying entity

    public CellGrid(int row, int column, int occupiedByEntityId)
    {
        Row = row;
        Column = column;
        OccupiedByEntityId = occupiedByEntityId;
    }

    public bool IsOccupied => OccupiedByEntityId != 0;
}

public class GridSystem : MonoBehaviour
{
    [SerializeField]
    private int _rowCount;

    [SerializeField]
    private int _columnCount;

    [SerializeField]
    private Vector2 _cellSize;

    [SerializeField]
    private Vector2 _spacing;

    [SerializeField]
    private bool _isDebug;

    [SerializeField]
    private Color _gridColor = Color.green;

    [SerializeField]
    private Color _textColor = Color.white;

    [SerializeField]
    private bool _showText = true;

    [SerializeField]
    private float _maxDrawDistance = 50f; // Only draw text when camera is close

    private CellGrid[][] _grid;

#if UNITY_EDITOR
    private static GUIStyle _cachedTextStyle; // Cache style to avoid creating every frame
#endif

    void Awake()
    {
        InitGrid();
        SubscribeEvents();
    }

    private void InitGrid()
    {
        _grid = new CellGrid[_rowCount][];
        for (int i = 0; i < _rowCount; i++)
        {
            _grid[i] = new CellGrid[_columnCount];
            for (int j = 0; j < _columnCount; j++)
            {
                _grid[i][j] = new CellGrid(i, j, 0);
            }
        }
    }

    private void SubscribeEvents()
    {
        QueryBus.Subscribe<GetSnapGridPositionQuery, Vector3>(query =>
            GetSnapGridPosition(query.position)
        );
        QueryBus.Subscribe<IsOverlappingGridQuery, bool>(query =>
            IsCellOccupied(query.position, query.size)
        );
        QueryBus.Subscribe<GetEntityIdAtPositionQuery, int>(query =>
            GetEntityIdAtPosition(query.position)
        );
        EventBus.Subscribe<SetOccupiedGridEvent>(SetCellOccupied);
    }

    void OnDestroy()
    {
        EventBus.Unsubscribe<SetOccupiedGridEvent>(SetCellOccupied);
    }

    void Start()
    {
        if (_isDebug)
        {
            CreateDebugger();
        }
    }

    private void CreateDebugger()
    {
        // Debug visualization is handled in OnDrawGizmos
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (!_isDebug)
            return;

        DrawGrid();
    }

    private void DrawGrid()
    {
        Gizmos.color = _gridColor;

        // Calculate grid dimensions
        float totalWidth = _columnCount * _cellSize.x + (_columnCount - 1) * _spacing.x;
        float totalHeight = _rowCount * _cellSize.y + (_rowCount - 1) * _spacing.y;

        Vector3 startPos = transform.position;

        // Draw center marker
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(startPos, 0.2f);

        Gizmos.color = _gridColor;

        // Calculate offset to center the grid
        float offsetX = -totalWidth / 2;
        float offsetY = -totalHeight / 2;

        // Draw horizontal lines (more efficient than drawing each cell separately)
        for (int row = 0; row <= _rowCount; row++)
        {
            float y = offsetY + row * (_cellSize.y + _spacing.y);
            Vector3 start = startPos + new Vector3(offsetX, y, 0);
            Vector3 end = start + new Vector3(totalWidth, 0, 0);
            Gizmos.DrawLine(start, end);
        }

        // Draw vertical lines
        for (int col = 0; col <= _columnCount; col++)
        {
            float x = offsetX + col * (_cellSize.x + _spacing.x);
            Vector3 start = startPos + new Vector3(x, offsetY, 0);
            Vector3 end = start + new Vector3(0, totalHeight, 0);
            Gizmos.DrawLine(start, end);
        }

        // Draw text labels if enabled
        if (_showText)
        {
            DrawCellTexts();
        }

        // Draw occupied cells
        if (_grid != null)
        {
            DrawOccupiedCells();
        }
    }

    private void DrawOccupiedCells()
    {
        Color occupiedColor = new Color(1f, 0f, 0f, 0.3f);
        for (int row = 0; row < _rowCount; row++)
        {
            for (int col = 0; col < _columnCount; col++)
            {
                if (!_grid[row][col].IsOccupied)
                    continue;
                Vector2 cellPos = GetCellPosition(row, col);
                Vector3 worldPos = transform.position + new Vector3(cellPos.x, cellPos.y, 0);
                Gizmos.color = occupiedColor;
                Gizmos.DrawCube(worldPos, new Vector3(_cellSize.x, _cellSize.y, 0));
            }
        }
    }

    private void DrawCellTexts()
    {
        // Check camera distance for performance
        Camera sceneCamera = SceneView.lastActiveSceneView?.camera;
        if (sceneCamera != null)
        {
            float distance = Vector3.Distance(sceneCamera.transform.position, transform.position);
            if (distance > _maxDrawDistance)
                return;
        }

        // Initialize cached style once
        if (_cachedTextStyle == null)
        {
            _cachedTextStyle = new GUIStyle();
            _cachedTextStyle.alignment = TextAnchor.MiddleCenter;
            _cachedTextStyle.fontSize = 11;
        }

        _cachedTextStyle.normal.textColor = _textColor;

        // Draw text for each cell
        for (int row = 0; row < _rowCount; row++)
        {
            for (int column = 0; column < _columnCount; column++)
            {
                Vector2 cellPos = GetCellPosition(row, column);
                Vector3 worldPos = transform.position + new Vector3(cellPos.x, cellPos.y, 0);

                string label = $"({row},{column})";
                Handles.Label(worldPos, label, _cachedTextStyle);
            }
        }
    }
#endif

    public Vector2 GetCellPosition(int row, int column)
    {
        // Calculate total grid dimensions
        float totalWidth = _columnCount * _cellSize.x + (_columnCount - 1) * _spacing.x;
        float totalHeight = _rowCount * _cellSize.y + (_rowCount - 1) * _spacing.y;

        // Calculate offset to center the grid
        float offsetX = -totalWidth / 2 + _cellSize.x / 2;
        float offsetY = -totalHeight / 2 + _cellSize.y / 2;

        // Calculate cell position from center
        float x = offsetX + column * (_cellSize.x + _spacing.x);
        float y = offsetY + row * (_cellSize.y + _spacing.y);

        return new Vector2(x, y);
    }

    public Vector3 GetSnapGridPosition(Vector3 worldPosition)
    {
        Vector3 localPos = worldPosition - transform.position;

        // Calculate total grid dimensions
        float totalWidth = _columnCount * _cellSize.x + (_columnCount - 1) * _spacing.x;
        float totalHeight = _rowCount * _cellSize.y + (_rowCount - 1) * _spacing.y;

        // First cell center in local space (matches GetCellPosition logic)
        float firstCellCenterX = -totalWidth / 2 + _cellSize.x / 2;
        float firstCellCenterY = -totalHeight / 2 + _cellSize.y / 2;

        float stepX = _cellSize.x + _spacing.x;
        float stepY = _cellSize.y + _spacing.y;

        // Find nearest cell by rounding distance from first cell center
        int column = Mathf.RoundToInt((localPos.x - firstCellCenterX) / stepX);
        int row = Mathf.RoundToInt((localPos.y - firstCellCenterY) / stepY);

        // Clamp to grid bounds
        column = Mathf.Clamp(column, 0, _columnCount - 1);
        row = Mathf.Clamp(row, 0, _rowCount - 1);

        // Get snapped position
        Vector2 cellPos = GetCellPosition(row, column);
        return transform.position + new Vector3(cellPos.x, cellPos.y, 0);
    }

    public void SetCellOccupied(SetOccupiedGridEvent e)
    {
        var position = e.position;
        var widthSize = e.size.x;
        var heightSize = e.size.y;
        // Convert world position to local position
        Vector3 localPos = position - transform.position;

        // Calculate total grid dimensions
        float totalWidth = _columnCount * _cellSize.x + (_columnCount - 1) * _spacing.x;
        float totalHeight = _rowCount * _cellSize.y + (_rowCount - 1) * _spacing.y;

        // First cell center (matches GetSnapGridPosition logic)
        float firstCellCenterX = -totalWidth / 2 + _cellSize.x / 2;
        float firstCellCenterY = -totalHeight / 2 + _cellSize.y / 2;

        float stepX = _cellSize.x + _spacing.x;
        float stepY = _cellSize.y + _spacing.y;

        // Calculate the center cell
        int centerColumn = Mathf.RoundToInt((localPos.x - firstCellCenterX) / stepX);
        int centerRow = Mathf.RoundToInt((localPos.y - firstCellCenterY) / stepY);

        // Calculate how many cells the object occupies
        int cellsWidth = Mathf.CeilToInt(widthSize / (_cellSize.x + _spacing.x));
        int cellsHeight = Mathf.CeilToInt(heightSize / (_cellSize.y + _spacing.y));

        // Calculate the range of cells to mark as occupied
        int startColumn = centerColumn - cellsWidth / 2;
        int endColumn = centerColumn + (cellsWidth - 1) / 2;
        int startRow = centerRow - cellsHeight / 2;
        int endRow = centerRow + (cellsHeight - 1) / 2;

        // Mark cells as occupied
        for (int row = startRow; row <= endRow; row++)
        {
            for (int col = startColumn; col <= endColumn; col++)
            {
                // Check if cell is within grid bounds
                if (row >= 0 && row < _rowCount && col >= 0 && col < _columnCount)
                {
                    // Create new struct with updated OccupiedByEntityId value
                    _grid[row][col] = new CellGrid(row, col, e.entityInstanceId);
                }
            }
        }
    }

    public bool IsCellOccupied(Vector3 position, Vector2 size)
    {
        Vector3 localPos = position - transform.position;

        float totalWidth = _columnCount * _cellSize.x + (_columnCount - 1) * _spacing.x;
        float totalHeight = _rowCount * _cellSize.y + (_rowCount - 1) * _spacing.y;

        float firstCellCenterX = -totalWidth / 2 + _cellSize.x / 2;
        float firstCellCenterY = -totalHeight / 2 + _cellSize.y / 2;

        float stepX = _cellSize.x + _spacing.x;
        float stepY = _cellSize.y + _spacing.y;

        int centerColumn = Mathf.RoundToInt((localPos.x - firstCellCenterX) / stepX);
        int centerRow = Mathf.RoundToInt((localPos.y - firstCellCenterY) / stepY);

        int cellsWidth = Mathf.CeilToInt(size.x / stepX);
        int cellsHeight = Mathf.CeilToInt(size.y / stepY);

        int startColumn = centerColumn - cellsWidth / 2;
        int endColumn = centerColumn + (cellsWidth - 1) / 2;
        int startRow = centerRow - cellsHeight / 2;
        int endRow = centerRow + (cellsHeight - 1) / 2;

        for (int row = startRow; row <= endRow; row++)
        {
            for (int col = startColumn; col <= endColumn; col++)
            {
                if (row < 0 || row >= _rowCount || col < 0 || col >= _columnCount)
                    return true;
                if (_grid[row][col].IsOccupied)
                    return true;
            }
        }
        return false;
    }

    public int GetEntityIdAtPosition(Vector3 position)
    {
        Vector3 localPos = position - transform.position;

        float totalWidth = _columnCount * _cellSize.x + (_columnCount - 1) * _spacing.x;
        float totalHeight = _rowCount * _cellSize.y + (_rowCount - 1) * _spacing.y;

        float firstCellCenterX = -totalWidth / 2 + _cellSize.x / 2;
        float firstCellCenterY = -totalHeight / 2 + _cellSize.y / 2;

        float stepX = _cellSize.x + _spacing.x;
        float stepY = _cellSize.y + _spacing.y;

        int column = Mathf.RoundToInt((localPos.x - firstCellCenterX) / stepX);
        int row = Mathf.RoundToInt((localPos.y - firstCellCenterY) / stepY);

        if (row >= 0 && row < _rowCount && column >= 0 && column < _columnCount)
        {
            return _grid[row][column].OccupiedByEntityId;
        }
        return 0; // Return 0 for out of bounds, meaning unoccupied
    }
}
