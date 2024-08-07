using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Monster_np0003 : MonsterBase
{
    protected override void InitMonster()
    {
        base.InitMonster();
        monsterID = "np_0003";
        mData = GameData.GetDialog(monsterID);
    }
    

}
