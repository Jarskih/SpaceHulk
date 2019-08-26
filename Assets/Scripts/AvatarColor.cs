using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarColor : MonoBehaviour
{
    public Color wounded;
    public Color healthy;
    private Unit _unit;
    private SpriteRenderer sprite;

    void Start()
    {
        _unit = GetComponent<Unit>();
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    public void ChangeSpriteColor(bool isWounded)
    {
        if (isWounded)
        {
            sprite.color = wounded;
        }
    }

    void Update()
    {
        ChangeSpriteColor((_unit.health <= 0));
    }
}
