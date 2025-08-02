using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Fishy", menuName = "ScriptableObjects/Abilities/Fishy")]
public class FishyAbility : AbilityEffectBase
{
    public override void Apply(WordSlotManager target, WordSlotManager own)
    {
        if (target.wordCount < 1) return;

        string lastLetter;
        
        if (own.InputManager.slotManager.wordCount == 0)
        {
            lastLetter = ((char)('a' + Random.Range(0, 26))).ToString();
        }
        else
        {
            lastLetter = own.InputManager.slotManager.GetLastLetter();
        }
        
        var hintword = own.InputManager.DictionaryLoader.WordSet
            .Where(w => w.Length > 1
             && w.StartsWith(lastLetter)
             && !GameManager.Instance.AllUsedWords.Contains(w))
            .ToList().OrderBy(_ => Random.value)
            .FirstOrDefault();

        own.InputManager.SetHintText(hintword);
    }
}
