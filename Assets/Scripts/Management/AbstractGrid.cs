using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbstractGrid : MonoBehaviour {



    public Vector2Int position;

    public Material material;


    void Start()
    {
        material = GetComponent<Renderer>().material;
        InitTextures();
        InitFunction();
    }

    public virtual void InitTextures() { }
    public virtual void InitFunction() { }


}
