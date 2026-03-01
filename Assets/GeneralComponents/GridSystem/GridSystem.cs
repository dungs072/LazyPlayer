using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GridSystem : MonoBehaviour
{
    [SerializeField] private int _rowCount;
    [SerializeField] private int _columnCount;
    [SerializeField] private Vector2 _cellSize;
    [SerializeField] private Vector2 _spacing;

    [SerializeField] private bool _isDebug;
    [SerializeField] private Color _gridColor = Color.green;
    [SerializeField] private Color _textColor = Color.white;
    [SerializeField] private bool _showText = true;
    [SerializeField] private float _maxDrawDistance = 50f; // Only draw text when camera is close

    private int[][] _grid;

#if UNITY_EDITOR
    private static GUIStyle _cachedTextStyle; // Cache style to avoid creating every frame
#endif

    void Awake()
    {
        _grid = new int[_rowCount][];
        for (int i = 0; i < _rowCount; i++)
        {
            _grid[i] = new int[_columnCount];
        }

        QueryBus.Subscribe<GetSnapGridPositionQuery, Vector3>(query => GetSnapGridPosition(query.position));
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
        if (!_isDebug) return;

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
    }

    private void DrawCellTexts()
    {
        // Check camera distance for performance
        Camera sceneCamera = SceneView.lastActiveSceneView?.camera;
        if (sceneCamera != null)
        {
            float distance = Vector3.Distance(sceneCamera.transform.position, transform.position);
            if (distance > _maxDrawDistance) return;
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

        // Calculate offset to center the grid
        float offsetX = -totalWidth / 2;
        float offsetY = -totalHeight / 2;

        // Calculate column and row based on local position
        int column = Mathf.RoundToInt((localPos.x - offsetX) / (_cellSize.x + _spacing.x));
        int row = Mathf.RoundToInt((localPos.y - offsetY) / (_cellSize.y + _spacing.y));

        // Clamp to grid bounds
        column = Mathf.Clamp(column, 0, _columnCount - 1);
        row = Mathf.Clamp(row, 0, _rowCount - 1);

        // Get snapped position
        Vector2 cellPos = GetCellPosition(row, column);
        return transform.position + new Vector3(cellPos.x, cellPos.y, 0);
    }
}
