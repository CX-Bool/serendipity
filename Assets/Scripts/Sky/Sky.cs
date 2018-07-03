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
    private GameObject[,] gridObj;
    #endregion

    Vector3 debugWorldPos;
    Vector3 debugDirPos;

    List<TemplateProperty> elimTemplate;
    // Use this for initialization
    void Start () {
        wNum = Global.HorizonalGridNum;
        hNum = Global.VerticalGridNum;
        planeWidth = transform.GetComponent<Collider>().bounds.size.x;
        planeHeight = transform.GetComponent<Collider>().bounds.size.z;
        gridWidth = planeWidth / wNum;
        gridHeight = planeHeight / hNum;
        gridObj = new GameObject[wNum, hNum];

        rayCastDir = new Vector3(0, 0, 1);

        InitSky();
        InitElimTemplate();

        EnableSubscribe();
    }
	
	// Update is called once per frame
	void Update () {
                    Debug.DrawLine(debugWorldPos, debugDirPos,Color.red);

    }
    private void EnableSubscribe()
    {
        CloudOption.DragingHandle += DragingOption;//拖动选项的时候要显示效果
        CloudOption.EndDragHandle += PutUpCloud;//放上云彩
    }
    private void InitElimTemplate()
    {
        elimTemplate = new List<TemplateProperty>();
        //read from xml
        string filepath = System.Environment.CurrentDirectory + "\\Assets\\Resources\\EliminationTemplate.xml";

        if (File.Exists(filepath))
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filepath);
            if (xmlDoc != null)
            {
                XmlNodeList templateList = xmlDoc.SelectSingleNode("main").ChildNodes;
                foreach (XmlNode xn in templateList)
                {
                    int width = int.Parse(xn.SelectSingleNode("width").InnerText);
                    int height = int.Parse(xn.SelectSingleNode("height").InnerText);
                    Global.TemplateType type = (Global.TemplateType)System.Enum.Parse(typeof(Global.TemplateType), xn.SelectSingleNode("type").InnerText);

                    CloudElimProperty tmp = new CloudElimProperty();
                    tmp.width = width;
                    tmp.height = height;
                    tmp.type = type;
                    
                    elimTemplate.Add(tmp);
                }

            }

        }
    }

    private void InitSky()
    {
        float wScale = 1f / wNum * transform.localScale.x;
        float hScale = 1f / hNum * transform.localScale.z;

        float wOffset = Mathf.Abs(Mathf.Cos(transform.rotation.eulerAngles.x));
        float hOffset = gridHeight * Mathf.Abs(Mathf.Sin(transform.rotation.eulerAngles.x));

        Vector3 leftTop = transform.position - new Vector3(planeWidth * 0.5f-gridWidth*0.5f, 
                                                             0,
                                                             0);
        for (int i = 0; i < wNum; i++)
            for (int j = 0; j < hNum; j++)
            {
                gridObj[i, j] = Instantiate(skyGridPrefab, leftTop + new Vector3(i * gridWidth, 0.1f, j*wOffset), transform.rotation);
                gridObj[i, j].transform.localScale = new Vector3(wScale, hScale, wScale);
                gridObj[i, j].transform.parent = this.transform;
            }
        transform.rotation =Quaternion.Euler(-140, 0, 0);
    }

    private void DragingOption(CloudProperty cloudProperty,Vector2 position)
    {

    }

    private void PutUpCloud(CloudProperty c, Vector2 position)
    {
        bool destroyOption = false;
        for(int i=0;i<c.width;i++)
        {
            for(int j=0;j<c.height;j++)
            {
                if (c.data[i, j] == 1)
                {
                    RaycastHit hit;

                    Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f));
                    worldPos += new Vector3(i * gridWidth, j * gridHeight, 0);
                    debugWorldPos = worldPos;
                    debugDirPos = worldPos - Camera.main.transform.position;
                    if (Physics.Raycast(worldPos, worldPos-Camera.main.transform.position, out hit, 50,LayerMask.GetMask("Sky")))
                    {
                        hit.transform.gameObject.SetActive(false);
                        destroyOption = true;
                    }             
                }
            }
        }   

        if (destroyOption)
            CloudOptManager.GetInstance().RemoveOption(c);
        else
        {
            //如果没拖到云彩上，要把选项放回到原来的位置
            CloudOptManager.GetInstance().PutBackOption(c);
        }
    }
}
