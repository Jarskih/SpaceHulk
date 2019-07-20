using UnityEngine;

public class Stats : MonoBehaviour
{
    public int actionPoints;
    private IMove movement;
    private ComplexActions complexActions;
    private int health = 1;

    void Start()
    {
        movement = GetComponent<IMove>();
        complexActions = GetComponent<ComplexActions>();
    }

    public void Movement()
    {
        movement.Act();
    }

    public void Actions()
    {
        complexActions.Act();
    }

    public void UpdateMovementPoints(int change)
    {
        actionPoints = Mathf.Clamp(actionPoints + change, 0, 4);
    }
}
