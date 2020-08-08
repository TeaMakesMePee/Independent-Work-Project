using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This script has the information of the tile
 * Active means that the tile is not occupied by a level object 
 * This is used so that the tile underneath a level object cannot be captured since its not visible
*/

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
