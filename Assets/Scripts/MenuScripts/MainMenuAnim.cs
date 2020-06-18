using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuAnim : MonoBehaviour
{
    public float interval = 0.01f;
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
        float hue = Random.Range(0f, 1f);
        hue = 0f;
        int rows = 19;
        int col = 22;
        int x = 0;
        foreach (Transform child in transform)
        {
            GameObject mesh = child.GetChild(0).gameObject;
            mesh.GetComponent<MeshRenderer>().material.color = Color.HSVToRGB(hue, Random.Range(0f, 1f), Random.Range(0f, 1f));
            child.gameObject.AddComponent<TileAnim>();
            child.gameObject.GetComponent<TileAnim>().Init(col - x / rows, interval);
            ++x;
        }
    }
}
