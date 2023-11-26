using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{

    public bool isEmpty;
    
    private int _x;
    private int _y;


    private void Start()
    {
        CheckIfTileIsEmpty();
    }

    public void CheckIfTileIsEmpty()
    {
        if (transform.childCount == 0  || transform.GetChild(0).name.Contains("gent"))
            isEmpty = true;
        else
            isEmpty = false;
    }

    public void Init(int x, int y)
    {
        _x = x;
        _y = y;
        name = "Tile_" + x + "_" + y;
    }

   
    public int _Cost
    {
        get
        {
            if (isEmpty)
                return 1;
            else
                return 0;
        }
    }
    public int _X => _x; 
    public int _Y => _y;
}