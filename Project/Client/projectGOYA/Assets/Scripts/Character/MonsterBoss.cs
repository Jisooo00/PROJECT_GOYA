using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MonsterBoss : MonsterBase
{
    protected override void InitMonster()
    {
        base.InitMonster();
        monsterID = "�ָ�����";
        mData = GameData.GetDialog("�ָ�����");
    }
    

}
