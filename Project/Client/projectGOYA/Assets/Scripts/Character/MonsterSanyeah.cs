using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MonsterSanyeah : MonsterBase
{
    protected override void InitMonster()
    {
        base.InitMonster();
        monsterID = "�꿹";
        mData = GameData.GetDialog("�꿹");
    }

}