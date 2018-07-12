using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundHintGrid : AbstractGrid {

    public enum State{
        
        Abnormal,
        Normal,
        SkyShadow,
        Empty
    }
    /// <summary>

    /// 0为异常（红色）hint
    /// 1为正常（绿色)hint
    /// 2为天空阴影
    /// 3为空
    /// </summary>
    private State hintState = State.Empty;
    public State HintState
    {
        get { return hintState; }
        set
        {
            hintState = value;
            if (hintState == State.Empty)
                material.color = Color.clear;
            else
            {
                material.color = Color.white*0.5f;
                material.mainTexture = hintTextures[(int)hintState];
            }
        }
    }

    private static List<Texture2D> hintTextures;

    public override void InitTextures()
    {
        hintTextures = new List<Texture2D>();

        hintTextures.Add(Resources.Load("Textures/hintUnavailable") as Texture2D);
        hintTextures.Add(Resources.Load("Textures/hintAvailable") as Texture2D);
        hintTextures.Add(Resources.Load("Textures/hintGround") as Texture2D);

    }
  
}
