using UnityEngine;

[CreateAssetMenu(menuName = "Units/UnitStats")]
public class UnitStats : ScriptableObject
{
    public string unitName;
    public int maxHealth;
    public int maxAP;
}
