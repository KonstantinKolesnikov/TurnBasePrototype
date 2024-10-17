using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ShootAction : ActionBase
{
    private Vector3Int positionBase, postionTo;
    private Transform body;
    public void Init(Transform body, Vector3Int position)
    {
        actionType = ActionType.Shoot;
        this.positionBase = position;
        this.body = body;
        actionName = "Shoot";
        speed = 3;
    }

    public override IEnumerator DoSomething()
    {
        Vector3 direction = postionTo - positionBase;
        Vector3 origin = body.position + new Vector3(0.5f, 1.3f, 0.5f);
        RaycastHit hitInfo;
        bool isHit = Physics.Raycast(origin, direction, out hitInfo);
        if (isHit)
        {
            UnitBase unit = hitInfo.transform.GetComponentInParent<UnitBase>();
            if (unit != null)
            {
                unit.hp -= 25;
            }
            EffectManager.Instance.StartLineEffect(origin, hitInfo.point, 0.1f);
            yield return EffectManager.Instance.StartImpactEffect(hitInfo.point, 0.2f);
            if (unit.hp <= 0)
            {
                yield return StartCoroutine(unit.Death());
            }
        }
        else
        {
            yield return EffectManager.Instance.StartLineEffect(origin, origin + (direction * 100), 0.1f);
        }
    }

    public bool SetPosition(Vector3Int position)
    {
        if (position != positionBase)
        {
            postionTo = position;
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void DoOnDummy(Transform dummy)
    {
        base.DoOnDummy(dummy);
    }
}
