using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlamAction : AnotherMoveAction // MoveAction
{
    List<AnotherMoveAction> moveActions = null;
    public bool isTerm = false; // for only GetMoves
    public override void Init(Transform body, int radius = 1, float time = 0.2f)
    {
        base.Init(body);
        actionType = ActionType.Slam;
        actionName = "Slam";
        this.body = body;
        this.time = time;
    }

    public override IEnumerator DoSomething()
    {
        if (isTerm) yield return null; 

        GetMoves();
        if (moveActions != null)
        {
            GameObject actionObj = new GameObject("Slam", typeof(SimulteneousMoveAction));
            SimulteneousMoveAction actionSim = actionObj.GetComponent<SimulteneousMoveAction>();
            actionSim.Init(moveActions);

            yield return StartCoroutine(actionSim.DoSomething());

            Destroy(actionObj);
        }
        yield return null;
    }

    public List<AnotherMoveAction> GetMoves()
    {
        Vector3 gridPositionBase = PlacementManager.Instance.LocalWorldToCell(body.position);
        Tile nextTile = GridManager.Instance.GetTileAtGridPosition((int)(gridPositionBase.x + direction.x), (int)(gridPositionBase.z + direction.z));
        if (nextTile != null || !nextTile.isWalkable)
        {
            moveActions = new List<AnotherMoveAction>();
            if (nextTile.OccupiedUnit != null)
            {
                Vector3 targetUnitGridPosition = PlacementManager.Instance.LocalWorldToCell(nextTile.OccupiedUnit.transform.position);
                Tile targetUnitNextTile = GridManager.Instance.GetTileAtGridPosition((int)(targetUnitGridPosition.x + direction.x), (int)(targetUnitGridPosition.z + direction.z));

                GameObject targetMoveObj = new GameObject("Move", typeof(AnotherMoveAction));
                AnotherMoveAction targetMove = targetMoveObj.GetComponent<AnotherMoveAction>();

                targetMove.Init(nextTile.OccupiedUnit.transform);
                targetMove.SetPosition(direction);
                moveActions.Add(targetMove);
            }
            GameObject selfMoveObj = new GameObject("Move", typeof(AnotherMoveAction));
            AnotherMoveAction selfMove = selfMoveObj.GetComponent<AnotherMoveAction>();
            selfMove.Init(body);
            selfMove.SetPosition(direction);
            moveActions.Add(selfMove);
        }
        return moveActions;
    }
}