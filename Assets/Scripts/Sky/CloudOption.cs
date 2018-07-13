using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using view;
public class CloudOption : Option
{
    private static CloudOption instance;
    private void Awake()
    {
        instance = this;
        //  EnableSubscribe();
    }

    public static CloudOption GetInstance()
    {
        return instance;
    }


    private CloudProperty cloudProperty;//必须知道在拖动的云彩的形状

   
    #region 委托
    /// <param name="cloudProperty">当前拖动的云彩</param>
    /// <param name="position">图片左上角的世界坐标</param>
    public delegate void Drag(CloudProperty cloudProperty, Vector2 position);
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

   
    protected override void InitSeletedItem()
    {
        int index = HUDManager.GetInstance().cloudOptImageList.FindIndex((RawImage s) => s == image);
        if (index < 0) Debug.Log("cloudProperty in <Option> is NULL");
        cloudProperty = CloudOptManager.GetInstance().optionList[index];

        //imgReduceScale = (256f/imgRect.rect.width);
        //imgNormalScale = (256f/imgRect.rect.width);

        imageOffset = new Vector3(-image.rectTransform.rect.width * 0.5f*image.rectTransform.localScale.x, 
            image.rectTransform.rect.height * 0.5f * image.rectTransform.localScale.y, 0);
    }
   
    protected override void Draging(Vector2 mousePos) {
        leftTop = image.rectTransform.position + imageOffset;
        DragingHandle(cloudProperty, leftTop);//通知天空，划过的区域产生特效等
    }

    protected override void EndDrag() {

        //传递图片左上角位置，注意有可能上下边沿超出
        EndDragHandle(cloudProperty,leftTop);//通知sky
    }
    
}
