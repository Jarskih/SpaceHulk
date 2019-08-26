using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "WeaponStats")]
public class WeaponStats : ScriptableObject
{
    public string name;
    public int shotsPerBurst;
    public int damage;
}
