using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantPanelToggle : MonoBehaviour {

    private int isHiding = 1;
    public float smoothing = 10f;
    public GameObject plantPanel;
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
        position[1] = plantPanel.transform.position;
        position[0] = plantPanel.transform.position + new Vector3(0,300f,0);
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

        while (Vector3.Distance(plantPanel.transform.position, target) >= 1f)
        {

            plantPanel.transform.position = Vector3.Lerp(plantPanel.transform.position, target, smoothing * Time.deltaTime);
            yield return null;

        }
    }
}
