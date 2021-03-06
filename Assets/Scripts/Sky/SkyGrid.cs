﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyGrid : AbstractGrid
{
    //state属性的访问控制
    public int State
    {
        get { return state; }
        set
        {
            state = value;
            material.mainTexture = textures[state];
        }
    }

    private static List<Texture2D> textures;

    public override void InitTextures()
    {
        textures = new List<Texture2D>();

        textures.Add(Resources.Load("Textures/EmptySkyGrid") as Texture2D);
        textures.Add(Resources.Load("Textures/SkyGrid") as Texture2D);
    }
}
