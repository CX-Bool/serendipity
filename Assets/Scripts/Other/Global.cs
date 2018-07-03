using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Global{

    #region 定义棋盘格子数
    public static int HorizonalGridNum = 12;
    public static int VerticalGridNum = 4;
        #endregion

  
    public enum CloudType
    {
        NORMAL//普通降雨的云彩块
    }

    public enum PlantType
    {
        CHI_SMALL,//生成“气”的植物，小中大
        CHI_MIDDLE,
        CHI_BIG
    }
    
    public enum TemplateType
    {
        SMALL_2_2,
        MIDDLE_3_2,
        BIG_4_3
    }

    public static void SwapVector3(Vector3 a,Vector3 b)
    {
        Vector3 tmp = a;
        a = b;
        b = tmp;
    }
}
