using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using view;
public class PlantOption : Option
{
    private static PlantOption instance;
    private void Awake()
    {
        instance = this;
        //  EnableSubscribe();
    }

    public static PlantOption GetInstance()
    {
        return instance;
    }
    public  PlantProperty plantProperty;
    public List<Vector2Int> position;

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

    //不一定被点到，所以有些东西被点到后再初始化
    protected override void InitSeletedItem() {
        imageOffset = new Vector3(-image.rectTransform.rect.width * 0.5f, image.rectTransform.rect.height * 0.5f,0);
    }
    protected override void Draging(Vector2 mousePos)
    {
        leftTop = image.rectTransform.position + imageOffset;

        DragingHandle(plantProperty, leftTop);//通知地面，提示可以放置植物的位置
    }

    protected override void EndDrag()
    {

        //传递图片左上角位置，注意有可能上下边沿超出
        EndDragHandle(plantProperty, leftTop);//通知sky
    }

}
