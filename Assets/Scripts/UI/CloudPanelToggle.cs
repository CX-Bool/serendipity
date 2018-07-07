using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudPanelToggle : MonoBehaviour {

    private int isHiding = 0;
    public float smoothing = 10f;
    public GameObject cloudPanel;
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
        position[1] = cloudPanel.transform.position - new Vector3(0, 300f, 0);
        position[0] = cloudPanel.transform.position;
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
    }
    IEnumerator Movement(Vector3 target)
    {

        while (Vector3.Distance(cloudPanel.transform.position, target) >= 1f)
        {

            cloudPanel.transform.position = Vector3.Lerp(cloudPanel.transform.position, target, smoothing*Time.deltaTime);
            yield return null;

        }
       // cloudPanel.SetActive((isHiding==0));
    }
}
