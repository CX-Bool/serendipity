using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Ground : MonoBehaviour
{
    #region 单例管理
    private static Ground instance;
    private void Awake()
    {
        instance = this;
    }

    public static Ground GetInstance()
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
    List<GroundHintGrid> activeHints;

    List<TemplateProperty> elimTemplate;
    #endregion

    #region 资源
    public GameObject groundGridPrefab;
    public GameObject groundHintGridPrefab;

    private GroundGrid[,] grids;
    public GroundHintGrid[,] hints;
    List<GroundHintGrid> skyHoveringGrids;

    #endregion

    PlantOptManager plantOptManager;
    List<GameObject> plantList;
    // Use this for initialization
    void Start()
    {
        wNum = Global.HorizonalGridNum;
        hNum = Global.VerticalGridNum;
        planeWidth = transform.GetComponent<Collider>().bounds.size.x;//大面片的长宽
        planeHeight = transform.GetComponent<Collider>().bounds.size.z;
        gridWidth = planeWidth / wNum;
        gridHeight = planeHeight / hNum;
        grids = new GroundGrid[wNum, hNum];

        hints = new GroundHintGrid[wNum, hNum];
        activeHints = new List<GroundHintGrid>();
        skyHoveringGrids = new List<GroundHintGrid>();

        elimTemplate = Global.elimTemplate;
        plantOptManager = PlantOptManager.GetInstance();
        plantList = new List<GameObject>();

        InitGround();
        EnableSubscribe();

        //UpdatePlantOption();
    }
    private void Update()
    {
        foreach (GroundHintGrid i in activeHints)//种植物的提示
        {
            i.HintState = (GroundHintGrid.State)hintAble;
        }
        foreach (GroundHintGrid i in skyHoveringGrids)//云彩划过天空时的提示
        {
            i.HintState = GroundHintGrid.State.SkyShadow;
        }
    }
    private void EnableSubscribe()
    {
        PlantOption.DragingHandle += DragingOption;
        PlantOption.EndDragHandle += EndDrag;
    }
    // sky的y坐标从上到下是由大到小，ground的y坐标从上到下是由小到大！
    public void InitGround()
    {
        float wScale = 1f / wNum * transform.localScale.x;
        float hScale = 1f / hNum * transform.localScale.z;

        //float wOffset = Mathf.Abs(Mathf.Cos(transform.rotation.eulerAngles.x));
        //float hOffset = gridHeight * Mathf.Abs(Mathf.Sin(transform.rotation.eulerAngles.x));

        Vector3 leftBottom = transform.position - new Vector3(planeWidth * 0.5f - gridWidth * 0.5f,
                                                               0f,
                                                               0f);
        for (int i = 0; i < wNum; i++)
            for (int j = 0; j < hNum; j++)
            {
                grids[i, j] = Instantiate(groundGridPrefab, leftBottom + new Vector3(i * gridWidth, 0.1f, -j * gridWidth), transform.rotation).GetComponent<GroundGrid>();
                grids[i, j].transform.localScale = new Vector3(wScale, hScale, wScale);
                grids[i, j].transform.parent = this.transform;
                grids[i, j].position = new Vector2Int(i, j);

                hints[i, j] = Instantiate(groundHintGridPrefab, leftBottom + new Vector3(i * gridWidth, 0.1f, -j * gridWidth), transform.rotation).GetComponent<GroundHintGrid>();
                hints[i, j].transform.localScale = new Vector3(wScale, hScale, wScale);
                hints[i, j].transform.parent = this.transform;
                hints[i, j].position = new Vector2Int(i, j);
            }
        transform.rotation = Quaternion.Euler(-50, 0, 0);
    }
    /// <summary>
    /// 对一种模板进行匹配
    /// </summary>
    /// <param name="template">植物库</param>
    /// <param name="templateList">返回的匹配上的块的数据</param>
    /// <param name="hor"></param>
    /// <param name="ver"></param>
    public void TemplateMatch(List<PlantProperty> template, Dictionary<PlantProperty, List<Vector2Int>> optionList)
    {
        int width = template[0].width;
        int height = template[0].height;

        for (int i = 0; i < template.Count; i++)
        {
            int moisture = (template[i]).moisture;
            for (int j = 0; j <= wNum - width; j++)
            {
                //bool flag = false;
                //for(int k=0;k<width;k++)//连续width列 //每列必须有多于height个moisture湿度的格子
                //{
                //    if (hor[j + k, moisture] < height)
                //    {
                //        flag = true;
                //        break;
                //    }
                //}
                //if(flag==false)
                //{
                for (int n = 0; n <= hNum - height; n++)///列的范围已经确定为[j,j+width]，在这个范围内依次以每行作为开头来尝试
                {
                    bool flag2 = false;
                    for (int a = 0; a < width; a++)
                    {
                        for (int b = 0; b < height; b++)
                        {
                            if (grids[j + a, n + b].Moisture < moisture || grids[j + a, n + b].State == 1)
                            {
                                flag2 = true;
                                break;
                            }
                        }
                        if (flag2)
                            break;
                    }
                    if (flag2 == false)
                    {
                        Vector2Int position = new Vector2Int(j, n);
                        //如果字典里有选项没位置，增加位置
                        if (optionList.ContainsKey(template[i]))
                        {
                            if (!optionList[template[i]].Contains(position))
                            {
                                optionList[template[i]].Add(position);
                            }
                        }
                        else//如果字典里没这个选项就新增选项
                        {
                            optionList.Add(template[i], new List<Vector2Int>());
                            optionList[template[i]].Add(position);

                        }

                    }

                }
                //}
            }
        }

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="i">选项左上角的对应格子数组的横坐标</param>
    /// <param name="j">选项左上角的对应格子数组的纵坐标</param>
    /// <param name="c">选项数据</param>
    /// <returns></returns>
    public void UpdatePlantOption()
    {
        //int left = i - 1 < 0 ? 0 : i - 1;
        //int right = (i + c.width+1) > wNum ? wNum : (i + c.width);
        //int bottom = j - 1 < 0 ? 0 : j - 1;
        //int top = (j + c.height+1) > hNum ? hNum : (j + c.height);
        //int[,] connectRegion=new int[wNum,hNum];
        //int regionIndex = 0;

        //int[,] horProjection = new int[wNum, Global.dMoisture];

        ////遍历一遍找出每行，每种湿度的格子有几个
        //for (int m = 0; m < hNum; m++)
        //{
        //    for (int n = 0; n < wNum; n++)
        //    {
        //        horProjection[n, grids[n,m].Moisture]++;
        //    }
        //}
        plantOptManager.optionList.Clear();

        TemplateMatch(plantOptManager.bigPlants, plantOptManager.optionList);
        TemplateMatch(plantOptManager.middlePlants, plantOptManager.optionList);
        TemplateMatch(plantOptManager.smallPlants, plantOptManager.optionList);

        PlantOptManager.optionChangeHandle();//通知HUD更新选项
                                             //连通区域标记算法
                                             //先逐行扫描，标记行中所有团
                                             //for (int n = bottom, a = 0; n < top; n++)
                                             //{
                                             //    connectRegion[a, 0] = regionIndex;

        //    for (int m=left+1,b=1;m<right;m++)
        //    {

        //       if(m>0&&grids[n,m].Moisture==grids[n,m-1].Moisture)
        //        {
        //            connectRegion[a,b] = connectRegion[a,b-1];
        //        }
        //        else
        //        {
        //            regionIndex++;
        //            connectRegion[a, b] = regionIndex;
        //        }
        //    }
        //}
        ////再将每行的团进行合并
        //for(int n=bottom+1,a=1;n<top;n++)
        //{
        //    for(int m=left,b=0;m<right;m++)
        //    {
        //        if (grids[n,m].Moisture == grids[n-1,m].Moisture)
        //        {
        //            int moisture = grids[n, m].Moisture;
        //            int index = connectRegion[a, b];
        //            while(m<right&&grids[n,m].Moisture==moisture)
        //            {
        //                connectRegion[a, b] = index;
        //                m++;b++;
        //            }

        //        }
        //    }
        //}


    }
    private void GrowPlant(Vector2Int leftTop, PlantProperty plant)
    {
        plant.position = leftTop;
        GameObject newPlant = new GameObject(plant.name.ToString());
        switch (plant.type)
        {
            case Global.PlantType.CHI_one_normal:
                newPlant.AddComponent<YuMeiRen>();
                newPlant.GetComponent<YuMeiRen>().property = plant;
                break;
            case Global.PlantType.Keep_Moisture1:
                newPlant.AddComponent<DanMu>();
                newPlant.GetComponent<DanMu>().property = plant;
                break;
            case Global.PlantType.Keep_Moisture2:
                newPlant.AddComponent<ShaTang>();
                newPlant.GetComponent<ShaTang>().property = plant;
                break;
            case Global.PlantType.CHI_two_normal:
                newPlant.AddComponent<ZhuYu>();
                newPlant.GetComponent<ZhuYu>().property = plant;
                break;
            case Global.PlantType.CHI_two_special:
                newPlant.AddComponent<XuanCao>();
                newPlant.GetComponent<XuanCao>().property = plant;
                break;
            case Global.PlantType.Increase_Moisture:
                newPlant.AddComponent<DaChun>();
                newPlant.GetComponent<DaChun>().property = plant;
                break;
            case Global.PlantType.CHI_one_super:
                newPlant.AddComponent<WuTong>();
                newPlant.GetComponent<WuTong>().property = plant;
                break;

        }
        plantList.Add(newPlant);
        
        for (int i = leftTop.x; i < leftTop.x + plant.width; i++)
        {
            for (int j = leftTop.y; j < leftTop.y + plant.height; j++)
            {
                grids[i, j].material.mainTexture = plant.texture;
                grids[i, j].State = 1;
            }
        }


    }
    
    public void AddIncreaseLock(int l,int r,int u,int d)
    {
        for(int i=l;i<r;i++)
        {
            for(int j=u;j<d;j++)
            {
                if (grids[i, j].State == 0)
                {
                    grids[i, j].AddIncreaseLock();
                }
            }
        }
    }
    public void AddDecreaseLock(int l, int r, int u, int d)
    {
        for (int i = l; i < r; i++)
        {
            for (int j = u; j < d; j++)
            {
                if (grids[i, j].State == 0)
                {
                    grids[i, j].AddDecreaseLock();
                }
            }
        }
    }
    public void DragingOption(PlantProperty p, Vector2 leftTop)
    {
        Ray ray = Camera.main.ScreenPointToRay(leftTop);

        RaycastHit hit;

        Vector2Int pos;

        hintAble = -1;
        activeHints.ClearHintState();
        if(Physics.Raycast(ray, out hit, 100.0f, LayerMask.GetMask("UI")))
        {
           //
        }
        if (Physics.Raycast(ray, out hit, 100.0f, LayerMask.GetMask("GroundHint")))
        {
            // 打印射线检测到的物体的名称  
            //Debug.Log("射线检测到的物体名称: " + hit.transform.name);

            pos = hit.transform.GetComponent<GroundHintGrid>().position;//图片左上角碰撞到的格子的坐标

            //如果块没有全部包含在棋盘里，一定提示异常
            if (pos.x + p.width > wNum || pos.y + p.height > hNum)
            {
                int right = pos.x + p.width > wNum ? wNum : pos.x + p.width;
                int bottom = pos.y + p.height > hNum ? hNum : pos.y + p.height;
                hintAble = 0;
                for (int i = pos.x; i < right; i++)
                {
                    for (int j = pos.y; j < bottom; j++)
                    {
                        if (grids[i, j].State == 0)
                            activeHints.Add(hints[i, j]);

                    }
                }
            }
            else
            {
                hintAble = 1;
                for (int i = 0; i < p.width; i++)
                {
                    for (int j = 0; j < p.height; j++)//注意pos.y是上大下小
                    {
                        if (grids[pos.x + i, pos.y + j].Moisture < p.moisture)
                            hintAble = 0;
                        //如果范围内没有别的云彩块，就是绿色提示，否则也要提示异常
                        if (grids[pos.x + i, pos.y + j].State == 0)
                        {
                            activeHints.Add(hints[pos.x + i, pos.y + j]);

                        }
                        else hintAble = 0;

                    }
                }
            }
        }

    }
    public void EndDrag(PlantProperty p, Vector2 leftTop)
    {
        activeHints.ClearHintState();

        bool destroyOption = false;

        RaycastHit hit;

        Vector2Int pos = new Vector2Int();
        int left = 0;//图片对应的最左和最右格子，用于传递给TemplateMatch(),减小遍历面积
        int right = 0;
        int top = 0;
        int bottom = 0;

        Ray ray = Camera.main.ScreenPointToRay(leftTop);
        if (Physics.Raycast(ray, out hit, 100.0f, LayerMask.GetMask("Ground")))
        {
            activeHints.ClearHintState();
            //debugHitPoint = hit.point;

            // 打印射线检测到的物体的名称  
            //Debug.Log("射线检测到的物体名称: " + hit.transform.name);

            pos = hit.transform.GetComponent<GroundGrid>().position;//图片左上角碰撞到的格子的坐标
            left = pos.x;
            right = pos.x + p.width - 1;
            top = pos.y;
            bottom = pos.y + p.height - 1;
            //判断选项是否被完全包含在棋盘里
            //既然左上角碰撞到了就只需检查右界、下界
            //ground的格子是从上到下下标从小到大的
            if (pos.x + p.width > wNum || pos.y + p.height > hNum)
            {
                destroyOption = false;
            }
            else
            {
                destroyOption = true;
                for (int i = 0; i < p.width; i++)
                {
                    for (int j = 0; j < p.height; j++)//注意pos.y是上大下小
                    {
                        //如果这片地上有东西就不能放置
                        if (grids[pos.x + i, pos.y + j].State == 1 || grids[pos.x + i, pos.y + j].Moisture < p.moisture)
                        {
                            destroyOption = false;
                            break;
                        }

                    }
                    if (destroyOption == false)
                        break;
                }
            }
        }


        if (destroyOption)
        {
            GrowPlant(pos, p);
            UpdatePlantOption();
        }
        else
        {
            //如果没拖到云彩上，要把选项放回到原来的位置
            PlantOptManager.GetInstance().PutBackOption(p);
        }
    }

    public void Sunshine(SunshineProperty property)
    {
        for (int i = property.position.x, m = 0; i < property.position.x + property.width; i++, m++)
        {
            for (int j = property.position.y, n = 0; j < property.position.y + property.height; j++, n++)
            {
                if(grids[i,j].increaseLock==false)
                {
                    grids[i, j].Moisture -= property.data[m, n];

                }
            }
        }
    }
    public void ChangeMoisture(int x, int y, int width, int height,int val)
    {
        for (int m = x; m < x + width; m++)
        {
            for (int n = y; n > y - height; n--)
            {
                grids[m, n].Moisture += val;

            }
        }
    }
    public void ChangeMoisture(int x,int y,int value)
    {
        grids[x, y].Moisture += value;
    }
    public void AddHintGrid(int i, int j)
    {
        //if(hints[i,j].HintState==GroundHintGrid.State.Empty)
        skyHoveringGrids.Add(hints[i, j]);
    }
    public void ClearHintState()
    {
        skyHoveringGrids.ClearHintState();
    }
}

public static class List_GroundHintGrid_ExtensionMethods
{

    /// <summary>
    /// 扩展方法：自定义的List的Add和Remove
    /// </summary>
    /// <param name="cl">表示调用这个方法的类型是List<CloudProperty></param>
    /// <param name="cloud">真正的参数，要添加到list中的item</param>
    public static void ClearHintState(this List<GroundHintGrid> options)
    {
        foreach (GroundHintGrid i in options)
        {
            i.HintState = GroundHintGrid.State.Empty;
        }
        options.Clear();

    }

}