﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Runtime.CompilerServices;
using UnityEngine.Events;
using System.Linq;

public class GenerateHexGrid : MonoBehaviourPunCallbacks
{
    public GameObject theTilePrefab;
    public Material theTileMat;
    public int gridWidth, gridHeight;
    public float tileScale;
    public List<GameObject> hexGrids = new List<GameObject>();
    private List<int> toColor = new List<int>();
    private List<List<int>> toColorList = new List<List<int>>();
    private List<bool> visited = new List<bool>();
    private List<int> counts;
    private int stepCounter;
    private int prevIndex = -1;
    private bool overShot;
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
        if (visited.Count != hexGrids.Count)
        {
            visited = new List<bool>();
            for (int x = 0; x < hexGrids.Count; ++x)
            {
                visited.Add(false);
            }
        }
        
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
        if (prevIndex == -1)
        {
            photonView.RPC("ApplyMaterialToHex", RpcTarget.AllBuffered, index);
            prevIndex = index;
        }
        else if (prevIndex != index)
        {
            prevIndex = index;
            photonView.RPC("ApplyMaterialToHex", RpcTarget.AllBuffered, index);
            if (row >= 0 && row < gridWidth)
            {
                if (column >= 0 && column < gridHeight)
                {
                    List<Vector2> adjTiles = new List<Vector2>();
                    adjTiles.Add(new Vector2(row - 1, column));
                    adjTiles.Add(new Vector2(row, column - 1));
                    adjTiles.Add(new Vector2(row + 1, column));
                    adjTiles.Add(new Vector2(row, column + 1));
                    if (row % 2 == 1)
                    {
                        adjTiles.Add(new Vector2(row - 1, column + 1));
                        adjTiles.Add(new Vector2(row + 1, column + 1));
                    }
                    else
                    {
                        adjTiles.Add(new Vector2(row - 1, column - 1));
                        adjTiles.Add(new Vector2(row + 1, column - 1));
                    }
                    toColorList = new List<List<int>>();
                    counts = new List<int>();

                    for (int x = 0; x < adjTiles.Count; ++x)
                    {
                        overShot = false;
                        visited = Enumerable.Repeat(false, hexGrids.Count).ToList();
                        toColor = new List<int>();
                        stepCounter = 0;

                        Transform theMatMesh = hexGrids[row * gridHeight + column].transform.GetChild(0);
                        Material theMat = theMatMesh.GetComponent<MeshRenderer>().material;
                        Transform theMesh = hexGrids[(int)(adjTiles[x].x) * gridHeight + (int)(adjTiles[x].y)].transform.GetChild(0);
                        Material nextMat = theMesh.GetComponent<MeshRenderer>().material;
                        if ((int)(adjTiles[x].x) >= 0 && (int)(adjTiles[x].x) < gridWidth)
                        {
                            if ((int)(adjTiles[x].y) >= 0 && (int)(adjTiles[x].y) < gridHeight)
                            {
                                int i = (int)(adjTiles[x].x) * gridHeight + (int)(adjTiles[x].y);
                                if (nextMat.name != theMat.name)
                                {
                                    toColor.Add(i);
                                    stepCounter++;
                                    visited[index] = true;
                                    visited[i] = true;
                                    FloodFill((int)(adjTiles[x].x), (int)(adjTiles[x].y), theMat);
                                    if (!overShot)
                                    {
                                        toColorList.Add(toColor);
                                        counts.Add(stepCounter);
                                    }
                                }
                            }
                        }
                    }

                    int temp = 999;
                    int tempind = -1;
                    for (int abc = 0; abc < counts.Count; ++abc)
                    {
                        if (counts[abc] < temp)
                        {
                            //Debug.LogError("lesser: " + counts[abc]);
                            temp = counts[abc];
                            tempind = abc;
                        }
                    }
                    //Debug.LogError("here");
                    //Debug.LogError(counts.Count);

                    if (counts.Count > 0)
                    {
                        List<int> tempIntList = toColorList[tempind];
                        for (int xx = 0; xx < tempIntList.Count; ++xx)
                            photonView.RPC("ApplyMaterialToHex", RpcTarget.AllBuffered, tempIntList[xx]);
                        //photonView.RPC("ApplyFloodFill", RpcTarget.AllBuffered, tempIntList);
                    }

                    //for (int f = 0; f < toColorList[tempind].Count; ++f)
                    //{
                    //    Debug.LogError("filling..: " + toColorList[tempind][f]);
                    //}
                    //List<int> tempIntList = toColorList[tempind];
                    //for (int xx = 0; xx < tempIntList.Count; ++xx)
                    //{
                    //    Debug.LogError("list ind: " + tempIntList[xx]);
                    //}
                        //photonView.RPC("ApplyFloodFill", RpcTarget.AllBuffered, index, toColorList[tempind]);
                    //photonView.RPC("ApplyMaterialToHex", RpcTarget.AllBuffered, index);
                }
            }
            
        }
    }

    private void FloodFill(int row, int col, Material theMat)
    {
        List<Vector2> adjTiles = new List<Vector2>();
        adjTiles.Add(new Vector2(row - 1, col));
        adjTiles.Add(new Vector2(row, col - 1));
        adjTiles.Add(new Vector2(row + 1, col));
        adjTiles.Add(new Vector2(row, col + 1));
        if (row % 2 == 1)
        {
            adjTiles.Add(new Vector2(row - 1, col + 1));
            adjTiles.Add(new Vector2(row + 1, col + 1));
        }
        else
        {
            adjTiles.Add(new Vector2(row - 1, col - 1));
            adjTiles.Add(new Vector2(row + 1, col - 1));
        }

        for (int x = 0; x < adjTiles.Count; ++x)
        {
            if (stepCounter > gridWidth * gridHeight / 2 - Math.Min(gridWidth, gridHeight))
            {
                overShot = true;
                return;
            }
            if ((int)(adjTiles[x].x) >= 0 && (int)(adjTiles[x].x) < gridWidth)
            {
                if ((int)(adjTiles[x].y) >= 0 && (int)(adjTiles[x].y) < gridHeight)
                {
                    int index = (int)(adjTiles[x].x) * gridHeight + (int)(adjTiles[x].y);
                    Transform theMesh = hexGrids[index].transform.GetChild(0);
                    Material nextMat = theMesh.GetComponent<MeshRenderer>().material;

                    if (!visited[index])
                    {
                        if (nextMat.name != theMat.name)
                        {
                            visited[index] = true;
                            toColor.Add(index);
                            stepCounter++;
                            FloodFill((int)(adjTiles[x].x), (int)(adjTiles[x].y), theMat);
                        }
                    }
                }
            }
        }
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
