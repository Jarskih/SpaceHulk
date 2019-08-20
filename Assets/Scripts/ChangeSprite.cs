using UnityEngine;

public class ChangeSprite : MonoBehaviour
{
    public StatsListVariable enemyTargets;
    public Sprite normal;
    public Sprite targeted;

    private Unit _unit;
    private SpriteRenderer _spriteRenderer;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _unit = GetComponentInParent<Unit>();
    }

    void Update()
    {
        if (enemyTargets.list.Contains(_unit))
        {
            SetTargeted();
        }
        else
        {
            SetNormal();
        }
    }
    void SetTargeted()
    {
        _spriteRenderer.sprite = targeted;
    }

    void SetNormal()
    {
        _spriteRenderer.sprite = normal;
    }
}
