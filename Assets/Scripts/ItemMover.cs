using System;
using DG.Tweening;
using UnityEngine;


public class ItemMover : MonoBehaviour
{
    [SerializeField] private float _moveDuration = 1f;
    [SerializeField] private IndexProvider _indexProvider;
    [SerializeField] private MapBuilder _mapBuilder;

    private GameObject _firstElement;
    private GameObject _firstParent;
    private Vector2Int _firstElementCoordinates;

    private GameObject _secondElement;
    private GameObject _secondParent;
    private Vector2Int _secondElementCoordinates;

    private bool _hasFirstElement;
    private bool _hasSecondElement;


    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && _hasFirstElement == false) //захват первого элемента
        {
            _hasFirstElement = SelectElement(out _firstElement, out _firstParent);
        }

        if (Input.GetMouseButtonUp(0) && _hasFirstElement) //захват второго элемента
        {
            _hasSecondElement = SelectElement(out _secondElement, out _secondParent);

            if (!_hasSecondElement) //если не оказалось элемента под мышью то сбрасываем так же и первый элемент
            {
                _hasFirstElement = false;
            }
        }

        if (_hasFirstElement && _hasSecondElement) //если оба элемента есть
            //и выполняется условие проверки, то меняем их местами
        {
            if (CanMove())
            {
                ChangeElements();
            }
            else
            {
                _hasFirstElement = false;
                _hasSecondElement = false;
            }
        }
    }

    private void ChangeElements()
    {
        _hasFirstElement = false;
        _hasSecondElement = false;

        var firstElementStartPosition = _firstElement.transform.position;
        var secondElementStartPosition = _secondElement.transform.position;

        _firstElement.transform
            .DOMove(secondElementStartPosition, _moveDuration)
            .SetEase(Ease.OutQuint);

        _secondElement.transform
            .DOMove(firstElementStartPosition, _moveDuration)
            .SetEase(Ease.OutQuint);
        
        ReversElements();

        if (!CheckMatches())
        {
            _firstElement.transform
                .DOMove(firstElementStartPosition, _moveDuration)
                .SetEase(Ease.OutQuint);

            _secondElement.transform
                .DOMove(secondElementStartPosition, _moveDuration)
                .SetEase(Ease.OutQuint);
            
            ReversElements();
        }
    }

    private void ReversElements()
    {
        var secondParent = _secondParent;
        var firstParent = _firstParent;

        _firstParent = secondParent;
        _secondParent = firstParent;

        _firstElement.transform.SetParent(secondParent.transform);
        _secondElement.transform.SetParent(firstParent.transform);

        var secondElement = _mapBuilder.TileMap[_secondElementCoordinates.y, _secondElementCoordinates.x];
        var firstElement = _mapBuilder.TileMap[_firstElementCoordinates.y, _firstElementCoordinates.x];

        _mapBuilder.TileMap[_secondElementCoordinates.y, _secondElementCoordinates.x] = firstElement;
        _mapBuilder.TileMap[_firstElementCoordinates.y, _firstElementCoordinates.x] = secondElement;
    }

    private bool SelectElement(out GameObject element, out GameObject parent)
    {
        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var hit = Physics2D.Raycast(mousePosition, Vector2.one);

        if (hit.collider != null)
        {
            element = hit.collider.gameObject;
            parent = element.transform.parent.gameObject;
            return true;
        }
        else
        {
            element = null;
            parent = null;
            return false;
        }
    }

    private bool CanMove() //проверяем что объекты рядом и что двигать планируем не диагонально
    {
        // если индекс x отлиается на единицу а у не отличается или если у отличается на единицу а х не меняется
        _firstElementCoordinates = _indexProvider.GetIndex(_firstElement.transform.position);
        _secondElementCoordinates = _indexProvider.GetIndex(_secondElement.transform.position);

        if (_firstElementCoordinates.x == _secondElementCoordinates.x &&
            Math.Abs(_firstElementCoordinates.y - _secondElementCoordinates.y) == 1)
        {
            return true;
        }

        else if (_firstElementCoordinates.y == _secondElementCoordinates.y &&
                 Math.Abs(_firstElementCoordinates.x - _secondElementCoordinates.x) == 1)
        {
            return true;
        }

        return false;
    }

    private bool CheckMatches()
    {
        return CheckColumn()||CheckRow();
    }

    private bool CheckColumn()
    {
        var matchesCount = 1; //единицей помечаем первое совпадение - это текущий элемент

        var typeOfElement =
            Convert.ToInt32(_mapBuilder.TileMap[_secondElementCoordinates.y, _secondElementCoordinates.x].Type);

        //проверяем совпадения справа
        for (int i = _secondElementCoordinates.y + 1; i < _indexProvider.ColumnCount; i++)
        {
            var typeOfCurrentElement = Convert.ToInt32(_mapBuilder.TileMap[i, _secondElementCoordinates.x].Type);

            if (typeOfElement == typeOfCurrentElement)
            {
                matchesCount++;
            }

            else
            {
                break;
            }
        }

        //проверяем слева
        for (int i = _secondElementCoordinates.y - 1; i >= 0; i--)
        {
            var typeOfCurrentElement = Convert.ToInt32(_mapBuilder.TileMap[i, _secondElementCoordinates.x].Type);

            if (typeOfElement == typeOfCurrentElement)
            {
                matchesCount++;
            }

            else
            {
                break;
            }
        }
        
        return matchesCount > 2;
    }

    private bool CheckRow()
    {
        var matchesCount = 1; //единицей помечаем первое совпадение - это текущий элемент

        var typeOfElement =
            Convert.ToInt32(_mapBuilder.TileMap[_secondElementCoordinates.y, _secondElementCoordinates.x].Type);

        //проверяем совпадения сверху
        for (int i = _secondElementCoordinates.x + 1; i < _indexProvider.RowCount; i++)
        {
            var typeOfCurrentElement = Convert.ToInt32(_mapBuilder.TileMap[_secondElementCoordinates.y, i].Type);

            if (typeOfElement == typeOfCurrentElement)
            {
                matchesCount++;
            }

            else
            {
                break;
            }
        }

        //проверяем вниз
        for (int i = _secondElementCoordinates.x - 1; i >= 0; i--)
        {
            var typeOfCurrentElement = Convert.ToInt32(_mapBuilder.TileMap[_secondElementCoordinates.y, i].Type);

            if (typeOfElement == typeOfCurrentElement)
            {
                matchesCount++;
            }

            else
            {
                break;
            }
        }

        return matchesCount > 2;
    }
}