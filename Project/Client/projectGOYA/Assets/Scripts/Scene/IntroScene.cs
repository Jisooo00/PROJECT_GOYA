using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroScene : BaseScene
{
    public Button mBtnNext;
    public Button mBtnSignOut;
    public UISignIn mUISignIn;
    public GameObject mGoMainMenuUI;
    protected override void InitScene()
    {
        base.InitScene();
        m_eSceneType = GameData.eScene.IntroScene;

        //m_uiManager.Init(m_eSceneType);
    }
    
    public override void Clear(Action del)
    {
        Debug.Log("IntroScene is clear.");
        del();
    }

    private void Start()
    {
        Global.InitUserData(); 
        
        GameData.DialogData[] list = new[] {new GameData.DialogData("주막주인", 1,new KeyValuePair<string,string>[] { new ("np0001_01","어머나? 거기 꼬질꼬질한 도깨비야, 여기서 뭘하는 거니?"),new ("dokkabi_01","김서방이 되고 싶어서 왔어!"),new ("np0001_01","사람이 되고 싶다고?"),new ("np0001_01","내가 시키는 대로 한 번 해볼래?"),new ("dokkabi_01","좋아!!")}, false,3) };
        GameData.dicDialogData.Add("주막주인", list);
        list = new[] {new GameData.DialogData("산예", 1,new KeyValuePair<string,string>[] {new("dokkabi_01","으악 산예를 깨워버렸다!"), new("np0002_01","잠자는 이 산예의 잠을 깨우다니!"), new("np0002_01","나를 신나게 해줘")}, false,4),new GameData.DialogData("산예", 2,new KeyValuePair<string,string>[] { new("np0002_02","나를 이렇게 지치게 하다니"), new("dokkabi_01","[산예의 수염]을 얻었다!")}, false,2) };
        GameData.dicDialogData.Add("산예", list);
        
        mBtnNext.onClick.AddListener(delegate
        {
            GameManager.Instance.Scene.LoadScene(GameData.eScene.MainScene);
        });
        
        mBtnSignOut.onClick.AddListener(delegate
        {
            var req = new ReqSignOut();
            req.userUid = UserData.myData.user_uid;
            WebReq.Instance.Request(req, delegate(ReqSignOut.Res res)
            {
                if (res.IsSuccess)
                {
                    PopupManager.Instance.OpenPopupNotice(res.responseMessage);
                    PlayerPrefs.DeleteKey(Global.KEY_USER_ID);
                    PlayerPrefs.DeleteKey(Global.KEY_USER_PW);
                    RefreshUI();
                }
                else
                {
                    PopupManager.Instance.OpenPopupNotice(res.responseMessage+string.Format("\n에러코드 : {0}",res.statusCode));
                }
            });

        });
        
        RefreshUI();
    }

    public void SetMenuUI()
    {
        mUISignIn.gameObject.SetActive(false);
        mGoMainMenuUI.SetActive(true);
    }

    public void RefreshUI()
    {
        if (PlayerPrefs.HasKey(Global.KEY_USER_ID) && PlayerPrefs.HasKey(Global.KEY_USER_PW))
        {
            var req = new ReqLogin();
            req.id = PlayerPrefs.GetString(Global.KEY_USER_ID);
            req.pw = PlayerPrefs.GetString(Global.KEY_USER_PW);

            WebReq.Instance.Request(req, delegate(ReqLogin.Res res)
            {
                if (res.IsSuccess)
                {
                    Debug.Log("로그인 성공");
                    SetMenuUI();
                }
                else
                {
                    PopupManager.Instance.OpenPopupNotice(res.responseMessage+string.Format("\n에러코드 : {0}",res.statusCode));
                }
                
            });

        }
        else
        {
            mUISignIn.Init(delegate
            {
                SetMenuUI();
            });
            mUISignIn.RefreshAfterSignOut();
            mGoMainMenuUI.SetActive(false);
            mUISignIn.gameObject.SetActive(true);
        }
    }
    
    
}