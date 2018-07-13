using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZhuYu : Plant {

    protected override void Skill()
    {
        base.Skill();

        LevelManager.GetInstance().AddSteps(2);
    }
}
