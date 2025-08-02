using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class CurvedWordDisplay : MonoBehaviour
{
    public float radius = 200f;
    public float centerAngle = 0f;

    private TextMeshProUGUI tmp;

    void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
    }

    public void SetWord(string word, float angle)
    {
        tmp.text = word.ToUpper();
        centerAngle = angle;
        ApplyCurve();
    }

    public void SetAngle(float angle)
    {
        centerAngle = angle;
        ApplyCurve();
    }

    public void ToggleWordVisible(int visible)
    {
        tmp.alpha = visible;
    }

    public float GetAngle()
    {
        return centerAngle;
    }

    public float GetArcAngle()
    {
        tmp.ForceMeshUpdate();
        float width = tmp.preferredWidth;
        return width / (2 * Mathf.PI * radius) * 360f;
    }

    public void ApplyCurve()
    {
        tmp.ForceMeshUpdate();
        TMP_TextInfo textInfo = tmp.textInfo;
        int charCount = tmp.text.Length;
        if (charCount == 0) return;

        float arc = GetArcAngle();
        float startAngle = centerAngle - arc / 2f;

        for (int i = 0; i < charCount; i++)
        {
            float t = (i + 0.5f) / charCount;
            float charAngle = (startAngle + arc * (1 - t)) * Mathf.Deg2Rad;
            float x = Mathf.Cos(charAngle) * radius;
            float y = Mathf.Sin(charAngle) * radius;
            Vector3 offset = new Vector3(x, y, 0);
            float rotation = Mathf.Atan2(y, x) * Mathf.Rad2Deg - 90f;

            int matIndex = textInfo.characterInfo[i].materialReferenceIndex;
            int vertexIndex = textInfo.characterInfo[i].vertexIndex;
            Vector3[] verts = textInfo.meshInfo[matIndex].vertices;
            Vector3 charMidBaseline = (verts[vertexIndex + 0] + verts[vertexIndex + 2]) / 2f;
            Vector3 offsetToMid = offset - charMidBaseline;
            Quaternion rot = Quaternion.Euler(0, 0, rotation);

            for (int j = 0; j < 4; j++)
                verts[vertexIndex + j] = rot * (verts[vertexIndex + j] - charMidBaseline) + offset;
        }
        tmp.UpdateVertexData();
    }
}
