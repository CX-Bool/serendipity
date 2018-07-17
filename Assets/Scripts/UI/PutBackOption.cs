using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PutBackOption : MonoBehaviour {

    public float smoothing = 10f;
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
	// Use this for initialization
	void Start () {

       
    }

    IEnumerator Movement(Vector3 target)
    {

        while (Vector3.Distance(transform.localPosition, target) >= 0.01f)
        {
            Debug.Log(target);
            transform.localPosition = Vector3.Lerp(transform.localPosition, target, smoothing * Time.deltaTime);
            yield return null;
        }
    }
	
	
}
