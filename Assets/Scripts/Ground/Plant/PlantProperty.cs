        using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantProperty : TemplateProperty
{
    /// <summary>
    /// 图片
    /// </summary>
    public Texture2D texture;
    /// <summary>
    /// 植物类型
    /// </summary>
    public Global.PlantType type;
    /// <summary>
    /// 长出植物需要满足的湿度
    /// </summary>
    [Range(-1,5)]
    public int moisture;

}
