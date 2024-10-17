using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBase : MonoBehaviour
{
    public UnitBase target;
    public int hp = 15;

    public Tile occupiedTile;
    public Faction Faction;
    public GameObject dummy;
    public List<ActionBase> unitsActions = new List<ActionBase>();
    public int speedSum = 0;
    public static Vector3 shootOrigin = new Vector3(0.5f, 1.3f, 0.5f);

    //public void SetAction(ActionType action)
    //{
    //    switch (action)
    //    {
    //        case ActionType.Null:
    //            currentAction = null;
    //            break;
    //        case ActionType.Move:
    //            currentAction = new MoveAction();
    //            break;
    //        case ActionType.Nothing:
    //            currentAction = null;
    //            break;
    //        case ActionType.Shoot:
    //            currentAction = new ShootAction();
    //            break;
    //        default:
    //            currentAction = null;
    //            break;
    //    }
    //}

    public IEnumerator Death()
    {
        transform.GetComponentInChildren<Renderer>().enabled = false;
        GameObject afterImage = Instantiate(dummy, transform.position, transform.rotation);
        yield return new WaitForSeconds((float)0.2);
        Destroy(afterImage);
        gameObject.SetActive(false);
    }
}
