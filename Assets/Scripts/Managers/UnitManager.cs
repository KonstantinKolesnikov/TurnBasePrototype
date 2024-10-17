using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;

    public UnitBase SelectedUnit;
    public UnitBase[] units;

    private void Awake()
    {
        Instance = this;
        units = (UnitBase[])FindObjectsOfType(typeof(UnitBase));
    }

    public void SetSelectedUnit(UnitBase unit)
    {
        SelectedUnit = unit;
    }

    public void ClearUnitsActions()
    {
        foreach (UnitBase unit in units)
        {
            foreach(ActionBase action in unit.unitsActions)
            {
                if (action != null) Destroy(action.gameObject);
            }
            unit.unitsActions.Clear();
            unit.speedSum = 0;
        }
    }

    public void GetMoves()
    {
        return;
    }
}
