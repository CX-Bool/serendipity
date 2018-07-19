using System.Collections;
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
            if (state==1)
                return;

            #region 湿度锁
            if (increaseLock && value > increaseLockVal) return;
            if (decreaseLock && value < decreaseLockVal) return;
            #endregion

            #region 计分
            bool overWet = false;
            bool overDry = false;
            if (value <= Global.lowestMoisture && overDry==false)
            {
                moisture = Global.lowestMoisture;
                LevelManager.GetInstance().Score -= 10;
                overDry = true;
            }
            else if (overDry && value > Global.lowestMoisture)
            {
                LevelManager.GetInstance().Score += 10;
                overDry = false;
            }

            if (value > Global.highestMoisture && overWet == false)
            {
                moisture = Global.highestMoisture;
                LevelManager.GetInstance().Score -= 1;
                overWet = true;
            }
            else if (overWet && value <= Global.highestMoisture)
            {
                LevelManager.GetInstance().Score += 1;
                overWet = false;
            }

            if (value >= Global.lowestMoisture && value <= Global.highestMoisture)
            {
                LevelManager.GetInstance().Score += value - moisture;
                moisture = value;
            }
            #endregion
            UpdateTexture();
        }
    }

    public bool increaseLock = false;//湿度不能再上升
    public bool decreaseLock= false;//湿度不能再下降
    public int increaseLockVal = -1;
    public int decreaseLockVal = -1;
    private bool unable = false;//湿度超了之后这块土地就不再可用了
    //变干的速率
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
        increaseLockVal = moisture;
        UpdateTexture();
    }
    public void AddDecreaseLock()
    {
        decreaseLock = true;
        decreaseLockVal = moisture;
        UpdateTexture();

    }
}
