using System.Collections.Generic;
using System.Collections;
using UnityEngine;
public class Sky : MonoBehaviour {

    #region 单例管理
    private static Sky instance;
    private void Awake()
    {
        instance = this;
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
    ///0为不可用(红色)，1为可用(绿色)，-1为不存在
    int hintAble = -1;
    List<SkyHintGrid> hoveringGrids;
    List<TemplateProperty> elimTemplate;

    #endregion

    #region 资源
    public GameObject skyGridPrefab;
    public GameObject skyHintGridPrefab;
    public GameObject rainWater;


    private SkyGrid[,] grids;
    private SkyHintGrid[,] hints;
    #endregion

    #region 委托
    public delegate void Skill(int i,int j);
    /// <summary>
    /// 1、通知HUDManager销毁当前UI并更新Options
    /// 2、通知Sky有一块云彩被放置了
    /// </summary>
    public static Skill UseSkill;
    #endregion
    Vector3 debugHitPoint;
    Vector3 debugDir;
    // Use this for initialization
    void Start () {
        wNum = Global.HorizonalGridNum;
        hNum = Global.VerticalGridNum;
        planeWidth = transform.GetComponent<Collider>().bounds.size.x;
        planeHeight = transform.GetComponent<Collider>().bounds.size.z;
        gridWidth = planeWidth / wNum;
        gridHeight = planeHeight / hNum;
        grids = new SkyGrid[wNum, hNum];
        hints = new SkyHintGrid[wNum, hNum];

        hoveringGrids = new List<SkyHintGrid>();

        elimTemplate = Global.elimTemplate;

        InitSky();

        EnableSubscribe();
    }
	
	// Update is called once per frame
	void Update () {
        foreach(SkyHintGrid i in hoveringGrids)
        {
            i.HintState=hintAble;
        }
        Debug.DrawLine(Camera.main.transform.position, debugHitPoint,Color.red); Ray ray;
        Debug.DrawRay(Camera.main.transform.position,debugDir,Color.green);
    }
    private void EnableSubscribe()
    {
        CloudOption.DragingHandle += DragingOption;//拖动选项的时候要显示效果
        CloudOption.EndDragHandle += EndDrag;//放上云彩
    }
   
    private void InitSky()
    {
        float angle = -130f;
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
                grids[i, j] = Instantiate(skyGridPrefab, leftTop + new Vector3(i * gridWidth, 0.05f, j*wOffset), transform.rotation).GetComponent<SkyGrid>();
                grids[i, j].transform.localScale = new Vector3(wScale, hScale, wScale);
                grids[i, j].transform.parent = this.transform;
                grids[i, j].position = new Vector2Int(i, j);

                hints[i, j] = Instantiate(skyHintGridPrefab, leftTop + new Vector3(i * gridWidth, 0.1f, j * wOffset), transform.rotation).GetComponent<SkyHintGrid>();
                hints[i, j].transform.localScale = new Vector3(wScale, hScale, wScale);
                hints[i, j].transform.parent = this.transform;
                hints[i, j].position = new Vector2Int(i, j);

            }
        transform.rotation =Quaternion.Euler(angle, 0, 0);

    }

    private void DragingOption(CloudProperty c,Vector2 leftTop)
    {
        Ray ray = Camera.main.ScreenPointToRay(leftTop);

        debugDir = ray.direction;
        RaycastHit hit;

        Vector2Int pos;

        hintAble = -1;
        hoveringGrids.ClearHintState();
        Ground.GetInstance().ClearHintState();

        if (Physics.Raycast(ray, out hit, 100.0f, LayerMask.GetMask("SkyHint")))
        {
            // 打印射线检测到的物体的名称  
            //Debug.Log("射线检测到的物体名称: " + hit.transform.name);
            debugHitPoint = hit.point;
            
            pos = hit.transform.GetComponent<SkyHintGrid>().position;//图片左上角碰撞到的格子的坐标

            //如果块没有全部包含在棋盘里，一定提示异常
            if (pos.x + c.width > wNum || pos.y - c.height + 1 < 0)
            {
                int right = pos.x + c.width > wNum ? wNum : pos.x + c.width;
                int bottom = pos.y - c.height + 1 < 0 ? 0 : pos.y - c.height + 1;
                hintAble = 0;
                for(int i=pos.x;i<right;i++)
                {
                    for(int j=pos.y;j>=bottom;j--)
                    {
                        if (grids[i, j].State == 0 && c.data[i - pos.x, pos.y - j] == 1)
                        {
                            hoveringGrids.Add(hints[i, j]);
                            Ground.GetInstance().AddHintGrid(i, j);
                        }

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
                            //如果范围内没有别的云彩块，就是绿色提示，否则也要提示异常
                            if (grids[pos.x + i, pos.y - j].State == 0)
                            {
                                hoveringGrids.Add(hints[pos.x + i, pos.y - j]);
                                Ground.GetInstance().AddHintGrid(pos.x + i, pos.y - j);

                            }
                            else hintAble = 0;                      
                        }
                    }
                }
            }
        }

    }

    private void EndDrag(CloudProperty c, Vector2 leftTop)
    {
        //消除提示
        hoveringGrids.ClearHintState();
        Ground.GetInstance().ClearHintState();

        bool destroyOption = false;
        RaycastHit hit;

        Vector2Int pos = new Vector2Int();
        int left = 0;//图片对应的最左和最右格子，用于传递给TemplateMatch(),减小遍历面积
        int right = 0;
        int top = 0;
        int bottom = 0;

        Ray ray = Camera.main.ScreenPointToRay(leftTop);

        if (Physics.Raycast(ray, out hit, 100.0f, LayerMask.GetMask("Sky")))
        {
            

            // 打印射线检测到的物体的名称  
            //Debug.Log("射线检测到的物体名称: " + hit.transform.name);

            pos = hit.transform.GetComponent<SkyGrid>().position;//图片左上角碰撞到的格子的坐标
            left = pos.x;
            right = pos.x + c.width - 1 ;
            top = pos.y;
            bottom = pos.y - c.height + 1;
            //判断选项是否被完全包含在天空里
            //既然左上角碰撞到了就只需检查右界、下界
            if (pos.x + c.width > wNum|| pos.y-c.height + 1 < 0)
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
                            if(grids[pos.x +i, pos.y -j].State==1)
                            {
                                destroyOption = false;
                                break;
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
            c.EnableSubscribe();
            //拖动成功，消除原来的选项
            CloudOptManager.GetInstance().RemoveOption(c);

            if(UseSkill==null)
            {
                //Normal skill
                for (int i = 0; i < c.width; i++)
                {
                    for (int j = 0; j < c.height; j++)//注意pos.y是上大下小
                    {
                        if (c.data[i, j] == 1)
                            grids[pos.x + i, pos.y - j].State = 1;
                    }
                }
                TemplateMatch(left, right, top, bottom);
            }
            else
            {
                //Special skill
                UseSkill(pos.x, pos.y);
            }
            c.DisableSubscribe();
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
        for (int e = elimTemplate.Count-1; e >=0; e--)
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
                            if (grids[i + m, j - n].State == 0)
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

                        Ground.GetInstance().UpdatePlantOption();
                    }
                }
            }
        }
    }
    /// <summary>
    /// 降雨 通知ground加湿
    /// </summary>
    /// <param name="x">区域左上角坐标</param>
    /// <param name="y">区域左上角坐标</param>
    /// <param name="width">区域宽度</param>
    /// <param name="height">区域高度</param>
    public void RainFall(int x,int y,int width,int height)
    {
        AudioManager.GetInstance().PlayAudio("rain");
        for (int m = x; m < x+width; m++)
        {
            for (int n = y; n > y-height; n--)
            {
                grids[m, n].State = 0;
                StartCoroutine("Rain", grids[m, n].transform.position);

            }
        }
        Ground.GetInstance().ChangeMoisture(x, y, width, height, 1);

    }
    public void RainFall(int x,int y,int val)
    {
        if (grids[x, y].State == 0)
            return;
        grids[x, y].State = 0;
        StartCoroutine("Rain", grids[x, y].transform.position);
        Ground.GetInstance().ChangeMoisture(x, y, val);

    }
    IEnumerator Rain(Vector3 pos)
    {
        Instantiate(rainWater, pos, Quaternion.Euler(Vector3.forward));
        yield return new WaitForSeconds(0.5f);
        Instantiate(rainWater, pos, Quaternion.Euler(Vector3.forward));
    }
}


public static class List_SkyHintGrid_ExtensionMethods
{
    /// <summary>
    /// 扩展方法：自定义的List的Add和Remove
    /// </summary>
    /// <param name="cl">表示调用这个方法的类型是List<CloudProperty></param>
    /// <param name="cloud">真正的参数，要添加到list中的item</param>
    public static void ClearHintState(this List<SkyHintGrid> options)
    {
        foreach(SkyHintGrid i in options)
        {
            i.HintState = -1;
        }
        options.Clear();

    }

}