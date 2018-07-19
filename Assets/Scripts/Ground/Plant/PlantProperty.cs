        using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantProperty : TemplateProperty
{
    public string name;
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
    /// <summary>
    /// 介绍
    /// </summary>
    public string comment;
    /// <summary>
    /// 出处典籍
    /// </summary>
    public string provenance;
    /// <summary>
    /// 出处文本
    /// </summary>
    public string reference;
    /// <summary>
    /// 技能描述
    /// </summary>
    public string skill;
 

}
