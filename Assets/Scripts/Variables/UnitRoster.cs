using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Units/UnitRoster")]
public class UnitRoster : ScriptableObject
{
    public List<UnitStats> unitList;
}
