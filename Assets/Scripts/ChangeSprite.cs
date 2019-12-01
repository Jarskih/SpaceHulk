using UnityEngine;

public class ChangeSprite : MonoBehaviour
{
    public StatsListVariable enemyTargets;
    public IntVariable targetIndex;
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
        if (enemyTargets.list.Count == 0)
        {
            SetNormal();
            return;
        }
        
        if (enemyTargets.list[targetIndex.Value] == _unit)
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
