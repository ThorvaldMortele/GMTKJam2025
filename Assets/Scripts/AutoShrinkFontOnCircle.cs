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

    }
}
