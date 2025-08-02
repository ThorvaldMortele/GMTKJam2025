using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Brainfart", menuName = "ScriptableObjects/Abilities/Brainfart")]
public class BrainfartAbility : AbilityEffectBase
{
    public override void Apply(WordSlotManager target, WordSlotManager own)
    {
        if (target.wordCount == 0) return;

        if (target.IsCPU) GameManager.Instance.CPU.TriggerBrainfart();
        else target.InputManager.TriggerBrainfart();
    }
}
