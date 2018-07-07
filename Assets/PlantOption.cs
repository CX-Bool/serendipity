﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using view;
public class PlantOption : Option
{

    private PlantProperty plantProperty;
    private List<Vector2Int> position;

    Vector2 leftTop;//当前在拖动的云彩图片的左上角
    Vector3 imageOffset;//图片左上角到图片中心的偏移量

    #region 委托
    /// <param name="plantProperty">当前拖动的云彩</param>
    /// <param name="position">图片左上角的世界坐标</param>
    public delegate void Drag(PlantProperty plantProperty, Vector2 position);
    /// <summary>
    /// 1、通知HUDManager销毁当前UI并更新Options
    /// 2、通知Sky有一块云彩被放置了
    /// </summary>
    public static Drag EndDragHandle;
    /// <summary>
    ///通知Sky显示Hover的效果
    /// </summary>
    public static Drag DragingHandle;
    #endregion


    protected override void Draging(Vector2 mousePos)
    {
        leftTop = image.rectTransform.position + imageOffset;

        DragingHandle(plantProperty, leftTop);//通知天空，划过的区域产生特效等
    }

    protected override void EndDrag()
    {

        //传递图片左上角位置，注意有可能上下边沿超出
        EndDragHandle(plantProperty, leftTop);//通知sky
    }

}
