using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using System;
using Unity.VisualScripting;
using static GridManager;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;
    public GameObject menu;
    public GameObject currentActionMenu;
    public CurrentActionList currentActionList;
    public ActionButton currentActionButton;
    public ActionButton selectedActionButton;
    public UnitBase currentUnit;

    private List<ActionButton> actionButtons = new List<ActionButton>();
    public GameObject dummy = null;
    public PlayButton playButton;

    private void Awake()
    {
        Instance = this;
        actionButtons = menu.GetComponentsInChildren<ActionButton>().ToList<ActionButton>();
    }
    public void ActiveMenu(bool active, bool isAlly = true)
    {
        // to realisation
        UnitManager.Instance.GetMoves();

        if (isAlly) menu.SetActive(active);
        currentActionMenu.SetActive(active);
    }

    public void ActivePlayButton() {
        playButton.gameObject.SetActive(true);
    }

    public void SetSelectedActionButton(ActionButton button)
    {
        if (currentUnit.unitsActions.Count > 0)
        {
            int moveActions = 0;
            int notMoveActions = 0;
            foreach (var action in currentUnit.unitsActions)
            {
                if (action.actionType == ActionType.Move)
                {
                    moveActions++;
                }
                else
                {
                    notMoveActions++;
                }
            }
            if (moveActions >= 2 && button.actionType == ActionType.Move)
            {
                return;
            }
            if (notMoveActions >= 1 && button.actionType != ActionType.Move)
            {
                return;
            }
        }
        if (currentActionButton != null)
        {
            currentActionButton.SetSelected(false);
        }
        if (selectedActionButton != null)
        {
            selectedActionButton.SetSelected(false);
        }
        selectedActionButton = button;
        button.SetSelected(true);
        ActionManager.Instance.SetAction(button.actionType);
    }

    public void ConfirmActionButton(ActionBase action)
    {
        currentActionButton = selectedActionButton;
        currentUnit.speedSum += action.speed;
        action.speed = currentUnit.speedSum;
        currentActionList.AddAction(action);
        currentUnit.unitsActions.Add(action);
    }

    public void LoadMenu(UnitBase unit, bool isAlly = true)
    {
        if (currentActionButton != null)
        {
            currentActionButton.SetSelected(false);
        }

        currentUnit = unit;
        
        currentActionList.ClearList();
        if (unit.unitsActions != null)
        {
            foreach(var action in unit.unitsActions)
            {
                currentActionList.AddAction((ActionBase)action);
            }
            if (!isAlly) return; 
            StartCoroutine(DemostrateActions());
        }
        else
        {
            foreach (var button in actionButtons)
            {
                if (button.actionType == ActionType.Nothing)
                {
                    currentActionButton = button;
                    button.SetSelected(true);
                }
            }
        }
    }

    public IEnumerator DemostrateActions()
    {
        int currentNumberOfActions = 0;

        while (menu.activeInHierarchy)
        {
            if (currentUnit.unitsActions.Count != currentNumberOfActions)
            {
                currentNumberOfActions = currentUnit.unitsActions.Count;
                Destroy(dummy);
                dummy = Instantiate(currentUnit.dummy, currentUnit.transform.position, currentUnit.transform.rotation);
                foreach (var action in currentUnit.unitsActions)
                {
                    action.DoOnDummy(dummy.transform);
                }
            }
            yield return new WaitForEndOfFrame();
        }

        Destroy(dummy);

        yield return null;
    }

    public void EndSelectionPhase()
    {
        if (currentActionButton != null)
        {
            currentActionButton.SetSelected(false);
        }
        if (selectedActionButton != null)
        {
            selectedActionButton.SetSelected(false);
        }
        HeroTurnManager.Instance.mode = SelectionMode.Normal;
        ActionManager.Instance.DropAction();
        menu.SetActive(false);
        playButton.gameObject.SetActive(false);
        PlacementManager.Instance.ResetSubscriptions();
        HeroTurnManager.Instance.DeselectTile();
        GameManager.Instance.UpdateGameState(GameState.FastPhase);
    }
}
