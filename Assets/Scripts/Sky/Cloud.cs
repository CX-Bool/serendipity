using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour{

    public CloudProperty cloudProperty;
    public Cloud(CloudProperty property)
    {
        cloudProperty = property;
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    protected void Init()
    {
        Debug.Log("cloud");
        //for (int i = 0; i < width; i++)
        //    for (int j = 0; j < height; j++)
        //    {
        //        cloud[i, j] = Random.Range(0, 3) > 2 ? 1 : 0;
        //    }
    }

}
