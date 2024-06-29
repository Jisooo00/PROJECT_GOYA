using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MonsterBoss : MonsterBase
{
    protected override void InitMonster()
    {
        base.InitMonster();
        monsterID = "np_0001";
        mData = GameData.GetDialog(monsterID);
        

    }
    

}
