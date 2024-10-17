using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnotherMoveAction : MoveAction
{
    public bool isSlippery = true;
    public IEnumerator DoSomething(Action<int> callback)
    {
        Vector3 gridPositionBase = PlacementManager.Instance.LocalWorldToCell(body.position);
        Tile nextTile = GridManager.Instance.GetTileAtGridPosition((int)(gridPositionBase.x + direction.x), (int)(gridPositionBase.z + direction.z));
        if (nextTile != null)
        {
            if (nextTile.isWalkable)
            {
                Vector3 end = body.position + direction;
                float elapsedTime = 0;
                Vector3 startingPos = body.position;
                while (elapsedTime < time)
                {
                    body.position = Vector3.Lerp(startingPos, end, (elapsedTime / time));
                    elapsedTime += Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                }
                body.position = end;
                Tile temp = GridManager.Instance.SetObjectToGrid(body.gameObject);
                nextTile.SetUnit(body.gameObject.GetComponent<UnitBase>());
            }
        }
        yield return null;
        callback(1);
    }

    public override bool SetPosition(Vector3Int direction)
    {
        float distance = Mathf.Sqrt(Mathf.Pow((float)(direction.x), 2) + Mathf.Pow((float)(direction.z), 2));
        if (distance > 1)
        {
            return false;
        }
        this.direction = direction;
        return true;
    }
}
