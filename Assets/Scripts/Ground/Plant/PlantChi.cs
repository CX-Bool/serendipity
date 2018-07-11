using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantChi:Plant {

    protected override void Skill()
    {
        base.Skill();
        LevelManager.GetInstance().AddSteps(1);
    }
}
