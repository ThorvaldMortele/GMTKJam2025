using TMPro;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextPopOnChange : MonoBehaviour
{
    private TextMeshProUGUI _text;
    private string _previousText;

    [Header("Pop Animation Settings")]
    public float popScale = 1.3f;
    public float popDuration = 0.2f;

    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
        _previousText = _text.text;
    }

    private void Update()
    {
        if (_text.text != _previousText)
        {
            TriggerPop();
            _previousText = _text.text;
        }
    }

    private void TriggerPop()
    {
        _text.transform.DOKill(); // kill any active tweens
        _text.transform.localScale = Vector3.one;

        Sequence pop = DOTween.Sequence();
        pop.Append(_text.transform.DOScale(popScale, popDuration * 0.5f).SetEase(Ease.OutQuad));
        pop.Append(_text.transform.DOScale(1f, popDuration * 0.5f).SetEase(Ease.InQuad));
    }
}
