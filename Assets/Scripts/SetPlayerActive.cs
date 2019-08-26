using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPlayerActive : MonoBehaviour
{
    public void SetUnitActive(List<Unit> p_players, Unit p_player)
    {
        foreach (var player in p_players)
        {
            player.setActive.SetUnitInactive();
        }
        p_player.setActive?.SetUnitActive();
    }

    public void DisableActiveUnits(List<Unit> p_players)
    {
        foreach (var player in p_players)
        {
            player.setActive.SetUnitInactive();
        }
    }
}
