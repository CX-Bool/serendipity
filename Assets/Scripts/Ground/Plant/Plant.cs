﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour {

    public PlantProperty property;
    protected int interval = 30;

	// Use this for initialization
	void Start () {
        // InvokeRepeating("Skill", interval, interval);
        Skill();
	}

    protected virtual void Skill() { }

}
