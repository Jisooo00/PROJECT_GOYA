using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MonsterBoss : MonsterBase
{
    protected override void InitMonster()
    {
        base.InitMonster();
        monsterID = "林阜林牢";
        mData = GameData.GetDialog("林阜林牢");
    }
    

}
