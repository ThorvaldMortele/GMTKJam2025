using UnityEngine;

[CreateAssetMenu(fileName = "Addition", menuName = "ScriptableObjects/Abilities/Addition")]
public class AdditionAbility : AbilityEffectBase
{
    public override void Apply(WordSlotManager target, WordSlotManager own)
    {
        if (target.wordCount == 0) return;

        string lastWord = target.curvedWords[0].textField.text;
        string addition = lastWord + GetRandomLetter();

        ReplaceLastWord(target, addition);
    }

    private char GetRandomLetter()
    {
        const string alphabet = "abcdefghijklmnopqrstuvwxyz";
        System.Random rng = new();
        return alphabet[rng.Next(alphabet.Length)];
    }
}
