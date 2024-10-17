using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class ActionButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public ActionType actionType;
    private GameObject highlight;
    private GameObject selected;

    public event Action<ActionButton> OnMouseClick;

    private void Awake()
    {
        highlight = gameObject.transform.Find("Highlight").gameObject;
        selected = gameObject.transform.Find("Selected").gameObject;
    }

    public void Start()
    {
        OnMouseClick += MenuManager.Instance.SetSelectedActionButton;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        highlight.SetActive(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        highlight.SetActive(false);
        OnMouseClick?.Invoke(this);
    }

    public void SetSelected(bool active)
    {
        selected.SetActive(active);
    }
}
