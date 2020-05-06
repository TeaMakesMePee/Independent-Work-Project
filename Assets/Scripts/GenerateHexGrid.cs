using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GenerateHexGrid : MonoBehaviour
{
    public GameObject theTilePrefab;
    public Material theTileMat;
    public int gridWidth, gridHeight;
    public float tileScale;
    // Start is called before the first frame update
    void Start()
    {
        //float width = 2f * tileScale;
        //float height = Mathf.Sqrt(3f) * tileScale;
        //float downShift = 0.08f;
        //Vector3 centerOffset = new Vector3((gridWidth - 1) * width - (gridWidth - 1) * width * .25f, 0f, (gridHeight - 1) * height + height / 2f);
        //for (int x = 0; x < gridWidth; ++x)
        //{
        //    for (int z = 0; z < gridHeight; ++z)
        //    {
        //        GameObject newTile = Instantiate(theTilePrefab) as GameObject;
        //        GameObject tileMesh = newTile.transform.GetChild(0).gameObject;
        //        newTile.transform.position = new Vector3(x * width - x * width * .25f, -downShift, z * height + (x % 2 == 0 ? 0 : height / 2f)) - centerOffset * .5f;
        //        newTile.transform.localScale = new Vector3(tileScale, 1f, tileScale);
        //        newTile.transform.parent = gameObject.transform;
        //        tileMesh.GetComponent<MeshRenderer>().material = theTileMat;
        //    }
        //}

        //PrefabUtility.SaveAsPrefabAsset(gameObject, "Assets/Prefabs/Grids.prefab");
    }

    private void Update()
    {
        Text fpsCounter = GameObject.Find("FPSCounter").GetComponent<Text>();
        float fps = 1f / Time.deltaTime;
        fpsCounter.text = ((int)fps).ToString();
    }
}
