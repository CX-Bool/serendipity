using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainWater : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Invoke("DestoryMyself", 3);
	}

    private void DestoryMyself()
    {
        Destroy(gameObject);
    }
}
