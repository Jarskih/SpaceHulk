using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class TextSetter : MonoBehaviour
{
    public TextMeshProUGUI text;
    public IntVariable Variable;

    private void Update()
    {
        if (text != null && Variable != null)
            text.text = Variable.Value.ToString();
    }
}
