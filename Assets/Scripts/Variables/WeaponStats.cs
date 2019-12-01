using UnityEngine;

[CreateAssetMenu(menuName = "WeaponStats")]
public class WeaponStats : ScriptableObject
{
    public string name;
    public int shotsPerBurst;
    public int damage;
    public int reloading;
    public int maxAmmo;
    public Sprite weaponImage;
    public int actionCost;
    public int range;
}
