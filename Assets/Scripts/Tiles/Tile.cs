using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tile : MonoBehaviour
{
    public bool isWalkable;

    public UnitBase OccupiedUnit;
    public int numOfSubscribers = 0;
    public bool Walkable => isWalkable && OccupiedUnit != null;

    private GameObject selectedIndicator, highlight, wrongIndicator;

    private void Awake()
    {
        selectedIndicator = transform.Find("SelectedTite").gameObject;
        highlight = transform.Find("Highlight").gameObject;
        wrongIndicator = transform.Find("Wrong").gameObject;
    }

    public void SetUnit(UnitBase unit)
    {
        // Если не проверять, можно наступить на последний элемент змейки
        if (OccupiedUnit != null) //ПОЧЕМУУУУ
        {
            OccupiedUnit.occupiedTile = null;
        }
        if (unit.occupiedTile != null)
        {
            unit.occupiedTile.OccupiedUnit = null;
            unit.occupiedTile.UnSubscribe();
        }
        numOfSubscribers = 0;
        unit.transform.position = transform.position;
        OccupiedUnit = unit;
        unit.occupiedTile = this;
        Subscribe();
    }

    public void Subscribe()
    {
        numOfSubscribers++;
    }

    public void UnSubscribe()
    {
        if (numOfSubscribers > 0)
        {
            numOfSubscribers --;
        }
        else
        {
            numOfSubscribers = 0;
        }
    }

    public static void SelectTile(Tile tile, bool activator)
    {
        tile.selectedIndicator.SetActive(activator);
    }
    
    public static void HighlightTile(Tile tile, bool activator)
    {
        tile.highlight.SetActive(activator);
    }
    
    public static void WrongIndicateTile(Tile tile, bool activator)
    {
        tile.wrongIndicator.SetActive(activator);
    }
}
