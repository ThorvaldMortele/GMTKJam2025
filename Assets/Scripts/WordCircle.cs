using TMPro;
using UnityEngine;

public class WordCircle : MonoBehaviour
{
    public string text = "TEXT ALONG A CIRCLE";
    public float radius = 100f;
    public float characterSpacingDegrees = 10f;
    public TMP_FontAsset font;
    public float fontSize = 36f;

    void Start()
    {
        DrawTextOnCircle();
    }

    void DrawTextOnCircle()
    {
        float angleOffset = -(text.Length - 1) * characterSpacingDegrees / 2f;

        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];

            GameObject letterObj = new GameObject("Char_" + c);
            letterObj.transform.SetParent(transform);

            var tmp = letterObj.AddComponent<TextMeshPro>();
            tmp.text = c.ToString();
            tmp.fontSize = fontSize;
            tmp.font = font;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.rectTransform.pivot = new Vector2(0.5f, 0.5f);

            float angle = angleOffset + i * characterSpacingDegrees;
            float rad = angle * Mathf.Deg2Rad;

            Vector3 pos = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0) * radius;
            letterObj.transform.localPosition = pos;

            // Rotate the letter so it's tangent to the circle
            letterObj.transform.localRotation = Quaternion.Euler(0, 0, angle + 90);
        }
    }
}
