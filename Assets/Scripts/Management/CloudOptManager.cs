using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;
using System.Collections;
using view;
/// <summary>
/// 管理云彩选项
/// </summary>
public class CloudOptManager : MonoBehaviour {

    #region 单例管理
    private static CloudOptManager instance;

    private void Awake()
    {
        instance = this;
    }

    public static CloudOptManager GetInstance()
    {
        return instance;
    }
    #endregion

    
    /// <summary>
    /// OptionsChange用于选项列表发生变化时，1 通知HUDManager更新UI
    /// 2 向option的gameObject传递cloudProperty
    /// </summary>
    public delegate void OptionsChange();
    public static OptionsChange optionChangeHandle;

    private List<CloudProperty> options;//可选的NORMAL类型云彩
    public Dictionary<Global.CloudType, CloudProperty> specialOptions;

    int maxImageWidth = 300;//选项最大宽度

    public int maxNum = 3;//最多同时有三个选项
    //private int insertIndex = 0;//当前要插入选项的位置
    public List<CloudProperty> optionList;//选项
   
    // Use this for initialization
    void Start () {
        options = new List<CloudProperty>();
        optionList = new List<CloudProperty>();
        specialOptions = new Dictionary<Global.CloudType, CloudProperty>();

        InitTemplet();
        for (int i = 0; i < maxNum; i++)
            ReloadOption();

        InvokeRepeating("ReloadOption", 3, 2.5f);
    }

    private void ReloadOption()
    {
        if (optionList.Count >= maxNum|| LevelManager.GetInstance().Steps == 0)
            return;
        optionList.AddAndNotify(RandomNewCloud());
      
    }
    public void RemoveOption(CloudProperty cloudProperty)
    {
        LevelManager.GetInstance().Steps--;
        optionList.RemoveAndNotify(cloudProperty);
    }
    public void PutBackOption(CloudProperty cloudProperty)
    {
        //int index = optionList.FindIndex(s => s == cloudProperty);
        //HUDManager.GetInstance().PutBackOption(index);
        HUDManager.GetInstance().PutBackOption();
    }
    private void InitTemplet()
    {
        //read from xml

        XmlDocument xmlDoc = new XmlDocument();

        xmlDoc.LoadXml(Resources.Load("Config/CloudOptionTemplet").ToString());
        if (xmlDoc != null)
        {
            XmlNodeList cloudList = xmlDoc.SelectSingleNode("main").ChildNodes;
            foreach (XmlNode xn in cloudList)
            {
                int width = int.Parse(xn.SelectSingleNode("width").InnerText);
                int height = int.Parse(xn.SelectSingleNode("height").InnerText);
                string data = xn.SelectSingleNode("data").InnerText;
                string tex = xn.SelectSingleNode("textureName").InnerText;
                Global.CloudType type = (Global.CloudType)System.Enum.Parse(typeof(Global.CloudType), xn.SelectSingleNode("type").InnerText);

                CloudProperty cloudProperty = null;
                switch(type)
                {
                    case Global.CloudType.SPECIAL_3_3:
                        cloudProperty  = new CloudProperty_Special_3_3();
                        break;
                    default:
                        cloudProperty = new CloudProperty();
                        break;
                }

                cloudProperty.width = width;
                cloudProperty.height = height;
                cloudProperty.data = new int[width, height];
                cloudProperty.texture= Resources.Load("Textures/"+ tex) as Texture2D;
                cloudProperty.type = type;
                for (int i = 0; i < width; i++)
                    for (int j = 0; j < height; j++)
                    {
                        cloudProperty.data[i, j] = data[i * height + j] - '0';
                    }

                if (type == Global.CloudType.NORMAL)
                    options.Add(cloudProperty);
                else specialOptions.Add(type, cloudProperty);
            }

        }

    }
    public CloudProperty RandomNewCloud(Global.CloudType type = Global.CloudType.NORMAL)
    {
        if (options.Count == 0)
            return null;

        if (type == Global.CloudType.NORMAL)
        {
            return options[Random.Range(0, options.Count)];
        }
        else
        {
            return specialOptions[type];
        }
    }
}

public static class List_CloudProperty_ExtensionMethods
{
    /// <summary>
    /// 扩展方法：自定义的List的Add和Remove
    /// </summary>
    /// <param name="cl">表示调用这个方法的类型是List<CloudProperty></param>
    /// <param name="cloud">真正的参数，要添加到list中的item</param>
    public static void AddAndNotify(this List<CloudProperty> options,CloudProperty cloud)
    {
        options.Add(cloud);

        //CloudOptManager.optionChangeHandle.Method;  
        if (CloudOptManager.optionChangeHandle==null)
        {
            Debug.Log("CloudOptManager.optionChangeHandle in <CloudOptManager> is NULL");
            return;
        }
        CloudOptManager.optionChangeHandle();
       
    }

    public static void RemoveAndNotify(this List<CloudProperty> options,CloudProperty cloud)
    {

        if (cloud==null)
        {
            Debug.Log("cloudProperty in <CloudOptManager>.RemoveAndNotify is NULL");
            return;
        }
        // index = options.FindIndex((CloudProperty s) => s == cloud);
        options.Remove(cloud);

        CloudOptManager.optionChangeHandle();
    }
}