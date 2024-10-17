using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyTurnManager : MonoBehaviour
{
    public static EnemyTurnManager Instance;

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
        if (state == GameState.EnemySelect)
        {
            StartCoroutine(DoLogic());
        }
    }

    IEnumerator DoLogic()
    {
        {
            ObstacleAxis axis = null;
            List<Obstacle1> obstacles = new List<Obstacle1>();
            foreach (var unit in UnitManager.Instance.units)
            {
                if (unit is Obstacle1)
                {
                    obstacles.Add(unit as Obstacle1);
                }
                if (unit is ObstacleAxis)
                {
                    axis = (ObstacleAxis)unit;
                }
            }

            foreach (var obstacle in obstacles)
            {
                axis.onOrbit.Add(obstacle);
            }

            List<AnotherMoveAction> moveActions = new List<AnotherMoveAction>();
            Vector3Int axisGridPos = PlacementManager.Instance.LocalWorldToCell(axis.transform.position);
            Vector3 axisPos = axisGridPos + (axis.transform.position - PlacementManager.Instance.LocalCellToWorld(axisGridPos));
            foreach (var obstacle in obstacles)
            {
                Vector3Int obstacleGridPos = PlacementManager.Instance.LocalWorldToCell(obstacle.transform.position);
                Vector3 relatedGridPos = axisPos - obstacleGridPos;
                if (Mathf.Abs(relatedGridPos.x) == Mathf.Abs(relatedGridPos.z) && Vector3Int.CeilToInt(relatedGridPos) != obstacle.lastTurnPos)
                {
                    int xTemp, zTemp;
                    xTemp = obstacle.xChange; zTemp = obstacle.zChange;
                    if (zTemp == xTemp)
                    {
                        obstacle.zChange = -zTemp;
                    }
                    else
                    {
                        obstacle.xChange = zTemp;
                    }
                    obstacle.lastTurnPos = Vector3Int.CeilToInt(relatedGridPos);
                    obstacle.direction += new Vector3Int(obstacle.xChange, 0, obstacle.zChange);
                }

                GameObject moveObj = new GameObject("Move", typeof(SlamAction));
                SlamAction move = moveObj.GetComponent<SlamAction>();
                move.isTerm = true;
                move.isSlippery = false;
                move.Init(obstacle.transform);
                move.SetPosition(obstacle.direction);
                moveActions = moveActions.Concat(move.GetMoves()).ToList();
            }

            GameObject actionObj = new GameObject("SlamSim", typeof(SimulteneousMoveAction));
            SimulteneousMoveAction actionSim = actionObj.GetComponent<SimulteneousMoveAction>();
            actionSim.Init(moveActions);
            obstacles[0].unitsActions.Add(actionSim);
        }
        {
            GameObject temp = new GameObject("EnemyAI");
            EnemyAI enemyAI = temp.AddComponent<EnemyAI>();
            UnitBase unit = (UnitBase)FindAnyObjectByType(typeof(EnemyBase));
            UnitBase hero = unit?.target;

            if (unit != null)
            {
                if (unit.target != null && unit.target.gameObject.activeInHierarchy)
                {

                    enemyAI.target = hero.transform.position;
                    enemyAI.selfAI = unit.transform.position;

                    ActionType actionType = enemyAI.SelectAction();

                    Vector3 pos = unit.transform.position;
                    Vector3Int gridPos = PlacementManager.Instance.LocalWorldToCell(pos);
                    ActionBase currentAction = null;

                    switch (actionType)
                    {
                        case ActionType.Null:
                            currentAction = null;
                            break;
                        case ActionType.Move:
                            MoveAction moveAction = (MoveAction)ActionManager.Instance.CreateAction(ActionType.Move, unit.transform);
                            List<Tile> tiles = new List<Tile>();
                            List<Vector2> positions = moveAction.GetAllPositions();
                            foreach (var position in positions)
                            {
                                Tile tempTile = GridManager.Instance.GetTileAtGridPosition(gridPos.x + (int)position.x, gridPos.z + (int)position.y);
                                if (temp != null)
                                {
                                    tiles.Add(tempTile);
                                }
                            }

                            float distance = Vector3.Distance(unit.transform.position, hero.transform.position);
                            Tile selectedTile = null;
                            foreach (var tile in tiles)
                            {
                                if (tile != null)
                                {
                                    if (Vector3.Distance(tile.transform.position, hero.transform.position) > distance)
                                    {
                                        selectedTile = tile;
                                        distance = Vector3.Distance(tile.transform.position, hero.transform.position);
                                    }
                                }

                            }
                            Vector3 tilePos = selectedTile.transform.position;
                            Vector3Int tileGridPos = PlacementManager.Instance.LocalWorldToCell(tilePos);
                            Debug.Log(moveAction.SetPosition(tileGridPos - gridPos));
                            Debug.Log(tileGridPos);
                            currentAction = moveAction;
                            break;
                        case ActionType.Nothing:
                            currentAction = null;
                            break;
                        case ActionType.Shoot:
                            ShootAction shootAction = (ShootAction)ActionManager.Instance.CreateAction(ActionType.Shoot, unit.transform);
                            shootAction.SetPosition(PlacementManager.Instance.LocalWorldToCell(hero.transform.position));
                            currentAction = shootAction;

                            break;
                        default:
                            currentAction = null;
                            break;
                    }

                    if (currentAction != null) unit.unitsActions.Add(currentAction);
                }
            }

            Destroy(temp);
            GameManager.Instance.UpdateGameState(GameState.PlayerSelect);
            yield return null;
        }
    }
}
