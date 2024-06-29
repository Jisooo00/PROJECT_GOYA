using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MonsterSanyeah : MonsterBase
{
    protected override void InitMonster()
    {
        base.InitMonster();
        monsterID = "np_0002";
        mData = GameData.GetDialog(monsterID);
    }

}