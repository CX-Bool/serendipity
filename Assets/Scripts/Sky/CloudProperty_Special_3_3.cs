using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudProperty_Special_3_3 : CloudProperty {


    public override void EnableSubscribe()
    {
        Sky.UseSkill += Skill;
    }
    public override void DisableSubscribe()
    {
        Sky.UseSkill-= Skill;
    }
    public void Skill(int i,int j)
    {
        int left = i - 1 < 0 ? 0 : i - 1;
        int right = i + 1 > Global.HorizonalGridNum ? Global.HorizonalGridNum - 1 : i + 1;
        int bottom = j - 1 >= 0 ? j - 1 : 0 ;
        int top = j + 1 < Global.VerticalGridNum ? j + 1 : Global.VerticalGridNum - 1;
        for(int m=left;m<=right;m++)
        {
            for(int n=top;n>=bottom;n--)
            {
                Sky.GetInstance().RainFall(m, n, 2);

            }
        }
    }
}
