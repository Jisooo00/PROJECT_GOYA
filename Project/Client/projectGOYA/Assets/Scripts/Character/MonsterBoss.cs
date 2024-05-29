using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MonsterBoss : MonsterBase
{
    protected override void InitMonster()
    {
        base.InitMonster();
        monsterID = "주막주인";
        mData = GameData.GetDialog("주막주인");
    }
    

}
