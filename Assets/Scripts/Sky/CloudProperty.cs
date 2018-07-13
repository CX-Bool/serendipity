using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudProperty//选项
{
    /// <summary>
    /// 块宽度
    /// </summary>
    public int width;
    /// <summary>
    /// 块长度
    /// </summary>
    public int height;
    /// <summary>
    /// 块详细数据,0表示该位置没有块，1表示该位置有块，
    /// 如果后面块种类增多，标志位可以依次往上增加
    /// </summary>
    public int[,] data;
    /// <summary>
    /// 图片
    /// </summary>
    public Texture2D texture;

    /// <summary>
    /// 云彩类型
    /// </summary>
    public Global.CloudType type;

    public virtual void EnableSubscribe() { }
    public virtual void DisableSubscribe() { }


}
