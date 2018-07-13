using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaChun : Plant {
    protected override void Skill()
    {
        base.Skill();
 
        Ground.GetInstance().ChangeMoisture(0, property.position.y, Global.HorizonalGridNum,  property.height,2);
   

    }
}
