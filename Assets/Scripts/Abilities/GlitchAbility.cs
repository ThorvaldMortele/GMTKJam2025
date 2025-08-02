using UnityEngine;

[CreateAssetMenu(fileName = "Glitch", menuName = "ScriptableObjects/Abilities/Glitch")]
public class GlitchAbility : AbilityEffectBase
{
    public override void Apply(WordSlotManager target, WordSlotManager own)
    {
        if (target.wordCount == 0) return;

        string lastWord = target.curvedWords[0].textField.text;
        char[] chars = lastWord.ToCharArray();
        System.Random rng = new();
        for (int i = chars.Length - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (chars[i], chars[j]) = (chars[j], chars[i]);
        }

        ReplaceLastWord(target, new string(chars));
    }
}
