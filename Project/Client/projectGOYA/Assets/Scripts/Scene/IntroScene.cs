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
    public TMP_Text mTxtSetting;
    public TMP_Text mTxtCredit;
    public TMP_Text mTxtLoading;
    public TMP_Text mTxtVersion;

    public RectTransform mRtLoadingGuage;

    public UICredit mUICredit;
    public GameObject mUILoading;
    public GameObject mGoMainMenuUI;
    public GameObject mUILoadingScene;
    public bool IsSignIn = false;
    public bool IsUserInfoExist = false;

    public bool isBusy = false;
    
    protected override void InitScene()
    {
        base.InitScene();
        m_eSceneType = GameData.eScene.IntroScene;
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
        Global.InitUISet();
        
        mTxtVersion.text = String.Format("version {0}",Application.version);
        
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
        
        if(mUICredit.IsActive)
            mUICredit.gameObject.SetActive(false);

        mTxtStart.outlineColor = new Color32(77, 42, 50, 255);
        mTxtReset.outlineColor = new Color32(77, 42, 50, 255);
        mTxtSetting.outlineColor = new Color32(77, 42, 50, 255);
        mTxtCredit.outlineColor = new Color32(77, 42, 50, 255);
        mTxtLoading.outlineColor = new Color32(54, 99, 135, 255);
        mTxtLoading.text = "돗가비 깨우는 중...";

        StartCoroutine("StartAfter");
    }

    IEnumerator StartAfter()
    {
        
        mUILoading.gameObject.SetActive(true);
        mGoMainMenuUI.gameObject.SetActive(false);

        float gauge = 0f;
        mRtLoadingGuage.localScale = new Vector3(0f, 1, 1);
        
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

        while (!checkNotice ||  gauge <0.25f)
        {
            if(gauge < 0.25f)
            {
                gauge += Time.deltaTime;
                mRtLoadingGuage.localScale = new Vector3(gauge, 1, 1);
            }

            yield return null;
        }
        
        if (GameData.IsNoticeExist)
        {
            PopupManager.Instance.OpenPopupNotice(GameData.NoticeMsg, title:"공지");
        }

        if (GameData.NeedDownloadDialog)
        {
            WebReq.Instance.LoadDialogData();
            while (!GameData.myData.bInitDialog)
            {
                yield return null;
            }

            GameData.NeedDownloadDialog = false;
        }
        //GameData.TempDemoNotice();


        isBusy = true;
        bool bFailLogin = false;
        
        //저장된 정보 있으면 로그인 시도
        if (SaveDataManager.Instance.bLoginInfo)
        {
            var req = new ReqLogin();
            req.id = SaveDataManager.Instance.LoginID;
            req.pw = SaveDataManager.Instance.LoginPW;
            
            WebReq.Instance.Request(req, delegate(ReqLogin.Res res)
            {
                isBusy = false;
                if (res.IsSuccess)
                {
                    IsSignIn = true;
                }
                else
                {
                    bFailLogin = true;
                    PopupManager.Instance.OpenPopupNotice(res.responseMessage , delegate
                    {
                        mUILoading.gameObject.SetActive(false);
                        PopupManager.Instance.OpenPopupAccount(delegate
                        {
                            StartCoroutine("SignInCompleteAfter");
                        });
                    });

                }
            });
            
            //TODO 출시버전 수정 필요

            if (IsSignIn)
            {
                StartCoroutine("SignInCompleteAfter");
            }
            

        }
        else
        {   
            var req = new ReqGuestSignUp();
            WebReq.Instance.Request(req, delegate(ReqGuestSignUp.Res res)
            {
                if (res.IsSuccess)
                {
                    var req2 = new ReqLogin();
                    req2.id = res.data.id;
                    req2.pw = res.data.pw;

                    WebReq.Instance.Request(req2, delegate(ReqLogin.Res res2)
                    {
                        isBusy = false;
                        if (res2.IsSuccess)
                        {
                            IsSignIn = true;
                        }
                        else
                        {
                            PopupManager.Instance.OpenPopupNotice(res2.responseMessage);
                        }
                    });
                }
                else
                {
                    PopupManager.Instance.OpenPopupNotice(res.responseMessage);
                }


            });
            
            while (!IsSignIn || gauge <0.5f)
            {
                if(gauge < 0.5f)
                {
                    gauge += Time.deltaTime;
                    mRtLoadingGuage.localScale = new Vector3(gauge, 1, 1);
                }
                yield return null;
            }
            
            
            //TODO 출시버전 수정 필요
            
            
            PopupManager.Instance.OpenPopupAccount(
                delegate
                {
                    SetMenuUI();
                    RefreshUI();
                });
                
            mUILoading.gameObject.SetActive(false);
            
        }
        
        while (true)
        {
            if(!isBusy && gauge >0.85f) break;
                
            if(gauge < 0.85f)
            {
                gauge += Time.deltaTime;
                mRtLoadingGuage.localScale = new Vector3(gauge, 1, 1);
            }
                
            yield return null;
                
        }
        
        if (IsSignIn)
        {
            StartCoroutine("SignInCompleteAfter");
        }
        else
        {
            PopupManager.Instance.OpenPopupNotice("네트워크 연결이 원활하지 않습니다.", delegate
            {
                StopAllCoroutines();
                GameManager.Instance.Scene.LoadScene(GameData.eScene.FirstScene);
            });
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
                    PopupManager.Instance.OpenPopupNotice(res.responseMessage);
                }
            }
        });
        while (!bReqComplete)
        {
            yield return null;
        }
        mRtLoadingGuage.localScale = new Vector3(1f, 1, 1);
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
            if(!mUILoadingScene.gameObject.activeSelf)
                mUILoadingScene.gameObject.SetActive(true);
            
            
            bool bReqComplete = false;
            WebReq.Instance.Request(new ReqQuestInfo(), delegate(ReqQuestInfo.Res res)
            {
                bReqComplete = true;
            });
            while (!bReqComplete)
            {
                yield return null;
            }

            if (GameData.myData.isNoAcceptQuest)
            {
                bReqComplete = false;
                var _req = new ReqQuestAccept();
                _req.questId = "Qu_0000";
                WebReq.Instance.Request(_req, delegate(ReqQuestAccept.Res res)
                {
                    bReqComplete = true;
                });
                while (!bReqComplete)
                {
                    yield return null;
                }
            }
            
            SaveDataManager.Instance.InitDialog();
            SaveDataManager.Instance.SetDialogByQuestInfo();
            GameData.SetGameDialogData();

            mTxtVersion.gameObject.SetActive(false);
            AudioManager.Instance.StopBgm();
            if(GameData.myData.curQuest == Global.KEY_QUEST_TUTO)
            {
                GameManager.Instance.Scene.LoadScene(GameData.eScene.PrologScene);
            }
            else
            {
                Debug.Log(GameData.myData.cur_map);
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
                PopupManager.Instance.OpenPopupNotice(res.responseMessage);
            }
        });
        while (!bReqComplete)
        {
            yield return null;
        }
        
        SaveDataManager.Instance.InitDialog();
        

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
            mTxtReset.text = "새로하기" ;
            mTxtStart.text = "이어하기" ; 
        }
        else
        {
            mTxtStart.text = "시작하기" ;
        }
    }
    
    
}