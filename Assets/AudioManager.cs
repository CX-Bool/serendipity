using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {
    #region 单例管理
    private static AudioManager instance;

    private void Awake()
    {
        instance = this;
    }

    public static AudioManager GetInstance()
    {
        return instance;
    }
    #endregion
    public AudioClip rain;
    private AudioSource audioSource;
	// Use this for initialization
	void Start () {
        audioSource = this.GetComponent<AudioSource>();

    }
    public void PlayAudio(string effect)
    {
        switch (effect)
        {
            case "rain":
                
                AudioSource.PlayClipAtPoint(rain,Camera.main.transform.position);
                
                break;
        }

    }
}
