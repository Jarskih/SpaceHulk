using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitPortrait : MonoBehaviour
{
    public Sprite _unitPortrait;
    public Sprite _unitDead;
    private TurnSystem _turnSystem;
    
    // Start is called before the first frame update
    void Start()
    {
        _turnSystem = FindObjectOfType<TurnSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < _turnSystem.players.Count; i++)
        {
            if (_turnSystem.players[i].health <= 0)
            {
                gameObject.transform.GetChild(i).GetComponent<Image>().sprite = _unitDead;
            }
            else
            {
                gameObject.transform.GetChild(i).GetComponent<Image>().sprite = _unitPortrait;
            }
        }
    }
}
