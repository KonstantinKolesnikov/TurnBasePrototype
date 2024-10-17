using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

public class MoveAction : ActionBase
{
    protected int radius;
    protected Vector3Int direction = Vector3Int.zero;
    protected Transform body;
    protected float time;
    public virtual void Init(Transform body, int radius = 1, float time = 0.2f)
    {
        actionType = ActionType.Move;
        this.radius = radius;
        this.body = body;
        actionName = "Move";
        speed = 2;
        this.time = time;
    }

    public override IEnumerator DoSomething()
    {
          Vector3 gridPositionBase = PlacementManager.Instance.LocalWorldToCell(body.position); 
        Tile nextTile = GridManager.Instance.GetTileAtGridPosition((int)(gridPositionBase.x + direction.x), (int)(gridPositionBase.z + direction.z));
        if (nextTile != null)
        {
            if (nextTile.isWalkable && nextTile.OccupiedUnit == null)
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
                GridManager.Instance.SetObjectToGrid(body.gameObject);
                nextTile.SetUnit(body.gameObject.GetComponent<UnitBase>());
            }
        }
        yield return null;
    }

    public override bool Reckon(out Tile tile)
    {
        Vector3 gridPositionBase = PlacementManager.Instance.LocalWorldToCell(body.position);
        Tile currentTile = body.GetComponent<UnitBase>().occupiedTile;
        Tile nextTile = GridManager.Instance.GetTileAtGridPosition((int)(gridPositionBase.x + direction.x), (int)(gridPositionBase.z + direction.z));
        if (nextTile != null)
        {
            currentTile.UnSubscribe();
            nextTile.Subscribe();
            tile = nextTile;
            return true;
        }
        tile = currentTile;
        return false;
    }

    public void UnRecon(Tile nextTile)
    {
        Tile currentTile = body.GetComponent<UnitBase>().occupiedTile;

        currentTile.Subscribe();
        nextTile.UnSubscribe();
    }

    public List<Vector2> GetAllPositions()
    {
        List<Vector2> result = new List<Vector2>();
        for (int x = -radius; x <= radius; x++)
        {
            int range = radius - Mathf.Abs(x);
            for (int z = -range; z <= range; z++)
            {
                if (x != 0 || z != 0)
                {
                    result.Add(new Vector2(x, z));
                }
            }
        }
        return result;
    }

    public virtual bool SetPosition(Vector3Int direction)
    {
        float distance = Mathf.Sqrt(Mathf.Pow((float)(direction.x), 2) + Mathf.Pow((float)(direction.z), 2));
        if (distance > 1 || distance == 0)
        {
            return false;
        }
        this.direction = direction;
        return true;
    }

    public Vector3Int GetDirection()
    {
        return this.direction;
    }

    public override void DoOnDummy(Transform dummy)
    {
        dummy.position += direction;
    }
}
