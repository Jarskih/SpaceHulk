using System.Collections;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;

public class MeleeAttackCommand : ICommand
{
    private Stats _attacker;
    private Stats _defender;
    private readonly int _moveCost;

    public MeleeAttackCommand(Stats attacker, Stats defender)
    {
        _attacker = attacker;
        _defender = defender;
        _moveCost = attacker.APrules.enemyAttacking;
    }
    
    public void Execute()
    {
        _attacker.UpdateMovementPoints(-_moveCost);
        _defender.TakeDamage(1);
    }

    public void Undo()
    {
        _attacker.UpdateMovementPoints(_moveCost);
        _defender.GainHealth(1);
    }
}
