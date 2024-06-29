using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SanyeahScene : BaseScene
{
    public MonsterSanyeah Sanyeah;
    public GameObject mUIEnding;
    
    protected override void InitScene()
    {
        base.InitScene();
        m_eSceneType = GameData.eScene.SanyeahScene;
        WebReq.Instance.Request(new ReqMapEnter(), delegate(ReqMapEnter.Res res) { });
        mUIEnding.SetActive(false);
        mUIEnding.transform.SetAsLastSibling();
        SetUIManager(m_eSceneType);
        if (GameData.GetQuestData("Qu_0002").GetState() != GameData.QuestData.eState.COMPLETED)
        {
            
            Player.instance.transform.localPosition = Vector3.zero;
            if (PlayerPrefs.HasKey(string.Format("{0}_Dl_0004",GameData.myData.user_uid)))
                Sanyeah.animator.SetBool("idle",true);
            else
                Sanyeah.animator.SetBool("idle",false);
            
            if(GameManager.Instance.bClearSanyeah == true) 
                StartCoroutine("ShowEnding");
        }
        else
        {
            
            Sanyeah.animator.SetBool("idle",true);
            Sanyeah.animator.SetBool("awake",false);
        }
        Player.instance.PlayEffect("Chara_APPEAR");
        
        AudioManager.Instance.PlayBgm();
        
    }
    
    IEnumerator ShowEnding()
    {
        mUIEnding.SetActive(true);
        float playTime = 0f;
        while (playTime < 5f)
        {
            playTime += Time.deltaTime;
            yield return null;
        }
        mUIEnding.SetActive(false);
    }

    public void SetSanyeahUp()
    {
        Sanyeah.animator.SetBool("awake",true);
    }    
    public override void Clear(Action del)
    {
        Debug.Log("SanyeahScene is clear.");
        Player.instance.PlayEffect("Chara_DISAPPEAR");
        del();
    }
    
    public override void DelFunc()
    {
        SetSanyeahUp();
    }
}
