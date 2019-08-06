using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ListVariable : ScriptableObject
{
    public List<int> list = new List<int>();

    public void Reset()
    {
        for (int i = 0; i < list.Count; i++)
        {
            list[i] = 0;
        }
    }

    public int this[int i]
    {
        get => list[i];
        set => list[i] = value;
    }
}
