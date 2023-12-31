using UnityEngine;

public class IndexProvider : MonoBehaviour
{
    [SerializeField] private float _cellOffset = 100f;

    public int RowCount => _rowCount;
    public int ColumnCount => _columnCount;
    
    private int _rowCount;
    private int _columnCount;
    
    private Vector2 _halfMapSize;

    public void Initialize(int countItemsColumn, int countItemsRow)
    {
        _columnCount = countItemsColumn;
        _rowCount = countItemsRow;
        
        _halfMapSize = new Vector2((countItemsColumn / 2), countItemsRow / 2);
    }

    public Vector2 GetCoordinates(int indexColumn, int indexRow)
    {
        return (new Vector2(indexColumn, indexRow) - _halfMapSize) * _cellOffset;
    }
    
    public Vector2Int GetIndex(Vector2 coordinates)
    {
        int indexColumn = Mathf.RoundToInt(coordinates.x / 100 + _halfMapSize.x);
        int indexRow = Mathf.RoundToInt(coordinates.y / 100  + _halfMapSize.y);
        return new Vector2Int( indexColumn,indexRow);
    }
}