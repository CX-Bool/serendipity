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

        #region 辅助变量
        private int optWidthOffset = 300;
        private int optHeightPos = 0;

        bool isCloudPanel = true;//当前显示的是否云彩菜单
        #endregion

        public List<CloudProperty> cloudOptionList;
        public Dictionary<PlantProperty, List<Vector2Int>> plantOptionList;

        #region UI资源
        public Text steps;
        public Text score;
        public Text floatingTextPrefab;
        /// <summary>
        /// option的图片资源
        /// </summary>
        public List<RawImage> cloudOptImageList;
        public List<RawImage> plantOptImageList;
        public RawImage plantOptPrefab;
        public RawImage sunshineObj;
        #endregion
        // Use this for initialization
        void Start()
        {

            cloudOptionList = CloudOptManager.GetInstance().optionList;
            plantOptionList = PlantOptManager.GetInstance().optionList;
            //cloudOptImageList = new List<RawImage>();
            //plantOptImageList = new List<RawImage>();
            EnableSubscribe();
            Debug.Log("HUDManager Start----1");
            SetSteps();
            UpdateCloudOption();

            StartCoroutine("InitialSunshine");
        }
        // Update is called once per frame
        //不知道为什么设置不成功，逼得我写了个这
        IEnumerator InitialSunshine()
        {
            sunshineObj.CrossFadeAlpha(0, 0, true);
            yield return null;
        }
        private void EnableSubscribe()
        {
            CloudOptManager.optionChangeHandle += UpdateCloudOption;//选项变更时更新UI
            //云彩panel可见时是无需更新植物panel的，只有当植物panel升上来，才有必要更新
            CloudPanelToggle.PanelToggleHandle += UpdatePlantOption;

            CloudPanelToggle.PanelToggleHandle += TogglePanelFlag;
            PlantPanelToggle.PanelToggleHandle += TogglePanelFlag;

            PlantOptManager.optionChangeHandle += UpdatePlantOption;
        }

        //CloudOption是重复使用的，每次调用只是设置是否可见，放回正确位置
        private void UpdateCloudOption()
        {
            int count = cloudOptionList.Count>CloudOptManager.GetInstance().maxNum? CloudOptManager.GetInstance().maxNum: cloudOptionList.Count;
            for (int i = 0; i < count; i++)
            {
                cloudOptImageList[i].gameObject.SetActive(true);
                Debug.Log("HUDManager Start----2");

                cloudOptImageList[i].texture = cloudOptionList[i].texture;
                cloudOptImageList[i].transform.localScale = new Vector3(cloudOptionList[i].width / 2f, (float)cloudOptionList[i].height / 2f, 1);
                cloudOptImageList[i].GetComponent<CloudOption>().imgNormalScale = cloudOptImageList[i].transform.localScale;
                cloudOptImageList[i].GetComponent<CloudOption>().imgReduceScale = cloudOptImageList[i].transform.localScale * 1.2f;
                cloudOptImageList[i].transform.localPosition = new Vector3(optWidthOffset * (i - 1), optHeightPos, 0);
            }
            for (int i = count; i < CloudOptManager.GetInstance().maxNum; i++)
            {
                cloudOptImageList[i].gameObject.SetActive(false);
            }
        }

        private void UpdatePlantOption()
        {
            //clear plant option
            for (int i = 0; i < plantOptImageList.Count; i++)
            {
                Destroy(plantOptImageList[i]);
            }
            plantOptImageList.Clear();
            int index = 0;
            //generate plant option
            foreach (KeyValuePair<PlantProperty, List<Vector2Int>> item in plantOptionList)
            {
                RawImage plantOption = Instantiate(plantOptPrefab, transform.localPosition, transform.rotation);
                plantOption.texture = item.Key.texture;

                plantOption.GetComponent<PlantOption>().canvas = transform.GetComponent<RectTransform>();
                plantOption.transform.SetParent(transform.Find("PlantPanel"));
                plantOption.transform.localPosition = new Vector3(optWidthOffset * (index - 1), optHeightPos, 0);

                plantOption.GetComponent<PlantOption>().plantProperty = item.Key;
                plantOption.GetComponent<PlantOption>().position = item.Value;

                plantOptImageList.Add(plantOption);
                index++;
            }
        }


        public void PutBackOption()
        {
            if (isCloudPanel)
            {
                for (int i = 0; i < cloudOptionList.Count; i++)
                    cloudOptImageList[i].gameObject.GetComponent<PutBackOption>().Target = new Vector3(optWidthOffset * (i - 1), optHeightPos, 0);
            }
            else
            {
                for (int i = 0; i < plantOptionList.Count; i++)
                {
                    plantOptImageList[i].gameObject.GetComponent<PutBackOption>().Target = new Vector3(optWidthOffset * (i - 1), optHeightPos, 0);
                }
            }

        }
        public void SetSteps()
        {
            steps.text = LevelManager.GetInstance().Steps.ToString();
        }
        public void SetScore()
        {
            score.text = LevelManager.GetInstance().Score.ToString();
        }

        private void TogglePanelFlag()
        { isCloudPanel = !isCloudPanel; }

        public void Sunshine(int pos)
        {
            sunshineObj.transform.position = new Vector3(Screen.currentResolution.width * (float)(pos) / (Global.HorizonalGridNum - 2),
                sunshineObj.transform.position.y, sunshineObj.transform.position.z);
            StartCoroutine("DisplaySunshine");
        }

        IEnumerator DisplaySunshine()
        {
            sunshineObj.CrossFadeAlpha(1, 3, true);

            yield return new WaitForSecondsRealtime(3);

            sunshineObj.CrossFadeAlpha(0, 2, true);
        }

        public void GenerateFloatingText(string text,Vector3 position,Quaternion rotation)
        {
            Text t=Instantiate(floatingTextPrefab, position, rotation);
            t.GetComponent<FloatingText>().SetText(text, Color.green);
            t.transform.SetParent(steps.transform);
        }
    }

}
