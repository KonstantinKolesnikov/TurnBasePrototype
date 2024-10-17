using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    private Dictionary<Vector2, Tile> tiles = new Dictionary<Vector2, Tile>();

    private ObjectToSet[] objectsToSet;
    private CellToSet[] cellsToSet;
    private ObjectToPlaceOnHalfGrid[] objectsOnHalfGrid;

    private void Awake()
    {
        Instance = this;
        objectsToSet = (ObjectToSet[])FindObjectsOfType(typeof(ObjectToSet));
        cellsToSet = (CellToSet[])FindObjectsOfType(typeof(CellToSet));
        objectsOnHalfGrid = (ObjectToPlaceOnHalfGrid[])FindObjectsOfType(typeof(ObjectToPlaceOnHalfGrid));
        GameManager.OnGameStateChange += DoStuff;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChange -= DoStuff;
    }

    public void DoStuff(GameState state)
    {
        if (state == GameState.GenerateGrid)
        {
            StartCoroutine(SetCellsToGrid());
        } else if (state == GameState.AttachToGrid) 
        {
            StartCoroutine(SetObjectsToGrid());
            foreach(var obj in objectsOnHalfGrid)
            {
                PlaceObjectToHalfGrid(obj.gameObject);
            }
        }
    }

    public IEnumerator SetCellsToGrid()
    {
        foreach (var cell in cellsToSet) 
        {
            Vector3 offset = new Vector3(0.5f, 0.5f, 0.5f); //Offset from center of grid
            Vector3 pos = cell.transform.position + offset;
            Vector3Int posOnGrid = PlacementManager.Instance.LocalWorldToCell(pos);
            pos = PlacementManager.Instance.LocalCellToWorld(posOnGrid);
            cell.transform.position = pos;
            Vector2 cellKey = new Vector2(posOnGrid.x, posOnGrid.z);
            Tile tempTile;
            if (tiles.TryGetValue(cellKey, out tempTile))
            {
                Debug.Log("2 object on cell!!!");
                Tile.HighlightTile(tempTile, true);
            }
            tiles[cellKey] = cell.gameObject.GetComponent<Tile>();
        }
        GameManager.Instance.UpdateGameState(GameState.AttachToGrid);
        yield return null;
    }

    public IEnumerator SetObjectsToGrid()
    {
        foreach(var thing in objectsToSet)
        {
            Vector3 offset = new Vector3(0.5f, 0.5f, 0.5f); //Offset from center of grid
            Vector3 pos = thing.transform.position + offset;
            Vector3Int posOnGrid = PlacementManager.Instance.LocalWorldToCell(pos);
            pos = PlacementManager.Instance.LocalCellToWorld(posOnGrid);
            thing.transform.position = pos;
            Tile tile = GetTileAtGridPosition(posOnGrid.x, posOnGrid.z);
            tile.SetUnit(thing.gameObject.GetComponent<UnitBase>());
        }
        GameManager.Instance.UpdateGameState(GameState.EnemySelect);
        yield return null;
    }

    public Tile SetObjectToGrid(GameObject thing)
    {
        Vector3 offset = new Vector3(0.5f, 0f, 0.5f);
        Vector3 pos = thing.transform.position + offset;
        Vector3Int posOnGrid = PlacementManager.Instance.LocalWorldToCell(pos);
        pos = PlacementManager.Instance.LocalCellToWorld(posOnGrid);
        thing.transform.position = pos;
        Tile tile = GetTileAtGridPosition(posOnGrid.x, posOnGrid.z);
        return tile;
    }

    public void PlaceObjectToHalfGrid(GameObject thing)
    {
        Vector3 offset = new Vector3(0.5f, 0.5f, 0.5f); //Offset from center of grid
        Vector3 pos = thing.transform.position + offset;
        Vector3Int posOnGrid = PlacementManager.Instance.LocalWorldToCell(pos);
        Vector3 posDelta = pos - posOnGrid;
        float[] deltas = new float[] { posDelta.x, posDelta.z };
        for (int i = 0; i < deltas.Length; ++i){
            if (Mathf.Abs(deltas[i]) > 0.25)
            {
                deltas[i] = 0.5f * (deltas[i] / Mathf.Abs(deltas[i])); // x / abs(x) -> -1 if x < 0, +1 if x > 0
            } else
            {
                deltas[i] = 0;  
            }
        }
        pos = PlacementManager.Instance.LocalCellToWorld(posOnGrid);
        pos += new Vector3(deltas[0], 0, deltas[1]);
        thing.transform.position = pos;
    }

    public Tile GetTileAtGridPosition(int x, int z)
    {
        Tile tile;
        if (tiles.TryGetValue(new Vector2(x, z), out tile))
        {
            return tile;
        }
        return null;
    }
}
