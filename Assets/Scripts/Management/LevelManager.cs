using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;


public class LevelManager : MonoBehaviour {

    #region 单例管理
    private static LevelManager instance;

    private void Awake()
    {
        instance = this;
    }

    public static LevelManager GetInstance()
    {
        return instance;
    }
    #endregion

    #region 关卡数据
    private int steps = 300;
    public int Steps
    {
        get { return steps; }
        set
        {
            
            steps = value;
            view.HUDManager.GetInstance().SetSteps();
            if (steps == 0)
            {
                Time.timeScale = 0;
            }
        }
    }

    int interval=70;
    List<SunshineProperty> sunshineList;
    #endregion

    // Use this for initialization
    void Start () {

        Global.InitElimTemplate();

        InitSunshineTemplate();
        InvokeRepeating("Sunshine", interval, interval);

    }
    void InitSunshineTemplate()
    {
        sunshineList = new List<SunshineProperty>();

        XmlDocument xmlDoc = new XmlDocument();

        xmlDoc.LoadXml(Resources.Load("Config/Sunshine").ToString());
        if (xmlDoc != null)
        {
            XmlNodeList cloudList = xmlDoc.SelectSingleNode("main").ChildNodes;
            foreach (XmlNode xn in cloudList)
            {
                int width = int.Parse(xn.SelectSingleNode("width").InnerText);
                int height = int.Parse(xn.SelectSingleNode("height").InnerText);
                string data = xn.SelectSingleNode("data").InnerText;
                //  Global.CloudType type = (Global.CloudType)System.Enum.Parse(typeof(Global.CloudType), xn.SelectSingleNode("type").InnerText);

                SunshineProperty property = new SunshineProperty();
                property.width = width;
                property.height = height;
                property.data = new int[width, height];

                for (int i = 0; i < width; i++)
                    for (int j = 0; j < height; j++)
                    {
                        property.data[i, j] = data[i * height + j] - '0';
                    }
                sunshineList.Add(property);
            }

        }

    }
    void Sunshine()
    {
        SunshineProperty property = sunshineList[Random.Range(0, sunshineList.Count)];
        property.position = new Vector2Int(Random.Range(0, Global.HorizonalGridNum - property.width), Random.Range(0, Global.VerticalGridNum - property.height));
        Ground.GetInstance().Sunshine(property);
        view.HUDManager.GetInstance().Sunshine(property.position.x);
    }
    public void AddSteps(int i)
    {
        Steps += i;
        view.HUDManager.GetInstance().GenerateFloatingText("+" + i.ToString()
            , view.HUDManager.GetInstance().steps.rectTransform.position
            , view.HUDManager.GetInstance().steps.rectTransform.rotation);
    }
}
