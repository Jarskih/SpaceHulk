using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarColor : MonoBehaviour
{
    public Color wounded;
    public Color healthy;
    private Stats stats;
    private SpriteRenderer sprite;

    void Start()
    {
        stats = GetComponent<Stats>();
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    public void ChangeSpriteColor(bool isWounded)
    {
        if (isWounded)
        {
            sprite.color = wounded;
        }
        else
        {
            sprite.color = healthy;
        }
    }

    void Update()
    {
        ChangeSpriteColor((stats.health <= 0));
    }
}
