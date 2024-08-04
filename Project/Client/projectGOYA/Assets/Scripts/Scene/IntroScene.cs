using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroScene : BaseScene
{
    public Button mBtnStart;
    public Button mBtnReset;
    public Button mBtnSetting;
    public Button mBtnCredit;

    public TMP_Text mTxtStart;
    public TMP_Text mTxtReset;
    
    //public Button mBtnSignOut;
    public UICredit mUICredit;
    public UILoading mUILoading;
    public GameObject mGoMainMenuUI;
    public bool IsSignIn = false;
    public bool IsUserInfoExist = false;

    public bool isBusy = false;
    
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
            if (isBusy)
                return;
            AudioManager.Instance.PlayClick();
            StartCoroutine(StartGameAfter());
        });
        
        mBtnReset.onClick.AddListener(delegate
        {
            if (isBusy)
                return;
            AudioManager.Instance.PlayClick();
            PopupManager.Instance.OpenPopupNotice("현재까지 진행한 게임 정보가 초기화됩니다.\n\n새로하기를 진행하시겠습니까?", delegate
            {
                StartCoroutine(ResetUserAfter());
            },"",true); //TODO 로컬 적용
        });
        
        mBtnSetting.onClick.AddListener(delegate
        {
            if (isBusy)
                return;
            AudioManager.Instance.PlayClick();
            PopupManager.Instance.OpenPopupSetting(delegate
            {
                //TODO 로그인 상태가 변경되었는지 확인
                RefreshUI();
            });
        });
        mBtnCredit.onClick.AddListener(delegate
        {
            if (isBusy)
                return;
            AudioManager.Instance.PlayClick();
            if(!mUICredit.IsActive)
                mUICredit.gameObject.SetActive(true); 
        });
        /*
        mBtnSignOut.onClick.AddListener(delegate
        {
            //PlayerPrefs.DeleteAll();
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

        });*/
        
//        mBtnSignOut.gameObject.SetActive(true);
        
        if(mUICredit.IsActive)
            mUICredit.gameObject.SetActive(false);

        StartCoroutine("StartAfter");
    }

    IEnumerator StartAfter()
    {
        
        mUILoading.gameObject.SetActive(true);

        bool checkNotice = false;
        if (GameData.IsServerMaintainance)
        {
            PopupManager.Instance.OpenPopupNotice(GameData.MaintainanceNoticeMsg, delegate()
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            },"서버 점검 안내");
        }
        else
        {
            checkNotice = true;
        }

        while (!checkNotice)
        {
            yield return null;
        }
        
        if (GameData.IsNoticeExist)
        {
            PopupManager.Instance.OpenPopupNotice(GameData.NoticeMsg, title:"공지");
        }
        
        bool bFailLogin = false;
        
        //저장된 정보 있으면 로그인 시도
        if (PlayerPrefs.HasKey(Global.KEY_USER_ID) && PlayerPrefs.HasKey(Global.KEY_USER_PW))
        {
            var req = new ReqLogin();
            req.id = PlayerPrefs.GetString(Global.KEY_USER_ID);
            req.pw = PlayerPrefs.GetString(Global.KEY_USER_PW);
            
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
                                                          string.Format("\n에러코드 : {0}", res.statusCode), delegate
                    {
                        PopupManager.Instance.OpenPopupAccount(delegate
                        {
                            StartCoroutine("SignInCompleteAfter");
                        });
                    });

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
            }

        }
        else
        {
            PopupManager.Instance.OpenPopupAccount(
                delegate
                {
                    RefreshUI();
                });
            mUILoading.gameObject.SetActive(false);
        }
        
    }
    
    IEnumerator SignInCompleteAfter()
    {
        isBusy = true;
        yield return null;
        bool bReqComplete = false;
        Global.InitUserData();
        WebReq.Instance.Request(new ReqUserInfo(), delegate(ReqUserInfo.Res res)
        {
            bReqComplete = true;
            if (res.IsSuccess)
            {
                
            }
            else
            {
                if (res.statusCode != 400)
                {
                    PopupManager.Instance.OpenPopupNotice(res.responseMessage+string.Format("\n에러코드 : {0}",res.statusCode));
                }
            }
        });
        while (!bReqComplete)
        {
            yield return null;
        }
        RefreshUI();
        SetMenuUI();
        if(mUILoading.gameObject.activeSelf)
            mUILoading.gameObject.SetActive(false);
        isBusy = false;
    }

    IEnumerator StartGameAfter()
    {
        isBusy = true;
        yield return null;
        if (!GameData.myData.bUserInfoExist)
        {
            isBusy = false;
            PopupManager.Instance.OpenPopupSetNickname(delegate
            {
                StartCoroutine(StartGameAfter());
            });
            yield break;
        }
        else
        {
            if(!mUILoading.gameObject.activeSelf)
                mUILoading.gameObject.SetActive(true);
            bool bReqComplete = false;
            WebReq.Instance.Request(new ReqQuestInfo(), delegate(ReqQuestInfo.Res res)
            {
                bReqComplete = true;
            });
            while (!bReqComplete)
            {
                yield return null;
            }
            WebReq.Instance.LoadDialogData();
            while (!GameData.myData.bInitDialog)
            {
                yield return null;
            }
            
            if((GameData.GetQuestData("Qu_0000").GetState() != GameData.QuestData.eState.FINISHED))
            {
                GameManager.Instance.Scene.LoadScene(GameData.eScene.PrologScene);
            }
            else
            {
                GameManager.Instance.Scene.LoadSceneByID(GameData.myData.cur_map);
            }

            isBusy = false;
        }
    }
    
    IEnumerator ResetUserAfter()
    {
        isBusy = true;
        yield return null;
        
        bool bReqComplete = false;
        WebReq.Instance.Request(new ReqInitUser(), delegate(ReqInitUser.Res res)
        {
            bReqComplete = true;
            if (res.IsSuccess)
            {
                PopupManager.Instance.OpenPopupNotice("초기화가 완료되었습니다.", delegate
                {
                    RefreshUI();
                });
            }
            else
            {
                PopupManager.Instance.OpenPopupNotice(res.responseMessage +
                                                      string.Format("\n에러코드 : {0}", res.statusCode));
            }
        });
        while (!bReqComplete)
        {
            yield return null;
        }

        isBusy = false;
    }

    public void SetMenuUI()
    {
        if(!mGoMainMenuUI.gameObject.activeSelf)
            mGoMainMenuUI.SetActive(true);
    }

    public void RefreshUI()
    {
        mBtnReset.gameObject.SetActive(GameData.myData.bUserInfoExist);
        if (GameData.myData.bUserInfoExist)
        {
            mTxtReset.text = "[ 새로하기 ]" ; //TODO 로컬적용   
            mTxtStart.text = "[ 이어하기 ]" ; //TODO 로컬적용   
        }
        else
        {
            mTxtStart.text = "[ 시작하기 ]" ; //TODO 로컬적용   
        }
    }
    
    
}