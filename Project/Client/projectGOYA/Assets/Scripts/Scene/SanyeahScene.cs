using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SanyeahScene : BaseScene
{
    public MonsterSanyeah Sanyeah;
    public GameObject mUIEnding;
    public GameObject m_uiLoading;
    public Transform m_tmPosFromMain;
    public Transform m_tmPosFrontSanyeah;
    public GameObject m_goNightEnvironment;
    public SpriteRenderer m_spriteSanyeah;
    public SpriteRenderer m_spritePlayer;
    
    protected override void InitScene()
    {
        base.InitScene();
        m_eSceneType = GameData.eScene.SanyeahScene;
        StartCoroutine(StartAfter());
    }


    IEnumerator StartAfter()
    {
        var moveFirst = new Vector2(0, 0);
        
        m_uiLoading.gameObject.SetActive(true);

        if(GameData.myData.isSanyeahQuest)
        {
            Player.instance.transform.localPosition = m_tmPosFrontSanyeah.localPosition;
        }
        else
        {
            Player.instance.transform.localPosition = m_tmPosFromMain.localPosition;
            moveFirst = Vector2.left;
        }
        
        yield return new WaitForSeconds(0.5f);
        m_uiLoading.gameObject.SetActive(false);
        
        if (moveFirst != Vector2.zero)
        {
            Player.instance.SetInputPos(moveFirst);
            yield return new WaitForSeconds(0.5f);
            Player.instance.SetInputPos(Vector2.zero);
        }
        
        WebReq.Instance.Request(new ReqMapEnter(), delegate(ReqMapEnter.Res res) { });
        mUIEnding.SetActive(false);
        mUIEnding.transform.SetAsLastSibling();
        SetUIManager();
        //Player.instance.PlayEffect("Chara_APPEAR");
        
        if (GameData.GetQuestData(Global.KEY_QUEST_SANYEAH_WAKE).GetState() != GameData.QuestData.eState.FINISHED)
        {
            Sanyeah.animator.SetBool("idle",false);
        }else if (GameData.GetQuestData(Global.KEY_QUEST_SANYEAH).GetState() == GameData.QuestData.eState.ACCOMPLISHING)
        {
            Sanyeah.animator.SetBool("idle",true);
            Sanyeah.animator.SetBool("awake",false);
        }
        else
        {
            //Player.instance.transform.localPosition = Vector3.zero;
            Sanyeah.animator.SetBool("idle",true);
            Sanyeah.animator.SetBool("awake",false);
            
            if (GameData.GetQuestData(Global.KEY_QUEST_SANYEAH_CLEAR).GetState() == GameData.QuestData.eState.ACCOMPLISHING)
            {
                StartCoroutine("ShowEnding");
            }
            
        }
        
        AudioManager.Instance.PlayBgm();
        

    }

    
    IEnumerator ShowEnding()
    {
        mUIEnding.SetActive(true);
        
        //PlayerPrefs.SetString(string.Format("{0}_{1}", GameData.myData.user_name, "Dl_0006"), "true");
        //PlayerPrefs.Save();
        //GameData.SetDialogPlayed("np_0002","Dl_0006");
        
        float playTime = 0f;
        while (playTime < 5f)
        {
            playTime += Time.deltaTime;
            yield return null;
        }
        mUIEnding.SetActive(false);
        
        //m_goNightEnvironment.gameObject.SetActive(true);
        //Sanyeah.mSprite.sortingOrder = 11;
       // m_spritePlayer.sortingOrder = 11;
        m_uiManager.PlayDialogForce("Dl_0007", delegate
        {
            //Sanyeah.mSprite.sortingOrder = 9;
            //m_spritePlayer.sortingOrder = 9;
            GameManager.Instance.bClearSanyeah = false;
        });
    }

    public void SetSanyeahUp()
    {
        Sanyeah.animator.SetBool("awake",true);
    }    
    public override void Clear(Action del)
    {
        Debug.Log("SanyeahScene is clear.");
        //Player.instance.PlayEffect("Chara_DISAPPEAR");
        del();
    }
    
    public override void DelFunc()
    {
        SetSanyeahUp();
    }
}
