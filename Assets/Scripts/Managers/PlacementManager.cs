using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    public static PlacementManager Instance;
    [SerializeField] 
    private GameObject mouseIndicator, cellIndicator;
    [SerializeField]
    private InputManager inputManager;
    [SerializeField]
    private Grid grid;

    public static event Action<Tile> OnMouseClick;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        Vector3 mousePosition;
        if (inputManager.GetSelectedMapPosition(out mousePosition))
        {
            mouseIndicator.SetActive(true);
            cellIndicator.SetActive(true);
            Vector3Int gridPosition = LocalWorldToCell(mousePosition);
            mouseIndicator.transform.position = mousePosition;
            cellIndicator.transform.position = LocalCellToWorld(gridPosition);

            if (Input.GetMouseButtonDown(0))
            {
                OnMouseClick?.Invoke(GridManager.Instance.GetTileAtGridPosition(gridPosition.x, gridPosition.z));
            }
        }
        else
        {
            mouseIndicator.SetActive(false);
            cellIndicator.SetActive(false);
        }
    }

    public Vector3Int LocalWorldToCell(Vector3 pos) { return grid.WorldToCell(pos);}

    public Vector3 LocalCellToWorld(Vector3Int pos) { return grid.CellToWorld(pos);}

    public void ResetSubscriptions() => OnMouseClick = null;
}
