using TMPro;
using UnityEngine;

public class TextSetter : MonoBehaviour
{
    public TextMeshProUGUI text;
    public IntVariable Variable;
    public string prefix;
    public string affix;

    private void Start()
    {
        if (text == null)
        {
            Debug.LogError("Text is null");
        }

        if (Variable == null)
        {
            Debug.LogError("Variable is null");
        }
    }

    private void Update()
    {
        if (text != null && Variable != null)
        {
            text.text = prefix + Variable.Value.ToString() + affix;
        }
    }
}
