using System.Collections;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;

public class MeleeAttackCommand : ICommand
{
    private Unit _attacker;
    private Unit _defender;
    private readonly int _moveCost;

    public MeleeAttackCommand(Unit attacker, Unit defender)
    {
        _attacker = attacker;
        _defender = defender;
        _moveCost = attacker.APrules.enemyAttacking;
    }
    
    public void Execute()
    {
        _attacker.UpdateMovementPoints(-_moveCost);
        _defender.TakeDamage(_attacker.unitStats.meleeDamage);
    }

    public void Undo()
    {
        _attacker.UpdateMovementPoints(_moveCost);
        _defender.GainHealth(_attacker.unitStats.meleeDamage);
    }
}
