using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyGrid : AbstractGrid
{
    /// <summary>
    /// 格子当前状态，0为空，1为有云彩，2为空且显示正常hint，3为空且显示异常hint
    /// </summary>
    private int status = 0;
    public int Status
    {
        get { return status; }
        set
        {
            status = value;
            material.mainTexture=textures[status];
        }
    }

    private static List<Texture2D> textures;

    // Update is called once per frame
    void Update () {
		
	}

    public override void InitTextures()
    {
        textures = new List<Texture2D>();
        textures.Add(Resources.Load("Textures/emptySkyGrid") as Texture2D);
        textures.Add(Resources.Load("Textures/SkyGrid") as Texture2D);
        textures.Add(Resources.Load("Textures/hintAvailable") as Texture2D);
        textures.Add(Resources.Load("Textures/hintUnavailable") as Texture2D);
    }


}
