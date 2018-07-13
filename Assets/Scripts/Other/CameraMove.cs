using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour {

    public float smoothing = 7f;
    private Quaternion target;
    public Quaternion Target
    {
        get { return target; }
        set
        {
            target = value;

            StopCoroutine("Movement");
            StartCoroutine("Movement", target);

        }
    }
    private void Start()
    {
       // Screen.SetResolution(540, 960, false);
     
    }

    // Use this for initialization
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Target = Quaternion.Euler(0, 35, 0);
        }
        if(Input.GetKeyDown(KeyCode.LeftArrow))
            Target = Quaternion.Euler(0, 0, 0);
        if (Input.GetKeyDown(KeyCode.DownArrow))
            transform.position += new Vector3(0, 0, -1);
    }

    IEnumerator Movement(Quaternion target)
    {
        while (transform.rotation!=target)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, target, smoothing*Time.deltaTime);
            yield return null;
        }
    }
	
	
}
