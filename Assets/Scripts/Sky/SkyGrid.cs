using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyGrid : AbstractGrid
{
    /// <summary>
    /// 是否可见
    /// </summary>
    private int active = 0;
    public int Active
    {
        get { return active; }
        set
        {
            active = active==0?1:0;
            material.mainTexture=textures[active];
        }
    }

    private List<Texture2D> textures;

    // Update is called once per frame
    void Update () {
		
	}

    public override void InitTextures()
    {
        textures = new List<Texture2D>();
        textures.Add(Resources.Load("Textures/emptySkyGrid") as Texture2D);
        textures.Add(Resources.Load("Textures/SkyGrid") as Texture2D);
    }
}
