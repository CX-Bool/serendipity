﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XuanCao : Plant {
    protected override void Skill()
    {
        base.Skill();

        CloudOptManager.GetInstance().optionList.Add(CloudOptManager.GetInstance().specialOptions[Global.CloudType.NORMAL_1_1]);
    }
}
