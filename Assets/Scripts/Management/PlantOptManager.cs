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
                PlantProperty property = new PlantProperty();
                property.name = xn.SelectSingleNode("Name").InnerText;
                property.width = int.Parse(xn.SelectSingleNode("Width").InnerText);
                property.height = int.Parse(xn.SelectSingleNode("Height").InnerText);
                property.type = (Global.PlantType)System.Enum.Parse(typeof(Global.PlantType), xn.SelectSingleNode("Type").InnerText);
                property.templateType = (Global.TemplateType)System.Enum.Parse(typeof(Global.TemplateType), xn.SelectSingleNode("TemplateType").InnerText);
                property.texture = Resources.Load("Textures/" + xn.SelectSingleNode("TextureName").InnerText) as Texture2D;
                property.moisture = int.Parse(xn.SelectSingleNode("Moisture").InnerText);
                property.skill= xn.SelectSingleNode("Skill").InnerText;
                property.comment= xn.SelectSingleNode("Comment").InnerText;
                property.provenance = xn.SelectSingleNode("Provenance").InnerText;
                property.reference= xn.SelectSingleNode("Reference").InnerText;

                switch (property.templateType)
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
