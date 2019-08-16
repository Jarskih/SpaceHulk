using UnityEngine;

public class ChangeSprite : MonoBehaviour
{
    public StatsListVariable enemyTargets;
    public Sprite normal;
    public Sprite targeted;

    private Stats _stats;
    private SpriteRenderer _spriteRenderer;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _stats = GetComponentInParent<Stats>();
    }

    void Update()
    {
        if (enemyTargets.list.Contains(_stats))
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
