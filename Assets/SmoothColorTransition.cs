using UnityEngine;

public class SmoothColorTransition : MonoBehaviour
{
    [SerializeField] private float _lowAlpha;
    [SerializeField] private float _highAlpha;
    [SerializeField] private float _currentAlpha;
    [SerializeField] private float _speed;
    [SerializeField] private bool _decreasing;
    private SpriteRenderer _spriteRenderer;

    // Start is called before the first frame update
    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    private void Update()
    {
        var color = _spriteRenderer.color;
        if (_decreasing)
        {
            color.a += Time.deltaTime * -_speed;
            if (color.a < _lowAlpha)
            {
                _decreasing = false;
            }
            _spriteRenderer.color = new Color(color.r, color.g, color.b, color.a);
        }
        else
        {
            color.a += Time.deltaTime * _speed;
            if (color.a > _highAlpha)
            {
                _decreasing = true;
            }
            _spriteRenderer.color = new Color(color.r, color.g, color.b, color.a);
        }
    }
}
