using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GridManager;

public class PhaseManager : MonoBehaviour
{
    public static PhaseManager Instance;
    public delegate IEnumerator ActionLogic();
    private delegate IEnumerator Foo();
    private static Dictionary<int, List<Foo>> orderList;
    private void Awake()
    {
        Instance = this;
        GameManager.OnGameStateChange += DoStuff;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChange -= DoStuff;
    }

    public void DoStuff(GameState state)
    {
        if (state == GameState.FastPhase)
        {
            orderList = new Dictionary<int, List<Foo>>();
            foreach (var unit in UnitManager.Instance.units)
            {
                if (unit.unitsActions != null || unit.unitsActions.Count > 0)
                {
                    foreach (var action in unit.unitsActions)
                    {
                        if (action != null)
                        {
                            if (!orderList.ContainsKey(action.speed))
                            {
                                orderList[action.speed] = new List<Foo>();
                            }
                            orderList[action.speed].Add(action.DoSomething);
                        }
                    }
                }
            }
            StartCoroutine(OperateActions());
        }
    }

    public IEnumerator OperateActions()
    {
        while (orderList.Count > 0)
        {
            List<int> keys = orderList.Keys.OrderBy(x => x).ToList();
            while (orderList[keys[0]].Count > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, orderList[keys[0]].Count);
                yield return StartCoroutine(orderList[keys[0]][randomIndex]());
                orderList[keys[0]].Remove(orderList[keys[0]][randomIndex]);
            }
            orderList.Remove(keys[0]);
        }
        orderList.Clear();
        EndPhases();
        GameManager.Instance.UpdateGameState(GameState.AttachToGrid);
        yield return null;
    }

    public void EndPhases()
    {
        UnitManager.Instance.ClearUnitsActions();
    }
}
