using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "Swap", menuName = "ScriptableObjects/Abilities/Swap")]
public class SwapAbility : AbilityEffectBase
{
    public override void Apply(WordSlotManager target, WordSlotManager own)
    {
        if (target.wordCount < 2) return;

        // Swap visual words
        var firstDisplay = target.curvedWords[^1];
        var lastDisplay = target.curvedWords[0];

        string firstWord = firstDisplay.textField.text;
        string lastWord = lastDisplay.textField.text;

        firstDisplay.SetWord(lastWord, firstDisplay.GetAngle());
        lastDisplay.SetWord(firstWord, lastDisplay.GetAngle());

        target.RecalculateLayout();
        target.firstWordFirstLetter = target.curvedWords[^1].GetComponent<TextMeshProUGUI>().text[0].ToString().ToLower();
    }
}