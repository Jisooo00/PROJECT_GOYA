
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainScene : BaseScene
{
    public GameObject SceneMoveBox;
    public GameObject m_uiLoading;
    protected override void InitScene()
    {
        base.InitScene();
        m_eSceneType = GameData.eScene.MainScene;
        WebReq.Instance.Request(new ReqMapEnter(), delegate(ReqMapEnter.Res res) { });
        SetUIManager();
        AudioManager.Instance.StopBgm();
        AudioManager.Instance.PlayBgm();
        Player.instance.PlayEffect("Chara_APPEAR");
    }

    public void Start()
    {
        StartCoroutine(StartAfter());
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

    IEnumerator StartAfter()
    {
        m_uiLoading.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        m_uiLoading.gameObject.SetActive(false);
        
        if (GameData.QuestDatas.ContainsKey("Qu_0001") && GameData.QuestDatas["Qu_0001"].GetState() ==
            GameData.QuestData.eState.UNAVAILABLE)
        {
            var req = new ReqQuestAccept();
            req.questId = "Qu_0001";
            WebReq.Instance.Request(req, delegate(ReqQuestAccept.Res res)
            {
            });
        }

        foreach (var dialog in GameData.GetDialog("np_0001"))
        {
            if (dialog.m_strDialogID == "Dl_0002")
            {
                SceneMoveBox.SetActive(dialog.m_bPlayed);
            }
        }
    }
    
    public override void DelFunc()
    {
        foreach (var dialog in GameData.GetDialog("np_0001"))
        {
            if (dialog.m_strDialogID == "Dl_0002")
            {
                SceneMoveBox.SetActive(dialog.m_bPlayed);
            }
        }
    }
}
