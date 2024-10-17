using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrentActionList : MonoBehaviour
{
    public RectTransform actionFieldPref;
    public List<RectTransform> actionList = new List<RectTransform>();
    private float currentHeight = 0;

    public void AddAction(ActionBase action)
    {
        RectTransform temp = Instantiate(actionFieldPref, transform);
        temp.localPosition = new Vector3(0, -currentHeight, 0);
        currentHeight += temp.sizeDelta.y + 2;
        Transform text = temp.Find("Text");
        if (text != null)
        {
            text.GetComponent<TextMeshProUGUI>().text = action.actionName + " " + action.speed;
        }
        actionList.Add(temp);
    }

    public void ClearList()
    {
        foreach (var action in actionList)
        {
            Destroy(action.gameObject);
        }
        actionList.Clear();

        currentHeight = 0;
    }
}
