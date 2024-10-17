using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    public static ActionManager Instance;

    ActionBase currentAction;
    public List<Tile> tiles = new List<Tile>();

    private void Awake()
    {
        Instance = this;
    }

    public void SetAction(ActionType actionType)
    {
        HeroTurnManager.Instance.mode = SelectionMode.Normal;
        DropAction();

        Transform body = UnitManager.Instance.SelectedUnit.transform;
        Vector3 pos = MenuManager.Instance.dummy == null ? body.position : MenuManager.Instance.dummy.transform.position;
        Vector3Int gridPos = PlacementManager.Instance.LocalWorldToCell(pos);

        ActionBase action = CreateAction(actionType, body);

        switch (action)
        {
            case MoveAction moveA:
                Tile tempTile;
                List<Vector2> positions = moveA.GetAllPositions();
                foreach (var position in positions)
                {
                    tempTile = GridManager.Instance.GetTileAtGridPosition(gridPos.x + (int)position.x, gridPos.z + (int)position.y);

                    if (tempTile != null)
                        tiles.Add(GridManager.Instance.GetTileAtGridPosition(gridPos.x + (int)position.x, gridPos.z + (int)position.y));
                }
                foreach (var tile in tiles)
                {
                    Tile.HighlightTile(tile, true);
                }
                HeroTurnManager.Instance.mode = SelectionMode.TileForAction;
                break;
            case ShootAction shotA:
                HeroTurnManager.Instance.mode = SelectionMode.TileForAction;
                break;
            default:
                currentAction = null;
                MenuManager.Instance.ConfirmActionButton(currentAction);
                break;

        }
        currentAction = action;
    }

    public void DropAction()
    {
        if (currentAction != null)
        {
            Destroy(currentAction.gameObject);
            currentAction = null;
        }
        foreach (var t in tiles)
        {
            Tile.HighlightTile(t, false);
        }
        tiles.Clear();
    }

    public ActionBase CreateAction(ActionType actionType, Transform body)
    {
        GameObject actionObject = new GameObject(actionType.ToString());
        Vector3 pos = MenuManager.Instance.dummy == null ? body.position : MenuManager.Instance.dummy.transform.position; // for movin dumy on preview
        Vector3Int gridPos = PlacementManager.Instance.LocalWorldToCell(pos);
        switch (actionType)
        {
            case ActionType.Null:
                return null;
            case ActionType.Nothing:
                return null;
            case ActionType.Move:
                MoveAction moveAction = actionObject.AddComponent<MoveAction>();
                moveAction.Init(body);
                return moveAction;
            case ActionType.Shove:
                ShoveAction shoveAction = actionObject.AddComponent<ShoveAction>();
                shoveAction.Init(body);
                return shoveAction;
            case ActionType.Slam:
                SlamAction slamAction = actionObject.AddComponent<SlamAction>();
                slamAction.Init(body);
                return slamAction;
            case ActionType.Shoot:
                ShootAction shootAction = actionObject.AddComponent<ShootAction>();
                shootAction.Init(body, gridPos);
                return shootAction;
            default:
                return null;
        }
    }

    public bool SetTileForAction(Tile tile)
    {
        if (currentAction == null) return false;

        Transform body = UnitManager.Instance.SelectedUnit.transform;
        Vector3 pos = MenuManager.Instance.dummy == null ? body.position : MenuManager.Instance.dummy.transform.position;
        Vector3Int gridPos = PlacementManager.Instance.LocalWorldToCell(pos);

        Vector3 tilePos = tile.transform.position;
        Vector3Int tileGridPos = PlacementManager.Instance.LocalWorldToCell(tilePos);
        bool result = false;
        switch (currentAction)
        {
            case MoveAction:
                result = ((MoveAction)currentAction).SetPosition(tileGridPos - gridPos);
                if (result)
                {
                    foreach (var t in tiles)
                    {
                        Tile.HighlightTile(t, false);
                    }
                }
                else
                {
                    return false;
                }
                break;
            case ShootAction:
                result = ((ShootAction)currentAction).SetPosition(tileGridPos);
                if (!result)
                {
                    return false;
                }
                break;
            default:
                return false;
        }
        MenuManager.Instance.ConfirmActionButton(currentAction);
        currentAction = null;
        return result;
    }
}
