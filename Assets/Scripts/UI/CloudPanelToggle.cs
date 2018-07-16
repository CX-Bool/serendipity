using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CloudPanelToggle : MonoBehaviour {

    #region 单例管理
    private static CloudPanelToggle instance;

    private void Awake()
    {
        instance = this;
    }

    public static CloudPanelToggle GetInstance()
    {
        return instance;
    }
    #endregion
    private int isHiding = 0;
    public float smoothing = 10f;
    public RawImage cloudPanel;
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
    /// 委托plantPanelToggle
    /// </summary>
    public delegate void Toggle();
    public static Toggle PanelToggleHandle;

    private void Start()
    {
        position = new Vector3[2];
        position[1] = cloudPanel.rectTransform.localPosition + new Vector3(0, -300f, 0);
        position[0] = cloudPanel.rectTransform.localPosition + new Vector3(0, 0, 0);
        PlantPanelToggle.PanelToggleHandle += ToggleHiding;

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
        Debug.Log(position[isHiding]);
    }
    IEnumerator Movement(Vector3 target)
    {

        while (Vector3.Distance(cloudPanel.rectTransform.localPosition, target) >= 1f)
        {

            cloudPanel.rectTransform.localPosition = Vector3.Lerp(cloudPanel.rectTransform.localPosition, target, smoothing*Time.deltaTime);
            yield return null;

        }

    }
}
