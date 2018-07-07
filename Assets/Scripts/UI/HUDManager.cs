﻿using System.Collections;
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

        #region 辅助变量
        private int optWidthOffset = 300;
        private int optHeightPos = 0;

        bool isCloudPanel = true;//当前显示的是否云彩菜单
        #endregion

        public List<CloudProperty> cloudOptionList;
        public Dictionary<PlantProperty,List<Vector2Int>> plantOptionList;

        #region UI资源
        public Text steps;
        /// <summary>
        /// option的图片资源
        /// </summary>
        public List<RawImage> cloudOptImageList;
        public List<RawImage> plantOptImageList;
        public RawImage plantOptPrefab;
        #endregion
        // Use this for initialization
        void Start()
        {
            cloudOptionList = CloudOptManager.GetInstance().optionList;
            plantOptionList = PlantOptManager.GetInstance().optionList;

            EnableSubscribe();
            UpdateOption();

        }
        // Update is called once per frame

        private void EnableSubscribe()
        {
            CloudOptManager.optionChangeHandle += UpdateOption;//选项变更时更新UI
            PlantOptManager.optionChangeHandle += UpdateOption;
        }

        private void UpdateOption()
        {
            if(isCloudPanel)//如果当前可见的是云彩菜单
            {
                for (int i = 0; i < cloudOptionList.Count; i++)
                {
                    cloudOptImageList[i].gameObject.SetActive(true);
                    cloudOptImageList[i].texture = cloudOptionList[i].texture;
                    cloudOptImageList[i].transform.localScale = new Vector3(cloudOptionList[i].width / (float)cloudOptionList[i].height, 1, 1);
                    cloudOptImageList[i].GetComponent<CloudOption>().imgNormalScale = cloudOptImageList[i].transform.localScale;
                    cloudOptImageList[i].GetComponent<CloudOption>().imgReduceScale = cloudOptImageList[i].transform.localScale * 1.2f;
                    cloudOptImageList[i].transform.localPosition = new Vector3(optWidthOffset * (i - 1), optHeightPos, 0);
                }
                for (int i = cloudOptionList.Count; i < CloudOptManager.GetInstance().maxNum; i++)
                {
                    cloudOptImageList[i].gameObject.SetActive(false);
                }
            }
            else//如果当前可见的是植物菜单
            {
                plantOptImageList.Clear();
                foreach(KeyValuePair<PlantProperty,List<Vector2Int>> item in plantOptionList)
                {
                    RawImage plantOption = Instantiate(plantOptPrefab, transform.position, transform.rotation);
                    plantOption.texture = item.Key.texture;
                    plantOption.GetComponent<PlantOption>().imgNormalScale = plantOption.transform.localScale;
                    plantOption.GetComponent<PlantOption>().imgReduceScale = plantOption.transform.localScale*1.2f;
                    plantOption.GetComponent<PlantOption>().transform.parent = transform;

                    plantOptImageList.Add(plantOption);

                }
            }
        }

        private void PutUpCloud(CloudProperty cloudProperty, Vector2 position)
        {

        }

        public void PutBackOption()
        {
            if(isCloudPanel)
            {
                for (int i = 0; i < cloudOptionList.Count; i++)
                    cloudOptImageList[i].gameObject.GetComponent<PutBackOption>().Target = new Vector3(optWidthOffset * (i - 1), optHeightPos, 0);
            }
            else
            {
                for(int i=0;i<plantOptionList.Count;i++)
                {
                    plantOptImageList[i].gameObject.GetComponent<PutBackOption>().Target = new Vector3(optWidthOffset * (i - 1), optHeightPos, 0);
                }
            }
           
        }
        public void SetSteps(int s)
        {
            steps.text = s.ToString();
        }
    }


}
