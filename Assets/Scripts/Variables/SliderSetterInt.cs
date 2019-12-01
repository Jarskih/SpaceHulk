using UnityEngine.UI;
using UnityEngine;

[ExecuteInEditMode]
public class SliderSetterInt : MonoBehaviour
{
    public Slider Slider;
    public IntVariable Variable;
    public IntVariable MaxVariable;

    private void Update()
    {
        if (Slider != null && Variable != null)
            Slider.value = Variable.Value;

        if (Slider != null && MaxVariable != null)
            Slider.maxValue = MaxVariable.Value;
    }
}
