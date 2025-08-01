using ntw.CurvedTextMeshPro;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
[RequireComponent(typeof(TextProOnACircle))]
public class AutoShrinkFontOnCircle : MonoBehaviour
{
    public float maxFontSize = 36f;
    public float minFontSize = 12f;

    private TextMeshProUGUI tmp;
    private TextProOnACircle circle;

    void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        circle = GetComponent<TextProOnACircle>();
    }

    void LateUpdate()
    {
        int charCount = tmp.textInfo.characterCount;
        if (charCount == 0) return;

        float maxAllowedSpacing = circle.m_arcDegrees / Mathf.Max(1, charCount);

        float spacing = Mathf.Min(circle.m_maxDegreesPerLetter, maxAllowedSpacing);

        // Calculate scale factor (proportional to spacing vs original)
        float spacingFactor = spacing / circle.m_maxDegreesPerLetter;

        float newFontSize = Mathf.Clamp(maxFontSize * spacingFactor, minFontSize, maxFontSize);

        if (Mathf.Abs(tmp.fontSize - newFontSize) > 0.01f)
        {
            tmp.fontSize = newFontSize;
            tmp.ForceMeshUpdate();
            circle.ForceUpdate();
        }
    }
}
