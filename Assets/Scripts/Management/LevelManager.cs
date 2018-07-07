using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class LevelManager : MonoBehaviour {

    #region 单例管理
    private static LevelManager instance;

    private void Awake()
    {
        instance = this;
    }

    public static LevelManager GetInstance()
    {
        return instance;
    }
    #endregion

    #region 关卡数据
    private int steps = 20;
    public int Steps
    {
        get { return steps; }
    }
    #endregion

    // Use this for initialization
    void Start () {

        Global.InitElimTemplate();

    }
	

}
