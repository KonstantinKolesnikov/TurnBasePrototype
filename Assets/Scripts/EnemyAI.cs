using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Vector3 selfAI;
    public Vector3 target;
    private float closeDistance = 3;
    private float farDistance = 5;

    public ActionType SelectAction()
    {
        float distance = Vector3.Distance(selfAI, target);
        if (distance < closeDistance)
        {
            return ActionType.Move;
        }
        else if (distance < farDistance)
        {
            Vector3 direction = target - selfAI;
            RaycastHit hit;
            Physics.Raycast(selfAI + UnitBase.shootOrigin, direction, out hit);

            if (hit.transform.GetComponentInParent<UnitBase>() != null)
            {
                return ActionType.Shoot;
            }
        }
        return ActionType.Nothing;

    }
}
