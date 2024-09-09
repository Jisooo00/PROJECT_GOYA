using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class MonsterSanyeah : MonsterBase
{
    private Image mImage = null;
    public List<GameObject> mGoSanyeah;
    public List<Animator> mListAnimator;
    public List<SpriteRenderer> mListSprite;
    public SpriteRenderer mSprite;
    
    protected override void InitMonster()
    {
        base.InitMonster();
        monsterID = "np_0002";
        mData = GameData.GetDialog(monsterID);
        if (GameData.GetQuestData(Global.KEY_QUEST_SANYEAH).GetState() == GameData.QuestData.eState.FINISHED )
        {
            animator = mListAnimator[1];
            mGoSanyeah[1].SetActive(true);
            mGoSanyeah[0].SetActive(false);
            mSprite = mListSprite[1];
        }
        else
        {
            animator = mListAnimator[0];
            mGoSanyeah[0].SetActive(true);
            mGoSanyeah[1].SetActive(false);
            mSprite = mListSprite[0];
        }
    }

}
