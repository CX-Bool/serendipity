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
    List<TemplateProperty> elimTemplate;
    #endregion

    #region 资源
    public GameObject groundGridPrefab;
    private GroundGrid[,] grids;
    #endregion

    PlantOptManager plantOptManager;
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

        elimTemplate = Global.elimTemplate;
        plantOptManager = PlantOptManager.GetInstance();
        InitGround();
        EnableSubscribe();
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
    public void TemplateMatch(List<PlantProperty> template, Dictionary<PlantProperty, List<Vector2Int>> optionList,int[,] hor)
    {
        int width = template[0].width;
        int height = template[0].height;

        for(int i=0;i<template.Count;i++)
        {
            int moisture = (template[i]).moisture;
            for(int j=0;j<=wNum-width;j++)
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
                    for(int n=0;n<hNum-height;n++)//列的范围已经确定为[j,j+width]，在这个范围内依次以每行作为开头来尝试
                    {
                        bool flag2 = false;
                        for(int a=0;a<width;a++)
                        {
                            for(int b=0;b<height;b++)
                            {
                                if(grids[j+a,n+b].Moisture<moisture)
                                {
                                    flag2 = true;
                                    break;
                                }
                            }
                            if (flag2)
                                break;
                        }
                        if(flag2==false)
                        {
                            Vector2Int position=new Vector2Int(j,n);
                            //如果字典里有选项没位置，增加位置
                            if(optionList.ContainsKey(template[i]))
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
    public void DragingOption(PlantProperty plantProperty,Vector2 leftTop)
    {

    }
    public void EndDrag(PlantProperty plantProperty, Vector2 leftTop)
    {

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

        int[,] horProjection = new int[wNum, Global.dMoisture];

        ////遍历一遍找出每行，每种湿度的格子有几个
        //for (int m = 0; m < hNum; m++)
        //{
        //    for (int n = 0; n < wNum; n++)
        //    {
        //        horProjection[n, grids[n,m].Moisture]++;
        //    }
        //}
        TemplateMatch(plantOptManager.bigPlants,plantOptManager.optionList,horProjection);
        TemplateMatch(plantOptManager.middlePlants,plantOptManager.optionList, horProjection);
        TemplateMatch(plantOptManager.smallPlants,plantOptManager.optionList, horProjection);
        
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
    public void RainFall(int x, int y, int width, int height)
    {
        for (int m = x; m < x + width; m++)
        {
            for (int n = y; n > y - height; n--)
            {
                grids[m, n].Moisture += 1 ;
                //Debug.LogFormat("ground：x:{0},y:{1}", m, n);

            }
        }
    }
}