using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionBase : MonoBehaviour
{
    public ActionType actionType;
    public string actionName = "Base";
    public int speed = 0;

    public Coroutine Begin()
    {
        return StartCoroutine(DoSomething());
    }
    public virtual IEnumerator DoSomething() {
        Debug.Log(actionName);
        yield return null;
    }

    public virtual bool Reckon(out Tile tile)
    {
        tile = null;
        return true;
    }

    //public virtual void ShowSelection(bool activator)
    //{
    //    Debug.Log("show selection");
    //}

    public virtual void DoOnDummy(Transform dummy)
    {
        Debug.Log("Показываю на кукле");
    }

}

public enum ActionType
{
    Nothing,
    SimulteneousMove,
    Move,
    Shoot,
    Shove,
    Slam,
    Null
}