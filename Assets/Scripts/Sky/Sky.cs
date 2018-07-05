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
    Vector3 rayCastDir;
    #endregion

    #region 资源
    public GameObject skyGridPrefab;
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

        rayCastDir = new Vector3(0, 0, 1);

        InitSky();

        EnableSubscribe();
    }
	
	// Update is called once per frame
	void Update () {
          Debug.DrawLine(Camera.main.transform.position, debugHitPoint,Color.red);
    

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

    private void DragingOption(CloudProperty cloudProperty,Vector2 position)
    {

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

        Ray ray = Camera.main.ScreenPointToRay(leftTop);

        if (Physics.Raycast(ray, out hit, 100.0f, LayerMask.GetMask("Sky")))
        {
            debugHitPoint = hit.point;

            // 打印射线检测到的物体的名称  
            //Debug.Log("射线检测到的物体名称: " + hit.transform.name);

            Vector2Int pos=hit.transform.GetComponent<SkyGrid>().position;

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
            CheckRainFall();
        }
        else
        {
            //如果没拖到云彩上，要把选项放回到原来的位置
            CloudOptManager.GetInstance().PutBackOption(c);
        }
    }

    public void CheckRainFall()
    {

    }
}
