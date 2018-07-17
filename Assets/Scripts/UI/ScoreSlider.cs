using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ScoreSlider : MonoBehaviour {

    private Slider slider;
    public int score;
	// Use this for initialization
	void Start () {
        slider = gameObject.GetComponent<Slider>();
	}
	

}
