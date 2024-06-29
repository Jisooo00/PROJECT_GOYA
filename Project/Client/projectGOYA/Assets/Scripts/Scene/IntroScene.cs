using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroScene : BaseScene
{
    public Button mBtnStart;
    public Button mBtnSetting;
    public Button mBtnCredit;
    public Button mBtnSignOut;
    public UISignIn mUISignIn;
    public UICredit mUICredit;
    public GameObject mGoMainMenuUI;
    public bool IsSignIn = false;
    
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

    public override void DelFunc()
    {
        
    }

    private void Start()
    {
        
        if (GameData.myData == null)
            GameData.myData = new GameData.UserData();
        
        Global.InitSoundSet();
        
        AudioManager.Instance.PlayBgm();
        
        mBtnStart.onClick.AddListener(delegate
        {
            AudioManager.Instance.PlayClick();
            GameManager.Instance.Scene.LoadSceneByID(GameData.myData.cur_map);
        });
        mBtnSetting.onClick.AddListener(delegate
        {
            AudioManager.Instance.PlayClick();
            PopupManager.Instance.OpenPopupSetting();
        });
        mBtnCredit.onClick.AddListener(delegate
        {
            AudioManager.Instance.PlayClick();
            if(!mUICredit.IsActive)
                mUICredit.gameObject.SetActive(true); 
        });
        
        mBtnSignOut.onClick.AddListener(delegate
        {
            PlayerPrefs.DeleteAll();
            AudioManager.Instance.PlayClick();
            var req = new ReqSignOut();
            req.userUid = GameData.myData.user_uid;
            WebReq.Instance.Request(req, delegate(ReqSignOut.Res res)
            {
                if (res.IsSuccess)
                {
                    PopupManager.Instance.OpenPopupNotice(res.responseMessage);
                    PlayerPrefs.DeleteKey(Global.KEY_USER_ID);
                    PlayerPrefs.DeleteKey(Global.KEY_USER_PW);
                    PlayerPrefs.DeleteKey(Global.KEY_USER_NAME);
                    RefreshUI();
                }
                else
                {
                    PopupManager.Instance.OpenPopupNotice(res.responseMessage+string.Format("\n에러코드 : {0}",res.statusCode));
                }
            });

        });
        
#if UNITY_EDITOR
        mBtnSignOut.gameObject.SetActive(true);
#else
        mBtnSignOut.gameObject.SetActive(false);
#endif
        
        
        if(mUICredit.IsActive)
            mUICredit.gameObject.SetActive(false);

        StartCoroutine("StartAfter");
        //RefreshUI();
    }

    IEnumerator StartAfter()
    {
        yield return null;
        bool bTryLogin = false;
        bool bFailLogin = false;
        
        //저장된 정보 있으면 로그인 시도
        if (PlayerPrefs.HasKey(Global.KEY_USER_ID) && PlayerPrefs.HasKey(Global.KEY_USER_PW))
        {
            var req = new ReqLogin();
            req.id = PlayerPrefs.GetString(Global.KEY_USER_ID);
            req.pw = PlayerPrefs.GetString(Global.KEY_USER_PW);

            bTryLogin = true;
            WebReq.Instance.Request(req, delegate(ReqLogin.Res res)
            {
                if (res.IsSuccess)
                {
                    IsSignIn = true;
                }
                else
                {
                    bFailLogin = true;
                    PopupManager.Instance.OpenPopupNotice(res.responseMessage +
                                                          string.Format("\n에러코드 : {0}", res.statusCode));

                }
            });
            
            while (true)
            {
                if(IsSignIn || bFailLogin)
                    break;
                yield return null;
            }

            if (IsSignIn)
            {
                StartCoroutine("SignInCompleteAfter");
                yield break;
            }
            
            if(bFailLogin)
            {
                PlayerPrefs.DeleteKey(Global.KEY_USER_ID);
                PlayerPrefs.DeleteKey(Global.KEY_USER_PW);
                RefreshUI();
            }
        }
        else
        {
            RefreshUI();
        }

    }
    
    IEnumerator SignInCompleteAfter()
    {
        yield return null;
        bool bUserCreate = false;
        bool bReqComplete = false;
        WebReq.Instance.Request(new ReqUserInfo(), delegate(ReqUserInfo.Res res)
        {
            bReqComplete = true;
            if (res.IsSuccess)
                bUserCreate = true;
            else
            {
                if (res.statusCode == 400)
                {
                    bUserCreate = false;
                }
                else
                    PopupManager.Instance.OpenPopupNotice(res.responseMessage+string.Format("\n에러코드 : {0}",res.statusCode));
            }
        });
        while (!bReqComplete)
        {
            yield return null;
        }

        if (!bUserCreate)
        {
            RefreshUI();
        }
        else
        {
            Global.InitUserData(); 
            bReqComplete = false;
            WebReq.Instance.Request(new ReqQuestInfo(), delegate(ReqQuestInfo.Res res)
            {
                bReqComplete = true;
            });
            while (!bReqComplete)
            {
                yield return null;
            }
            WebReq.Instance.LoadDialogData();

            SetMenuUI();
        }
    }

    public void SetMenuUI()
    {
        mUISignIn.gameObject.SetActive(false);
        mGoMainMenuUI.SetActive(true);
    }

    public void RefreshUI()
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