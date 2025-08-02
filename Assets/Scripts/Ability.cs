using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability", menuName = "ScriptableObjects/Abilities")]
public class Ability : ScriptableObject
{
    public string Name;
    public string Description;
    public AbilityEffectBase Effect;
}

public interface IAbilityEffect
{
    void Apply(WordSlotManager target, WordSlotManager own = null);
}

