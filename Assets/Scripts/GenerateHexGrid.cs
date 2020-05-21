using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class GenerateHexGrid : MonoBehaviourPunCallbacks
{
    public GameObject theTilePrefab;
    public Material theTileMat;
    public int gridWidth, gridHeight;
    public float tileScale;
    public List<GameObject> hexGrids = new List<GameObject>();
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
        //        hexGrids.Add(newTile);
        //    }
        //}

        //PrefabUtility.SaveAsPrefabAsset(gameObject, "Assets/Prefabs/Grids2.prefab");
    }

    public void ChangeHexColor(Vector3 playerPos)
    {
        Vector3ToHexGrid(playerPos);
        //GetClosestTile(playerPos);
    }

    private void Vector3ToHexGrid(Vector3 playerPos)
    {
        float height = 2f * tileScale;
        float gHeight = height * .75f;
        float width = Mathf.Sqrt(3f) * tileScale;
        float halfWidth = width * .5f;
        Vector3 centerOffset = new Vector3((gridWidth - 1) * height - (gridWidth - 1) * height * .25f, 0f, (gridHeight - 1) * width + width / 2f);
        //This makes sure the top left of the grid is set to 0,0,0.
        Vector3 newPlayerPos = playerPos + centerOffset * 0.5f + new Vector3(height * .5f, 0f, width * .5f); //Added the offset to the player position that i initially subtracted from the grids
        int row = (int)(newPlayerPos.x / gHeight);
        int column;

        if (row % 2 == 1)
        {
            column = (int)((newPlayerPos.z - halfWidth) / width);
        }
        else
        {
            column = (int)(newPlayerPos.z / width);
        }

        double relVertBox = newPlayerPos.x - (row * gHeight);
        
        double relHoriBox;

        if (row % 2 == 1)
            relHoriBox = (newPlayerPos.z - (column * width)) - halfWidth;
        else
            relHoriBox = newPlayerPos.z - (column * width);

        float c = height * .25f;
        float m = c / halfWidth;

        if (relVertBox < (-m * relHoriBox) + c)
        {
            if (row % 2 == 0)
            { 
                column--;
            }
            row--;
        }
        else if (relVertBox < (m * relHoriBox) - c)
        {
            if (row % 2 == 1)
            {
                column++;
            }
            row--;
        }

        int index = row * gridHeight + column;
        photonView.RPC("ApplyMaterialToHex", RpcTarget.AllBuffered, index);
    }

    [PunRPC]
    private void ApplyMaterialToHex(int index)
    {
        Material theMat = Resources.Load("Material/Blue", typeof(Material)) as Material;
        Transform theMesh = hexGrids[index].transform.GetChild(0);
        theMesh.GetComponent<MeshRenderer>().material = theMat;
    }

    private void GetClosestTile(Vector3 playerPos)
    {
        float dist = 999999f;
        int index = 0;
        for (int x = 0; x < hexGrids.Count; ++x)
        {
            float newDist = Vector3.Distance(hexGrids[x].transform.position, playerPos);
            if (newDist < dist)
            {
                dist = newDist;
                index = x;
            }
        }

        Debug.LogError("For loop index: " + index);
    }
}
