using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MonsterSanyeah : MonsterBase
{
    protected override void InitMonster()
    {
        base.InitMonster();
        monsterID = "산예";
        mData = GameData.GetDialog("산예");
    }

}