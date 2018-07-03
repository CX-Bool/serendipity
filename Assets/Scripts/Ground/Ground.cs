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
    #endregion

    #region 资源
    public GameObject groundGridPrefab;
    private GameObject[,] gridObj;
    #endregion

  
    // Use this for initialization
    void Start()
    {
        wNum = Global.HorizonalGridNum;
        hNum = Global.VerticalGridNum;
        planeWidth = transform.GetComponent<Collider>().bounds.size.x;//大面片的长宽
        planeHeight = transform.GetComponent<Collider>().bounds.size.z;
        Debug.Log(transform.GetComponent<Collider>().bounds.size);
        gridWidth = planeWidth / wNum;
        gridHeight = planeHeight / hNum;
        gridObj = new GameObject[wNum, hNum];

        InitGround();
    }

    // Update is called once per frame
    void Update()
    {

    }
    //初始化生成天空格子
    public void InitGround()
    {
        float wScale = 1f / wNum * transform.localScale.x;
        float hScale = 1f / hNum * transform.localScale.z;
       
        float wOffset = Mathf.Abs(Mathf.Cos(transform.rotation.eulerAngles.x));
        float hOffset = gridHeight * Mathf.Abs(Mathf.Sin(transform.rotation.eulerAngles.x));

        Vector3 leftBottom = transform.position - new Vector3(planeWidth * 0.5f - gridWidth * 0.5f,
                                                               0f,
                                                               0f);
        for (int i = 0; i < wNum; i++)
            for (int j = 0; j < hNum; j++)
            {
                gridObj[i, j] = Instantiate(groundGridPrefab, leftBottom + new Vector3(i * gridWidth, 0.1f, -j * wOffset), transform.rotation);
                gridObj[i, j].transform.localScale = new Vector3(wScale, hScale, wScale);
                gridObj[i, j].transform.parent = this.transform;
            }
       transform.rotation = Quaternion.Euler(-40, 0, 0);
    }

}