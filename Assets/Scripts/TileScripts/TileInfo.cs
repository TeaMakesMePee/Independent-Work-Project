using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInfo : MonoBehaviour
{
    [SerializeField]
    public bool active;
    [SerializeField]
    public int index;

    public void SetActive(bool _active)
    {
        active = _active;
    }

    public void SetIndex(int _index)
    {
        index = _index;
    }
}
