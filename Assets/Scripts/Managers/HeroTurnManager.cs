using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HeroTurnManager : MonoBehaviour
{
    public static HeroTurnManager Instance;
    public SelectionMode mode = SelectionMode.Normal;
    private Tile selectedTile;


    private void Awake()
    {
        Instance = this;
        GameManager.OnGameStateChange += Begin;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChange -= Begin;
    }
    public void Begin(GameState state)
    {
        if (state == GameState.PlayerSelect)
        {
            MenuManager.Instance.ActivePlayButton();
            PlacementManager.OnMouseClick += DoLogic;
        }
    }

    private void DoLogic(Tile tile)
    {
        if (mode == SelectionMode.Normal)
        {
            if (selectedTile != null)
            {
                Tile.SelectTile(selectedTile, false);
            }
            selectedTile = tile;
            Tile.SelectTile(tile, true);

            if (tile.OccupiedUnit != null)
            {
                if (tile.OccupiedUnit.Faction == Faction.Hero)
                {
                    UnitManager.Instance.SetSelectedUnit(tile.OccupiedUnit);
                    MenuManager.Instance.ActiveMenu(true);
                    MenuManager.Instance.LoadMenu(tile.OccupiedUnit);
                } else
                {
                    UnitManager.Instance.SetSelectedUnit(tile.OccupiedUnit);
                    MenuManager.Instance.ActiveMenu(true, false);
                    MenuManager.Instance.LoadMenu(tile.OccupiedUnit, false);
                }
            }
            else
            {
                if (UnitManager.Instance.SelectedUnit != null)
                {
                    UnitManager.Instance.SetSelectedUnit(null);
                    MenuManager.Instance.ActiveMenu(false);
                }
            }
        } else if (mode == SelectionMode.TileForAction) {
            bool result = ActionManager.Instance.SetTileForAction(tile);
            if (result)
            {
                mode = SelectionMode.Normal;
            }
            else
            {
                StartCoroutine(WrongIndicate(tile));
            }
        }
    }
    private IEnumerator WrongIndicate(Tile tile)
    {
        Tile.WrongIndicateTile(tile, true);
        yield return new WaitForSeconds(1);
        Tile.WrongIndicateTile(tile, false);
    }

    public void DeselectTile()
    {
        if (selectedTile != null)
        {
            Tile.SelectTile(selectedTile, false);
        }   
        UnitManager.Instance.SetSelectedUnit(null);
    }
}


public enum SelectionMode
{
    Normal,
    TileForAction
}