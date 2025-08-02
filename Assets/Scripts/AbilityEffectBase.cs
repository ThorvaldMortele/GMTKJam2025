using UnityEngine;

public abstract class AbilityEffectBase : ScriptableObject, IAbilityEffect
{
    public abstract void Apply(WordSlotManager target, WordSlotManager own);

    protected void ReplaceLastWord(WordSlotManager slotManager, string newWord)
    {
        slotManager.RemoveLastWord();
        slotManager.AddWord(newWord, true);
    }
}
