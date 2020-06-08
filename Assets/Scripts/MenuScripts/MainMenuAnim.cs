using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuAnim : MonoBehaviour
{
    private bool d = false;
    [Range(0, 1)]
    public float h, s, v, hm;

    //private void OnValidate()
    //{
    //    foreach (Transform child in transform)
    //    {
    //        GameObject mesh = child.GetChild(0).gameObject;
    //        mesh.GetComponent<MeshRenderer>().material.color = Color.HSVToRGB(Random.Range(0.5f, 0.5f), Random.Range(0f, s), Random.Range(0f, v));
    //    }
    //}
    private void Start()
    {
        foreach (Transform child in transform)
        {
            GameObject mesh = child.GetChild(0).gameObject;
            //mesh.GetComponent<MeshRenderer>().material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
            mesh.GetComponent<MeshRenderer>().material.color = Color.HSVToRGB(Random.Range(0f, 1f), Random.Range(0f, 0.7f), Random.Range(0f, 1f));
            child.gameObject.AddComponent<TileAnim>();
        }
    }
}
