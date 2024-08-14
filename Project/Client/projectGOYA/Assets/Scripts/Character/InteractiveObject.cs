using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InteractiveObject : MonsterBase
{
    protected override void InitMonster()
    {
        base.InitMonster();
        mData = GameData.GetDialog(monsterID);
    }
    

}