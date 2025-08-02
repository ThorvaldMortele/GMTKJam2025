using UnityEngine;

[CreateAssetMenu(fileName = "Scissor", menuName = "ScriptableObjects/Abilities/Scissor")]
public class ScissorAbility : AbilityEffectBase
{
    public override void Apply(WordSlotManager target, WordSlotManager own)
    {
        if (target.wordCount == 0) return;

        string lastWord = target.curvedWords[0].textField.text;
        string scissored = lastWord.Remove(lastWord.Length-1);

        ReplaceLastWord(target, scissored);
    }
}
