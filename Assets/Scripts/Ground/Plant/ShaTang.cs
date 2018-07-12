using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaTang : Plant {

    protected override void Skill()
    {
        base.Skill();

        int left = property.position.x - 2;
        left = left >= 0 ? left : 0;
        //left = property.position.x < Global.HorizonalGridNum ? property.position.x : Global.HorizonalGridNum;
        int right = property.position.x + property.width + 2;
        right = right < Global.HorizonalGridNum ? right : Global.HorizonalGridNum;

        Ground.GetInstance().AddDecreaseLock(left, property.position.x, property.position.y, property.position.y + property.height);
        Ground.GetInstance().AddDecreaseLock(property.position.x + 1, right, property.position.y, property.position.y + property.height);
    }
}
