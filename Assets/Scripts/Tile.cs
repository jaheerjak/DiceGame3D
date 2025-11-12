using UnityEngine;

public class Tile : MonoBehaviour
{
    public int number;
    public TextMesh numberText;

    public void SetNumber(int n)
    {
        number = n;
        if (numberText != null) numberText.text = n.ToString();
    }

    void Reset()
    {
        if (GetComponentInChildren<TextMesh>() == null)
        {
            var go = new GameObject("NumberText");
            go.transform.parent = this.transform;
            go.transform.localPosition = Vector3.up * 0.6f;
            var tm = go.AddComponent<TextMesh>();
            tm.characterSize = 0.2f;
            tm.anchor = TextAnchor.MiddleCenter;
            numberText = tm;
        }
    }
}
