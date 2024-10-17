using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class PlayButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private GameObject highlight;

    public static event Action OnMouseClick;

    private void Awake()
    {
        highlight = gameObject.transform.Find("Highlight").gameObject;
    }

    public void Start()
    {
        OnMouseClick += MenuManager.Instance.EndSelectionPhase;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        highlight.SetActive(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        highlight.SetActive(false);
        OnMouseClick?.Invoke();
    }
}