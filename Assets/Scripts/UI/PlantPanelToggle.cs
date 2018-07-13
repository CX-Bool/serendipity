using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlantPanelToggle : MonoBehaviour {

    #region 单例管理
    private static PlantPanelToggle instance;

    private void Awake()
    {
        instance = this;
    }

    public static PlantPanelToggle GetInstance()
    {
        return instance;
    }
    #endregion

    private int isHiding = 1;
    public float smoothing = 10f;
    public RawImage plantPanel;
    private Vector3[] position;
    private Vector3 target;
    public Vector3 Target
    {
        get { return target; }
        set
        {
            target = value;
            StopCoroutine("Movement");
            StartCoroutine("Movement", target);
        }
    }
    /// <summary>
    /// 委托cloudPanelToggle
    /// </summary>
    public delegate void Toggle();
    public static Toggle PanelToggleHandle;

    private void Start()
    {
        position = new Vector3[2];
        position[0] = plantPanel.rectTransform.localPosition + new Vector3(0, 300, 0);
        position[1] = plantPanel.rectTransform.localPosition + new Vector3(0, 0, 0); ;
        CloudPanelToggle.PanelToggleHandle += ToggleHiding;
    }
    public void Click()
    {
        ToggleHiding();

        PanelToggleHandle();

    }
    public void ToggleHiding()
    {
        isHiding = isHiding == 0 ? 1 : 0;
        Target = position[isHiding];
    }
    IEnumerator Movement(Vector3 target)
    {
        while (Vector3.Distance(plantPanel.rectTransform.localPosition, target) >= 1f)
        {

            plantPanel.rectTransform.localPosition = Vector3.Lerp(plantPanel.rectTransform.localPosition, target, smoothing * Time.deltaTime);
            yield return null;

        }

    }
}
