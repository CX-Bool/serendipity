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

    private List<CloudProperty> options;//可选的云彩种类
    public int maxNum = 3;//最多同时有三个选项
    private int insertIndex = 0;//当前要插入选项的位置
    public List<CloudProperty> optionList;

    public int steps;//剩余步数

	// Use this for initialization
	void Start () {
        options = new List<CloudProperty>();
        optionList = new List<CloudProperty>();

        steps = LevelManager.GetInstance().Steps;

        InitTemplet();
        for (int i = 0; i < maxNum; i++)
            ReloadOption();

        InvokeRepeating("ReloadOption", 3, 2.5f);
    }

    // Update is called once per frame
    void Update () {

    }

    private void ReloadOption()
    {
        if (optionList.Count >= maxNum|| steps == 0)
            return;
        steps--;
        optionList.AddAndNotify(ref insertIndex,RandomNewCloud());
      
    }
    public void RemoveOption(CloudProperty cloudProperty)
    {
        optionList.RemoveAndNotify(ref insertIndex,cloudProperty);
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
        string filepath = System.Environment.CurrentDirectory + "\\Assets\\Resources\\CloudOptionTemplet.xml";

        if (File.Exists(filepath))
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filepath);
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

                    CloudProperty cloudProperty = new CloudProperty();
                    cloudProperty.width = width;
                    cloudProperty.height = height;
                    cloudProperty.data = new int[width, height];
                    cloudProperty.texture= Resources.Load("Textures/"+ tex) as Texture2D;

                    for (int i = 0; i < width; i++)
                        for (int j = 0; j < height; j++)
                        {
                            cloudProperty.data[i, j] = data[i * height + j] - '0';
                        }
                    options.Add(cloudProperty);
                }

            }

        }
    }
    public CloudProperty RandomNewCloud(Global.CloudType type = Global.CloudType.NORMAL)
    {
        if (options.Count == 0)
            return null;
        else
        {
            CloudProperty c= options[Random.Range(0, options.Count)];
            return c;
        }
    }
}

public static class ExtensionMethods
{
    /// <summary>
    /// 扩展方法：自定义的List的Add和Remove
    /// </summary>
    /// <param name="cl">表示调用这个方法的类型是List<CloudProperty></param>
    /// <param name="cloud">真正的参数，要添加到list中的item</param>
    public static void AddAndNotify(this List<CloudProperty> options,ref int index,CloudProperty cloud)
    {
        options.Add(cloud);

        index++;
        
        //CloudOptManager.optionChangeHandle.Method;  
        if (CloudOptManager.optionChangeHandle==null)
        {
            Debug.Log("CloudOptManager.optionChangeHandle in <CloudOptManager> is NULL");
            return;
        }
        CloudOptManager.optionChangeHandle();
       
    }

    public static void RemoveAndNotify(this List<CloudProperty> options,ref int index,CloudProperty cloud)
    {

        if (cloud==null)
        {
            Debug.Log("cloudProperty in <CloudOptManager>.RemoveAndNotify is NULL");
            return;
        }
        index = options.FindIndex((CloudProperty s) => s == cloud);
        options.RemoveAt(index);

        CloudOptManager.optionChangeHandle();
    }
}