using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private Camera sceneCamera;

    private Vector3 lastPosition;

    [SerializeField]
    private LayerMask placementLayermask;

    public bool GetSelectedMapPosition(out Vector3 mousePosition)
    {
        Vector3 mousePos = Input.mousePosition;
        Ray ray = sceneCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (!IsMouseOverUi() && Physics.Raycast(origin: ray.origin,
                                                direction: ray.direction,
                                                hitInfo: out hit,
                                                maxDistance: 200,
                                                layerMask: placementLayermask))
        {
            lastPosition = hit.point;
            mousePosition = lastPosition;
            return true;
        }
        else
        {
            mousePosition = lastPosition;
            return false;
        }
    }

    private bool IsMouseOverUi()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
}
