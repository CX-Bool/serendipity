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
            if (steps <= 0)
            {
                
             //   view.HUDManager.GetInstance().GameOver();
            }
        }
    }
    private int score = 60;
    public int Score
    {
        get { return score; }
        set
        {
            score = value;
            view.HUDManager.GetInstance().SetScore(score);
            if(score==0)
            {
               // view.HUDManager.GetInstance().GameOver();
            }

        }
    }
    int sunshineInterval=90;
    int []ratingScore;
   // bool isNormal_1_1Available = false;
    #endregion

    List<SunshineProperty> sunshineList;

    // Use this for initialization
    void Start () {
        ratingScore = new int[5];
      
        InitScoreSlider();
        Global.InitElimTemplate();

        InitSunshineTemplate();
        InvokeRepeating("Sunshine", sunshineInterval, sunshineInterval);

    }
    void InitScoreSlider()
    {
        //test
        ratingScore[0] = 60;//起始分
        ratingScore[1] = 180;//一颗星
        ratingScore[2] = 220;//两颗星
        ratingScore[3] = 240;//三颗星
        ratingScore[4] = Global.HorizonalGridNum * Global.VerticalGridNum * Global.highestMoisture;//总分
        //end test

        view.HUDManager.GetInstance().InitScoreSlider(ratingScore,5);

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
    }
    public void AddSteps(int i)
    {
        Steps += i;
        if(i>0)
        {
            view.HUDManager.GetInstance().GenerateFloatingText("+" + i.ToString()
           , view.HUDManager.GetInstance().steps.rectTransform.position
           , view.HUDManager.GetInstance().steps.rectTransform.rotation);
        }
        else
        {
            view.HUDManager.GetInstance().GenerateFloatingText("-" + i.ToString()
           , view.HUDManager.GetInstance().steps.rectTransform.position
           , view.HUDManager.GetInstance().steps.rectTransform.rotation);
        }
       
    }
}
