using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Mirror", menuName = "ScriptableObjects/Abilities/Mirror")]
public class MirrorAbility : AbilityEffectBase
{
    public override void Apply(WordSlotManager target, WordSlotManager own)
    {
        if (target.wordCount == 0) return;

        string lastWord = target.curvedWords[0].textField.text;
        string reversed = new string(lastWord.Reverse().ToArray());

        ReplaceLastWord(target, reversed);
    }
}
