using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;
using view;
public class Sky : MonoBehaviour {

    #region 单例管理
    private static Sky instance;
    private void Awake()
    {
        instance = this;
        EnableSubscribe();
    }

    public static Sky GetInstance()
    {
        return instance;
    }
    #endregion

    #region 辅助变量
    int wNum;//宽高方向上的网格数
    int hNum;
    float planeWidth;//天空面片宽高
    float planeHeight;
    float gridWidth;
    float gridHeight;

    int hintAble = -1;
    List<SkyGrid> hoveringGrids;
    List<TemplateProperty> elimTemplate;

    #endregion

    #region 资源
    public GameObject skyGridPrefab;
    public GameObject rainWater;

    ///0为不可用(红色)，1为可用(绿色)，-1为不存在
    private SkyGrid[,] grids;
    #endregion

    Vector3 debugHitPoint;

    // Use this for initialization
    void Start () {
        wNum = Global.HorizonalGridNum;
        hNum = Global.VerticalGridNum;
        planeWidth = transform.GetComponent<Collider>().bounds.size.x;
        planeHeight = transform.GetComponent<Collider>().bounds.size.z;
        gridWidth = planeWidth / wNum;
        gridHeight = planeHeight / hNum;
        grids = new SkyGrid[wNum, hNum];

        hoveringGrids = new List<SkyGrid>();

        elimTemplate = Global.elimTemplate;

        InitSky();

        EnableSubscribe();
    }
	
	// Update is called once per frame
	void Update () {
        foreach(SkyGrid i in hoveringGrids)
        {
            i.Status=hintAble;
        }
    }
    private void EnableSubscribe()
    {
        CloudOption.DragingHandle += DragingOption;//拖动选项的时候要显示效果
        CloudOption.EndDragHandle += PutUpCloud;//放上云彩
    }
   
    private void InitSky()
    {
        float angle = -140f;
        float wScale = 1f / wNum * transform.localScale.x;
        float hScale = 1f / hNum * transform.localScale.z;

        float wOffset = gridWidth;
        //float hOffset = gridHeight * Mathf.Abs(Mathf.Sin(angle));

        Vector3 leftTop = transform.position - new Vector3(planeWidth * 0.5f-gridWidth*0.5f, 
                                                             0,
                                                             0);
        for (int i = 0; i < wNum; i++)
            for (int j = 0; j < hNum; j++)
            {
                grids[i, j] = Instantiate(skyGridPrefab, leftTop + new Vector3(i * gridWidth, 0.1f, j*wOffset), transform.rotation).GetComponent<SkyGrid>();
                grids[i, j].transform.localScale = new Vector3(wScale, hScale, wScale);
                grids[i, j].transform.parent = this.transform;
                grids[i, j].position = new Vector2Int(i, j);
            }
        transform.rotation =Quaternion.Euler(angle, 0, 0);

    }

    private void DragingOption(CloudProperty c,Vector2 leftTop)
    {
        Ray ray = Camera.main.ScreenPointToRay(leftTop);

        RaycastHit hit;

        Vector2Int pos;

        hintAble = -1;
        hoveringGrids.ClearAndNotify();

        if (Physics.Raycast(ray, out hit, 100.0f, LayerMask.GetMask("Sky")))
        {
            // 打印射线检测到的物体的名称  
            //Debug.Log("射线检测到的物体名称: " + hit.transform.name);

            pos = hit.transform.GetComponent<SkyGrid>().position;//图片左上角碰撞到的格子的坐标

            if (pos.x + c.width >= wNum || pos.y - c.height + 1 < 0)
            {
                int right = pos.x + c.width >= wNum ? wNum - 1 : pos.x + c.width;
                int bottom = pos.y - c.height + 1 < 0 ? 0 : pos.y - c.height + 1;
                hintAble = 0;
                for(int i=pos.x;i<right;i++)
                {
                    for(int j=pos.y;j>=bottom;j--)
                    {
                        if (grids[i, j].Status == 0 && c.data[i - pos.x, pos.y - j] == 1)
                            hoveringGrids.Add(grids[i,j]);
                      
                    }
                }
            }
            else
            {
                hintAble = 1;
                for (int i = 0; i < c.width; i++)
                {
                    for (int j = 0; j < c.height; j++)//注意pos.y是上大下小
                    {
                        if (c.data[i, j] == 1)
                        {
                            if (grids[pos.x + i, pos.y - j].Status == 0)
                            {
                                hoveringGrids.Add(grids[pos.x + i, pos.y - j]);

                            }
                            else hintAble = 0;
                           
                        }
                    }
                }
            }
        }

    }
    /// <summary>
    /// 将云彩放到天空上
    /// </summary>
    /// <param name="c">该云彩的数据</param>
    /// <param name="position">该云彩图片左上角在屏幕上的位置</param>
    private void PutUpCloud(CloudProperty c, Vector2 leftTop)
    {
        bool destroyOption = false;
        
        RaycastHit hit;

        Vector2Int pos;
        int left = 0;//图片对应的最左和最右格子，用于传递给TemplateMatch(),减小遍历面积
        int right = 0;
        int top = 0;
        int bottom = 0;

        Ray ray = Camera.main.ScreenPointToRay(leftTop);

        if (Physics.Raycast(ray, out hit, 100.0f, LayerMask.GetMask("Sky")))
        {
            debugHitPoint = hit.point;

            // 打印射线检测到的物体的名称  
            //Debug.Log("射线检测到的物体名称: " + hit.transform.name);

            pos = hit.transform.GetComponent<SkyGrid>().position;//图片左上角碰撞到的格子的坐标
            left = pos.x;
            right = pos.x + c.width - 1 ;
            top = pos.y;
            bottom = pos.y - c.height + 1;
            //判断选项是否被完全包含在天空里
            //既然左上角碰撞到了就只需检查右界、下界
            if (pos.x + c.width >=wNum|| pos.y-c.height+1  < 0)
            {
                destroyOption = false;
            }
            else
            {
                destroyOption = true;
                for (int i = 0; i < c.width; i++)
                {
                    for (int j = 0; j < c.height; j++)//注意pos.y是上大下小
                    {
                        if (c.data[i, j] == 1)
                        {
                            if(grids[pos.x +i, pos.y -j].Active==1)
                            {
                                destroyOption = false;
                                break;
                            }
                            else
                            {
                                grids[pos.x + i, pos.y - j].Active = 1;
                            }
                        }
                    }
                    if (destroyOption == false)
                        break;
                }
            }
        }
  

        if (destroyOption)
        {
            //拖动成功，消除原来的选项
            CloudOptManager.GetInstance().RemoveOption(c);
            TemplateMatch(left,right,top,bottom);
        }
        else
        {
            //如果没拖到云彩上，要把选项放回到原来的位置
            CloudOptManager.GetInstance().PutBackOption(c);
        }
    }

    public void TemplateMatch(int l, int r, int t, int b)
    {
        //先检查大模板
        for (int e = elimTemplate.Count; e >=0; e--)
        {
            int width = elimTemplate[e].width;
            int height = elimTemplate[e].height;

            int left = l - width + 1 >= 0 ? l - width + 1 : 0;//向左width格有可能受到新放入的云彩的影响而消除
            int right = r <= wNum - width ? r : wNum - width ;
            int top = t + height - 1 < hNum ? t + height - 1 : hNum - 1 ;
            int bottom = b >= height - 1 ? b : height - 1;


            //依次以范围内每个点为左上角进行模板匹配
            for (int i = left; i <= right; i++)
            {
                for (int j = bottom; j <= top; j++)
                {
                    bool flag = false;
                    for (int m = 0; m < width; m++)
                    {
                        for (int n = 0; n < height; n++)
                        {
                            if (grids[i + m, j - n].Active == 0)
                            {
                                flag = true;
                                break;
                            }
                        }
                        if (flag)
                            break;
                    }
                    if (flag == false)
                    {
                        
                        RainFall(i,j,width,height);
                        //这里注意！sky的y坐标从上到下是由大到小，ground的y坐标从上到下是由小到大！

                        Ground.GetInstance().RainFall(i, j, width, height);
                    }
                }
            }
        }
    }
    /// <summary>
    /// 通知rainManager降雨
    /// 通知ground加湿
    /// </summary>
    /// <param name="x">区域左上角坐标</param>
    /// <param name="y">区域左上角坐标</param>
    /// <param name="width">区域宽度</param>
    /// <param name="height">区域高度</param>
    public void RainFall(int x,int y,int width,int height)
    {
        for (int m = x; m < x+width; m++)
        {
            for (int n = y; n > y-height; n--)
            {
                grids[m, n].Active = 0;
                Instantiate(rainWater, grids[m, n].transform.position,Quaternion.Euler(Vector3.forward));
                Debug.LogFormat("x:{0},y:{1},active:{2}", m, n, grids[m, n].Active);

            }
        }

    }
}


public static class List_SkyGrid_ExtensionMethods
{
    /// <summary>
    /// 扩展方法：自定义的List的Add和Remove
    /// </summary>
    /// <param name="cl">表示调用这个方法的类型是List<CloudProperty></param>
    /// <param name="cloud">真正的参数，要添加到list中的item</param>
    public static void ClearAndNotify(this List<SkyGrid> options)
    {
        foreach(SkyGrid i in options)
        {
            i.Status = 0;
        }
        options.Clear();

    }

}