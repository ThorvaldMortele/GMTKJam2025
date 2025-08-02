using UnityEngine;

[CreateAssetMenu(fileName = "Plural", menuName = "ScriptableObjects/Abilities/Plural")]
public class PluralAbility : AbilityEffectBase
{
    public override void Apply(WordSlotManager target, WordSlotManager own)
    {
        if (target.wordCount == 0) return;

        string lastWord = target.curvedWords[0].textField.text;
        string plural = lastWord + "s";

        ReplaceLastWord(target, plural);
    }
}
