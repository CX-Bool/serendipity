using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;
public static class Global{

   
    #region 定义棋盘格子数
    public static int HorizonalGridNum = 12;
    public static int VerticalGridNum = 6;
    #endregion

    #region 定义湿度
    public static int lowestMoisture = -1;
    public static int highestMoisture = 5;
    public static int dMoisture = 7;
    #endregion

    public enum CloudType
    {
        NORMAL,//普通降雨的云彩块
        NORMAL_1_1,//1*1的云彩
        SPECIAL_3_3
    }

    public enum PlantType
    {
        CHI_one_normal,//虞美人
        Keep_Moisture1,//丹木
        Keep_Moisture2,//沙棠
        CHI_two_normal,//祝余
        CHI_two_special,//忘忧草（萱草）
        Increase_Moisture,//大椿
        CHI_one_super//梧桐
    }
    
    public enum TemplateType
    {
        SMALL_2_2,
        MIDDLE_3_2,
        BIG_3_3,
        BIG_4_3,
        UNMATCH
    }

   

    public static void SwapVector3(Vector3 a,Vector3 b)
    {
        Vector3 tmp = a;
        a = b;
        b = tmp;
    }

    //可消除的模板
    public static List<TemplateProperty> elimTemplate;

    public static void InitElimTemplate()
    {
        elimTemplate = new List<TemplateProperty>();
        //read from xml
       
        XmlDocument xmlDoc = new XmlDocument();

        xmlDoc.LoadXml(Resources.Load("Config/EliminationTemplate").ToString());
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
                tmp.templateType = type;

                elimTemplate.Add(tmp);
            }

        }

    }


}
