using UnityEngine;

public class ChangeSprite : MonoBehaviour
{
    public StatsListVariable enemyTargets;
    public IntVariable targetIndex;

    private Unit _unit;
    private SpriteRenderer _targetRenderer;

    void Start()
    {
        _targetRenderer = GetComponentInChildren<TargetIconRenderer>().GetComponent<SpriteRenderer>();
        _unit = GetComponentInParent<Unit>();
    }

    void Update()
    {
        if (enemyTargets.list.Count == 0)
        {
            SetTargeted(false);
            return;
        }
        
        if (enemyTargets.list[targetIndex.Value] == _unit)
        {
            SetTargeted(true);
        }
        else
        {
            SetTargeted(false);
        }
    }
    void SetTargeted(bool value)
    {
        _targetRenderer.enabled = value;
    }
}
