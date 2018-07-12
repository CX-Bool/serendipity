using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;
using view;
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
    #endregion
    public List<PlantProperty> smallPlants;
    public List<PlantProperty> middlePlants;
    public List<PlantProperty> bigPlants;

    public Dictionary<PlantProperty,List<Vector2Int>> optionList;
    /// <summary>
    /// 仅当放置拖动植物选项后通知HUDManager放回或销毁选项
    /// (销毁选项也要调用putBackOption移动其他选项的位置）
    /// </summary>
    public delegate void OptionsChange();
    public static OptionsChange optionChangeHandle;
    // Use this for initialization
    void Start () {
        InitPlantList();
	}
	

    private void InitPlantList()
    {
        smallPlants = new List<PlantProperty>();
        middlePlants = new List<PlantProperty>();
        bigPlants = new List<PlantProperty>();
        optionList = new Dictionary<PlantProperty, List<Vector2Int>>();

        XmlDocument xmlDoc = new XmlDocument();

        xmlDoc.LoadXml(Resources.Load("Config/Plant").ToString());
        if (xmlDoc != null)
        { 
            XmlNodeList cloudList = xmlDoc.SelectSingleNode("main").ChildNodes;
            foreach (XmlNode xn in cloudList)
            {
                string name = xn.SelectSingleNode("Name").InnerText;
                int width = int.Parse(xn.SelectSingleNode("Width").InnerText);
                int height = int.Parse(xn.SelectSingleNode("Height").InnerText);
                int moisture = int.Parse(xn.SelectSingleNode("Moisture").InnerText);

                Global.TemplateType templetType = (Global.TemplateType)System.Enum.Parse(typeof(Global.TemplateType), xn.SelectSingleNode("TemplateType").InnerText);
                string tex = xn.SelectSingleNode("TextureName").InnerText;
                Global.PlantType type = (Global.PlantType)System.Enum.Parse(typeof(Global.PlantType), xn.SelectSingleNode("Type").InnerText);

                PlantProperty property = new PlantProperty();
                property.name = name;
                property.width = width;
                property.height = height;
                property.type = type;
                property.templateType = templetType;
                property.texture = Resources.Load("Textures/" + tex) as Texture2D;
                property.moisture = moisture;

                switch(property.templateType)
                {
                    case Global.TemplateType.SMALL_2_2:
                        smallPlants.Add(property);
                        break;
                    case Global.TemplateType.MIDDLE_3_2:
                        middlePlants.Add(property);
                        break;
                    case Global.TemplateType.BIG_3_3:
                        bigPlants.Add(property);
                        break;
                }
            }

        }

    }

    public void PutBackOption(PlantProperty property)
    {
        //int index = optionList.FindIndex(s => s == cloudProperty);
        //HUDManager.GetInstance().PutBackOption(index);
        HUDManager.GetInstance().PutBackOption();
    }


}
