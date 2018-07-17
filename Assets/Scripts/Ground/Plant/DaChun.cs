using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaChun : Plant {
    protected override void Skill()
    {
        base.Skill();
 
        Ground.GetInstance().ChangeMoisture(0, property.position.y, Global.HorizonalGridNum,  property.height,2);


        int left = property.position.x - 2;
        left = left >= 0 ? left : 0;
        //left = property.position.x < Global.HorizonalGridNum ? property.position.x : Global.HorizonalGridNum;
        int right = property.position.x + property.width + 2;
        right = right < Global.HorizonalGridNum ? right : Global.HorizonalGridNum;

        Ground.GetInstance().ChangeMoisture(left, property.position.y, 2, property.height, 2) ;
        Ground.GetInstance().ChangeMoisture(property.position.x+property.width, property.position.y, 2, property.height, 2) ;
    }
}
