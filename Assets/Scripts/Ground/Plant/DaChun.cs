using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaChun : Plant {
    protected override void Skill()
    {
        base.Skill();
 
        Ground.GetInstance().ChangeMoisture(0, Global.HorizonalGridNum, property.position.y, property.position.y + property.height,2);
   

    }
}
