using TMPro;
using UnityEngine;

public class StringTextSetter : MonoBehaviour
{
    public TextMeshProUGUI text;
    public StringVariable Variable;

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
            text.text = Variable.Value;
        }
    }
}
