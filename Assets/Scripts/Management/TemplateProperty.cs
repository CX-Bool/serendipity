using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// CloudElimProperty和PlantProperty使用相同的模板因此都从TemplateProperty继承，
/// 选项云彩是不规则的形状，和前者行为不同因此不继承TemplateProperty！
/// </summary>
public class TemplateProperty {

    /// <summary>
    /// 块宽度
    /// </summary>
    public int width;
    /// <summary>
    /// 块长度
    /// </summary>
    public int height;
    /// <summary>
    ///// 块详细数据
    ///// </summary>
    //public int[,] data;
    /// <summary>
    /// 云彩消除和植物生长使用相同的模板类型，目前有小中大三种
    /// </summary>
    public Global.TemplateType templateType;

}
