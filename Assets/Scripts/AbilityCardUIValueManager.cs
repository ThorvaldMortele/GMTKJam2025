using TMPro;
using UnityEngine;

public class AbilityCardUIValueManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _abilityNameText;
    [SerializeField] private TMP_Text _abilityDescriptionText;

    public void SetCardValue(string abilityName, string abilityDescription)
    {
        _abilityNameText.text = abilityName;
        _abilityDescriptionText.text = abilityDescription;
    }
}
