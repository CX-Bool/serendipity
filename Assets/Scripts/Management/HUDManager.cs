using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace view
{
    public class HUDManager : MonoBehaviour
    {

        #region 单例管理
        private static HUDManager instance;

        private void Awake()
        {
            instance = this;
        }

        public static HUDManager GetInstance()
        {
            return instance;
        }
        #endregion
        /// <summary>
        /// option的图片资源
        /// </summary>
        public List<RawImage> cloudOptList;
        public List<CloudProperty> optPropertyList;
        public List<Vector3> originPosition;
        // Use this for initialization
        void Start()
        {
            for (int i = 0; i < cloudOptList.Count; i++)
            {
                originPosition.Add(cloudOptList[i].transform.position);
            }

            EnableSubscribe();
            UpdateOption();

        }
        // Update is called once per frame

        private void EnableSubscribe()
        {
            CloudOptManager.optionChangeHandle += UpdateOption;//选项变更时更新UI
            optPropertyList = CloudOptManager.optionList;
           // Option.EndDragHandle += PutUpCloud;//把云彩放置到天空上后要销毁当前UI
        }

        private void UpdateOption()
        {
            for (int i = 0; i < optPropertyList.Count; i++)
            {
                cloudOptList[i].gameObject.SetActive(true);
                cloudOptList[i].texture = optPropertyList[i].texture;
                cloudOptList[i].transform.localScale = new Vector3(optPropertyList[i].width / (float)optPropertyList[i].height, 1, 1);
                cloudOptList[i].GetComponent<CloudOption>().imgNormalScale = cloudOptList[i].transform.localScale;
                cloudOptList[i].GetComponent<CloudOption>().imgReduceScale = cloudOptList[i].transform.localScale * 1.2f;
                cloudOptList[i].transform.position=originPosition[i];
            }
            for (int i = optPropertyList.Count; i < CloudOptManager.GetInstance().maxNum; i++)
            {
                cloudOptList[i].gameObject.SetActive(false);
            }
        }

        private void PutUpCloud(CloudProperty cloudProperty, Vector2 position)
        {

        }

        public void PutBackOption()
        {
            for(int i=0;i< optPropertyList.Count;i++)
                 cloudOptList[i].gameObject.GetComponent<PutBackOption>().Target = originPosition[i];
        }
    }

}
