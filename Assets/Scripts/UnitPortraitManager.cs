using UnityEngine;
using UnityEngine.UI;

public class UnitPortraitManager : MonoBehaviour
{
    public Unit unit;
    private Image image;
    private UnitPortrait unitPortrait;
    private TurnSystem _turnSystem;
    private PlayerInteractions _pi;
    public Image activeIndicator;

    // Start is called before the first frame update
    private void Start()
    {
        image = GetComponent<Image>();
        unitPortrait = GetComponentInParent<UnitPortrait>();
        _pi = FindObjectOfType<PlayerInteractions>();
        _turnSystem = FindObjectOfType<TurnSystem>();
    }

    public void SelectUnit()
    {
        _pi.SetActiveUnit(unit);
    }

    // Update is called once per frame
    private void Update()
    {
        if (unit.health <= 0)
        {
            image.sprite = unitPortrait._unitDead;
        }
        else
        {
            image.sprite = unitPortrait._unitPortrait;
        }

        if (unit == _pi.activeUnit)
        {
            activeIndicator.gameObject.SetActive(true);
        }
        else
        {
            activeIndicator.gameObject.SetActive(false);
        }

        if (unit.actionPoints <= 0)
        {
            image.color = Color.gray;
        }
        else
        {
            image.color = Color.white;
        }
    }
}
