using UnityEngine;

[CreateAssetMenu(fileName = "Remove", menuName = "ScriptableObjects/Abilities/Remove")]
public class RemoveAbility : AbilityEffectBase
{
    public override void Apply(WordSlotManager target, WordSlotManager own)
    {
        if (target.wordCount == 0) return;

        target.RemoveLastWord();
    }
}
