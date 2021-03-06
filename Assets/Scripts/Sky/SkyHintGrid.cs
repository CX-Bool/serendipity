﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyHintGrid : AbstractGrid
{

    /// <summary>
    /// -1为空，0为异常（红色）hint,1为正常（绿色)hint
    /// </summary>
    private int hintState = 0;
    public int HintState
    {
        get { return hintState; }
        set
        {
            hintState = value;
            if (hintState == -1)
                material.color = Color.clear;
            else
            {
                material.color = Color.white*0.6f;
                material.mainTexture = hintTextures[hintState];
            }
        }
    }

    private static List<Texture2D> hintTextures;

    public override void InitTextures()
    {
        hintTextures = new List<Texture2D>();

        hintTextures.Add(Resources.Load("Textures/hintUnavailable") as Texture2D);
        hintTextures.Add(Resources.Load("Textures/hintAvailable") as Texture2D);
    }


}
