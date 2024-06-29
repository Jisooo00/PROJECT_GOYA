
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainScene : BaseScene
{
    public GameObject SceneMoveBox;
    protected override void InitScene()
    {
        base.InitScene();
        m_eSceneType = GameData.eScene.MainScene;
        WebReq.Instance.Request(new ReqMapEnter(), delegate(ReqMapEnter.Res res) { });
        SetUIManager(m_eSceneType);
        AudioManager.Instance.StopBgm();
        AudioManager.Instance.PlayBgm();
        Player.instance.PlayEffect("Chara_APPEAR");
    }

    public void Start()
    {
        if (GameData.QuestDatas.ContainsKey("Qu_0001") && GameData.QuestDatas["Qu_0001"].GetState() ==
            GameData.QuestData.eState.UNAVAILABLE)
        {
            var req = new ReqQuestAccept();
            req.questId = "Qu_0001";
            WebReq.Instance.Request(req,delegate(ReqQuestAccept.Res res){});
        }
        if(GameData.QuestDatas.ContainsKey("Qu_0002") && GameData.QuestDatas["Qu_0002"].GetState() ==
           GameData.QuestData.eState.ACCOMPLISHING)
            SceneMoveBox.SetActive(true);
        else
            SceneMoveBox.SetActive(false);
            
    }

    public override void Clear(Action del)
    {
        Player.instance.PlayEffect("Chara_DISAPPEAR");
        if (del != null)
        {
            StartCoroutine(ClearSceneAfter(del));
        }
        Debug.Log("MainScene is clear.");

    }
        
    IEnumerator ClearSceneAfter(Action del)
    {
        yield return new WaitForSeconds(0.5f);
        del();
    }
    
    public override void DelFunc()
    {
        if(GameData.QuestDatas.ContainsKey("Qu_0002") && GameData.QuestDatas["Qu_0002"].GetState() ==
           GameData.QuestData.eState.ACCOMPLISHING)
            SceneMoveBox.SetActive(true);
        else
            SceneMoveBox.SetActive(false);
    }
}
