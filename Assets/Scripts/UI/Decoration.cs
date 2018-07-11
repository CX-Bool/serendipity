using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decoration : MonoBehaviour {

    float phase;
	// Use this for initialization
	void Start () {
		phase = Random.Range(0, 2);
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        phase += 0.01f;
        phase %= 100000;
        transform.Translate(0,0,Mathf.Sin(phase)*0.01f);
	}
}
