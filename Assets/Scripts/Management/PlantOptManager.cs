using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;
/// <summary>
/// 管理植物选项菜单
/// </summary>
public class PlantOptManager : MonoBehaviour {
    #region 单例管理
    private static PlantOptManager instance;

    private void Awake()
    {
        instance = this;
    }

    public static PlantOptManager GetInstance()
    {
        return instance;
    }

    private List<TemplateProperty> smallPlants;
    private List<TemplateProperty> middlePlants;
    private List<TemplateProperty> bigPlants;

    #endregion
    // Use this for initialization
    void Start () {
        InitPlantList();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void InitPlantList()
    {
        smallPlants = new List<TemplateProperty>();
        middlePlants = new List<TemplateProperty>();
        bigPlants = new List<TemplateProperty>();
        string filepath = System.Environment.CurrentDirectory + "\\Assets\\Resources\\Plant.xml";

        if (File.Exists(filepath))
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filepath);
            if (xmlDoc != null)
            {
                XmlNodeList cloudList = xmlDoc.SelectSingleNode("main").ChildNodes;
                foreach (XmlNode xn in cloudList)
                {
                    int width = int.Parse(xn.SelectSingleNode("Width").InnerText);
                    int height = int.Parse(xn.SelectSingleNode("Height").InnerText);
                    Global.TemplateType templetType = (Global.TemplateType)System.Enum.Parse(typeof(Global.TemplateType), xn.SelectSingleNode("TemplateType").InnerText);
                    string tex = xn.SelectSingleNode("TextureName").InnerText;
                    Global.PlantType type = (Global.PlantType)System.Enum.Parse(typeof(Global.PlantType), xn.SelectSingleNode("Type").InnerText);

                    PlantProperty property = new PlantProperty();
                    property.width = width;
                    property.height = height;
                    property.type = type;
                    property.templateType = templetType;
                    property.texture = Resources.Load("Textures/" + tex) as Texture2D;

                   switch(property.templateType)
                    {
                        case Global.TemplateType.SMALL_2_2:
                            smallPlants.Add(property);
                            break;
                        case Global.TemplateType.MIDDLE_3_2:
                            middlePlants.Add(property);
                            break;
                        case Global.TemplateType.BIG_4_3:
                            bigPlants.Add(property);
                            break;
                    }
                }

            }

        }
    }
}
