using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SimulteneousSlamAction : ActionBase
{
    Dictionary<AnotherMoveAction, Tile> actionsTiles = new Dictionary<AnotherMoveAction, Tile>();
    Dictionary<AnotherMoveAction, List<Vector3Int>> actionAlternatives = new Dictionary<AnotherMoveAction, List<Vector3Int>>();
    public void Init(List<AnotherMoveAction> actions)
    {
        actionType = ActionType.SimulteneousMove;
        foreach (AnotherMoveAction action in actions)
        {
            actionsTiles[action] = null;
            Vector3Int direction = action.GetDirection();
            Vector3Int alternative1 = new Vector3Int(direction.z, 0, direction.x);
            Vector3Int alternative2 = new Vector3Int(-direction.z, 0, -direction.x);
            actionAlternatives[action] = new List<Vector3Int> { alternative1, alternative2 };
        }
        speed = 0;
    }

    public IEnumerator ReckonActions()
    {
        Dictionary<AnotherMoveAction, Tile> troubles = new Dictionary<AnotherMoveAction, Tile>();
        List<AnotherMoveAction> normActions = new List<AnotherMoveAction>();
        do
        {
            troubles.Clear();
            List<AnotherMoveAction> keysAct = actionsTiles.Keys.ToList<AnotherMoveAction>();
            for (int i = 0; i < keysAct.Count; i++)
            {
                Tile temp;
                if (keysAct[i].Reckon(out temp))
                {
                    if (temp != null)
                    {
                        actionsTiles[keysAct[i]] = temp;
                    }
                }
                else
                {
                    troubles[keysAct[i]] = null;
                }
            }
            foreach (var pair in actionsTiles)
            {
                if (pair.Value != null && !normActions.Contains(pair.Key))
                {
                    if (pair.Value.numOfSubscribers > 1)
                    {
                        troubles[pair.Key] = pair.Value;
                    }
                }
            }

            List<AnotherMoveAction> keysTrab = troubles.Keys.ToList<AnotherMoveAction>();
            for (int i = 0; i < keysTrab.Count; i++)
            {
                if (keysTrab[i] == null)
                {
                    continue;
                }
                if (troubles.Values.Contains<Tile>(null))
                {
                    if (troubles[keysTrab[i]] == null)
                    {
                        if (keysTrab[i].isSlippery && actionAlternatives[keysTrab[i]].Count > 0)
                        {
                            int rand = Random.Range(0, actionAlternatives[keysTrab[i]].Count);
                            keysTrab[i].SetPosition(actionAlternatives[keysTrab[i]][rand]);
                            actionAlternatives[keysTrab[i]].Remove(actionAlternatives[keysTrab[i]][rand]);
                        }
                        else
                        {
                            keysTrab[i].SetPosition(Vector3Int.zero);
                            normActions.Add(keysTrab[i]);
                        }
                    }
                }
                else
                {
                    bool changed = false;
                    for (int j = i + 1; j < keysTrab.Count; j++)
                    {
                        if (troubles[keysTrab[i]].GetInstanceID() == troubles[keysTrab[j]].GetInstanceID())
                        {
                            changed = true;
                            int random = Random.Range(100, 100);
                            int key, anotherKey;
                            if (random < 0)
                            {
                                key = j;
                                anotherKey = i;
                            }
                            else
                            {
                                key = i;
                                anotherKey = j;
                            }
                            if (keysTrab[key].isSlippery && actionAlternatives[keysTrab[key]].Count > 0)
                            {
                                int rand = Random.Range(0, actionAlternatives[keysTrab[key]].Count);
                                keysTrab[key].SetPosition(actionAlternatives[keysTrab[key]][rand]);
                                actionAlternatives[keysTrab[key]].Remove(actionAlternatives[keysTrab[key]][rand]);
                            }
                            else
                            {
                                keysTrab[key].SetPosition(Vector3Int.zero);
                                normActions.Add(keysTrab[key]);
                            }
                            keysTrab[key] = null;
                            keysTrab[anotherKey] = null;
                        }
                    }
                    if (!changed)
                    {
                        if (keysTrab[i].isSlippery && actionAlternatives[keysTrab[i]].Count > 0)
                        {
                            int rand = Random.Range(0, actionAlternatives[keysTrab[i]].Count);
                            keysTrab[i].SetPosition(actionAlternatives[keysTrab[i]][rand]);
                            actionAlternatives[keysTrab[i]].Remove(actionAlternatives[keysTrab[i]][rand]);
                        }
                        else
                        {
                            keysTrab[i].SetPosition(Vector3Int.zero);
                            normActions.Add(keysTrab[i]);
                        }
                    }
                }
            }

            if (troubles.Count > 0)
            {
                for (int i = 0; i < keysAct.Count; i++)
                {
                    if (actionsTiles[keysAct[i]] != null)
                    {
                        keysAct[i].UnRecon(actionsTiles[keysAct[i]]);
                    }
                }
            }
            else
            {
                for (int i = 0; i < keysAct.Count; i++)
                {
                    Tile temp;
                    if (keysAct[i].Reckon(out temp))
                    {
                        if (temp != null)
                        {
                            actionsTiles[keysAct[i]] = temp;
                        }
                    }
                    else
                    {
                        troubles[keysAct[i]] = null;
                    }
                }
            }

            yield return null;
        } while (troubles.Count > 0);
    }

    public override IEnumerator DoSomething()
    {
        yield return StartCoroutine(ReckonActions());
        int done = 0;
        int inProcess = 0;
        foreach (var action in actionsTiles.Keys)
        {
            inProcess++;
            StartCoroutine(action.DoSomething(result =>
            {
                done += result;
            }));
        }
        while (done < inProcess)
        {
            yield return null;
        }

        yield return null;
    }

    private void OnDestroy()
    {
        List<AnotherMoveAction> keysAct = actionsTiles.Keys.ToList<AnotherMoveAction>();
        foreach (AnotherMoveAction i in keysAct)
        {
            if (i != null) Destroy(i.gameObject);
        }
    }
}
