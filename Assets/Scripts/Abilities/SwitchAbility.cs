using UnityEngine;

[CreateAssetMenu(fileName = "Switch", menuName = "ScriptableObjects/Abilities/Switch")]
public class SwitchAbility : AbilityEffectBase
{
    public override void Apply(WordSlotManager target, WordSlotManager own)
    {
        if (target.wordCount == 0) return;

        string opponentlastWord = target.curvedWords[0].textField.text;
        string mylastWord = own.curvedWords[1].textField.text;

        ReplaceLastWord(target, mylastWord);
        ReplaceLastWord(own, opponentlastWord);
    }
}