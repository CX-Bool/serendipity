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
        public Text floatingTextPrefab;
        public Text score;
        public Slider slider;

        public RawImage attention;
        public Texture2D[] attentionTexture;

        /// <summary>
        /// option的图片资源
        /// </summary>
        public List<RawImage> cloudOptImageList;
        public List<RawImage> plantOptImageList;
        public RawImage plantOptPrefab;
        public RawImage sunshineObj;
        public RawImage gameoverCanvas;
        public RawImage plantPanel;

        public RawImage[] arrows;
        #endregion
        // Use this for initialization
        void Start()
        {

            cloudOptionList = CloudOptManager.GetInstance().optionList;
            plantOptionList = PlantOptManager.GetInstance().optionList;
            
            //cloudOptImageList = new List<RawImage>();
            //plantOptImageList = new List<RawImage>();
            EnableSubscribe();
            SetSteps();
            UpdateCloudOption();

            StartCoroutine("InitialSunshine");
        }
        //不知道为什么设置不成功，逼得我写了个这
        IEnumerator InitialSunshine()
        {
            sunshineObj.CrossFadeAlpha(0, 0, true);
            yield return null;
        }

        public void InitScoreSlider(int[]rating,int n)
        {
            slider.interactable = false;

            slider.maxValue = rating[n - 1];
            slider.minValue = 0;
            slider.value = rating[0];
            SetScore((int)slider.value);


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
            while(plantOptImageList.Count>0)
            {
                RawImage p = plantOptImageList[0];
                plantOptImageList.RemoveAt(0);
                for (int j = 0; j < p.transform.childCount; j++)
                {
                    Destroy(p.transform.GetChild(j).gameObject);
                }
                Destroy(p.gameObject);
                p = null;
            }
            plantOptImageList.Clear();

            if (plantOptionList.Count == 0)
            {
                attention.texture = attentionTexture[0];
                return;
            }
            else
            {
                attention.texture = attentionTexture[1];
            }

            int index = 0;
            plantPanel.GetComponent<PutBackOption>().Target = plantPanel.GetComponent<PlantPanel>().right;
            //generate plant option
            foreach (KeyValuePair<PlantProperty, List<Vector2Int>> item in plantOptionList)
            {
                RawImage plantOption = Instantiate(plantOptPrefab, transform.localPosition, transform.rotation);
                plantOption.texture = item.Key.texture;

                plantOption.GetComponent<PlantOption>().canvas = transform.GetComponent<RectTransform>();
                plantOption.transform.SetParent(transform.Find("PlantPanel/MyScrollView"));
                plantOption.transform.localPosition = new Vector3(optWidthOffset * (index+1), optHeightPos, 0);
                plantOption.transform.localScale = new Vector3(1,1,1);

                plantOption.GetComponent<PlantOption>().plantProperty = item.Key;
                plantOption.GetComponent<PlantOption>().position = item.Value;
                plantOption.GetComponentInChildren<Text>().text = item.Key.name;

                plantOptImageList.Add(plantOption);

                index++;
            }
            if(plantOptImageList.Count>=4)
              plantPanel.GetComponent<PlantPanel>().left.x = plantPanel.GetComponent<PlantPanel>().right.x-(plantOptImageList.Count - 3) * (plantOptImageList[0].rectTransform.rect.width+optWidthOffset) ;
            
            //Debug.Log(plantPanel.GetComponent<PlantPanel>().right.x);

        }


        public void PutBackOption()

        {
            
            if (isCloudPanel)
            {
                for (int i = 0; i < cloudOptionList.Count; i++)
                {
                    cloudOptImageList[i].gameObject.GetComponent<PutBackOption>().Target = new Vector3(optWidthOffset * (i - 1), optHeightPos, 0);
                }
            }
            else
            {
                for (int i = 0; i < plantOptionList.Count; i++)
                {
                    plantOptImageList[i].gameObject.GetComponent<PutBackOption>().Target = new Vector3(optWidthOffset * (i + 1), optHeightPos, 0);
                }
            }

        }
        public void SetSteps()
        {
            steps.text = LevelManager.GetInstance().Steps.ToString();
        }
        public void SetScore(int s)
        {
            slider.value = s;
            score.text = s+"/"+slider.maxValue;
        }

        private void TogglePanelFlag()
        { isCloudPanel = !isCloudPanel; }

        public void Sunshine(Vector3 pos)
        {
            sunshineObj.transform.position = Camera.main.WorldToScreenPoint(pos);
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
            t.GetComponent<FloatingText>().SetText(text, Color.gray);
            t.transform.SetParent(steps.transform);
        }

        public void GameOver()
        {
        //    Time.timeScale = 0;
        //    gameoverCanvas.gameObject.SetActive(true);
        }
    }

}
