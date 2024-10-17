using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoveAction : MoveAction
{
    public override void Init(Transform body, int radius = 1, float time = 0.2f)
    {
        base.Init(body);
        actionType = ActionType.Shove;
        actionName = "Shove";
    }

    public override IEnumerator DoSomething()
    {
        Vector3 gridPositionBase = PlacementManager.Instance.LocalWorldToCell(body.position);
        Tile nextTile = GridManager.Instance.GetTileAtGridPosition((int)(gridPositionBase.x + direction.x), (int)(gridPositionBase.z + direction.z));
        if (nextTile != null)
        {
            if (nextTile.OccupiedUnit != null)
            {
                Vector3 targetUnitGridPosition = PlacementManager.Instance.LocalWorldToCell(nextTile.OccupiedUnit.transform.position);
                Tile targetUnitNextTile = GridManager.Instance.GetTileAtGridPosition((int)(targetUnitGridPosition.x + direction.x), (int)(targetUnitGridPosition.z + direction.z));
                if (targetUnitNextTile != null && targetUnitNextTile.isWalkable && targetUnitNextTile.OccupiedUnit == null)
                {
                    MoveAction targetMove = (MoveAction)ActionManager.Instance.CreateAction(ActionType.Move, nextTile.OccupiedUnit.transform);
                    targetMove.SetPosition(direction);
                    yield return targetMove.Begin();
                }
            }
        }

        yield return null;
    }

    public override void DoOnDummy(Transform dummy)
    {
        base.DoOnDummy(dummy);
    }
}
