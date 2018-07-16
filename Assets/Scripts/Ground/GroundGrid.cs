﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundGrid : AbstractGrid {

    //state属性的访问控制
    public int State
    {
        get {
            if (unable) return 1;
            return state;
        }
        set
        {
            state = value;
            if(state==0)//如果上面没有东西就显示湿度
                material.mainTexture = textures[state];
        }
    }

    private int moisture = 1;
    public int Moisture
    {
        get { return moisture; }
        set
        {
            if (unable)
                return;
                
            if (increaseLock && value > moisture) return;
            if (decreaseLock && value < moisture) return;

            LevelManager.GetInstance().Score += value - moisture;

            moisture = value;
            if (moisture <= Global.lowestMoisture)
            {
                moisture = Global.lowestMoisture;
                LevelManager.GetInstance().Score -= 10;
            }
            if (moisture >= Global.highestMoisture)
            {
                moisture = Global.highestMoisture+1;
                LevelManager.GetInstance().Score -= 5;
                unable = true;
            }

            UpdateTexture();
        }
    }

    public bool increaseLock = false;//湿度不能再上升
    public bool decreaseLock= false;//湿度不能再下降
    private bool unable = false;//湿度超了之后这块土地就不再可用了
    //变干的速率
    private int dryRate = 50;
    public static List<Texture2D> textures;

    public override void InitFunction()
    {
       // InvokeRepeating("UpdateMoisture", dryRate, dryRate);
    }

    void UpdateMoisture()
    {
        Moisture -= 1;
    }
    void UpdateTexture()
    {
        //if (increaseLock && state == 0)
        //    material.mainTexture = textures[8];
        //else if (decreaseLock && state == 0)
        //    material.mainTexture = textures[9];
        //else 
        if (state == 0)
            material.mainTexture = textures[moisture + 1];
    }
    public override void InitTextures()
    {
        textures = new List<Texture2D>();
        textures.Add(Resources.Load("Textures/Ground/ground-1") as Texture2D);
        textures.Add(Resources.Load("Textures/Ground/ground0") as Texture2D);
        textures.Add(Resources.Load("Textures/Ground/ground1") as Texture2D);
        textures.Add(Resources.Load("Textures/Ground/ground2") as Texture2D);
        textures.Add(Resources.Load("Textures/Ground/ground3") as Texture2D);
        textures.Add(Resources.Load("Textures/Ground/ground4") as Texture2D);
        textures.Add(Resources.Load("Textures/Ground/ground5") as Texture2D);
        textures.Add(Resources.Load("Textures/Ground/groundUnable") as Texture2D);

        textures.Add(Resources.Load("Textures/IncreaseLock") as Texture2D);//8
        textures.Add(Resources.Load("Textures/DecreaseLock") as Texture2D);//9
    }

    public void AddIncreaseLock()
    {
        increaseLock = true;
        UpdateTexture();
    }
    public void AddDecreaseLock()
    {
        decreaseLock = true;
        UpdateTexture();

    }
}
