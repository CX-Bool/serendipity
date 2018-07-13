using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WuTong : Plant {
    protected override void PrePare()
    {
    }
    protected override void Skill()
    {
        base.Skill();

        CloudOptManager.GetInstance().optionList.AddAndNotify(CloudOptManager.GetInstance().specialOptions[Global.CloudType.SPECIAL_3_3]);
    }

}
