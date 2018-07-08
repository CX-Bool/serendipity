using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbstractGrid : MonoBehaviour {


    /// <summary>
    /// 格子当前状态，0为空，1为有东西
    /// </summary>
    protected int state = 0;
  

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
