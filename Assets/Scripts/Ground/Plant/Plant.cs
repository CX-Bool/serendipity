using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour {

    protected PlantProperty property;
    protected int interval = 2;

	// Use this for initialization
	void Start () {
        InvokeRepeating("Skill", interval, interval);
	}

    protected virtual void Skill() { }

}
