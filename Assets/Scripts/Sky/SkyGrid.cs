using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyGrid : AbstractGrid
{
    /// <summary>
    /// 格子当前状态，0为空，1为有云彩
    /// </summary>
    private int state = 0;
    public int State
    {
        get { return state; }
        set
        {
            state = value;
            material.mainTexture=textures[state];
        }
    }

    private static List<Texture2D> textures;

    public override void InitTextures()
    {
        textures = new List<Texture2D>();

        textures.Add(Resources.Load("Textures/emptySkyGrid") as Texture2D);
        textures.Add(Resources.Load("Textures/SkyGrid") as Texture2D);
    }
}
