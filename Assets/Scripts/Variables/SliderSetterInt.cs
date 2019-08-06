using UnityEngine.UI;
using UnityEngine;

[ExecuteInEditMode]
public class SliderSetterInt : MonoBehaviour
{
    public Slider Slider;
    public IntVariable Variable;

    private void Update()
    {
        if (Slider != null && Variable != null)
            Slider.value = Variable.Value;
    }
}
