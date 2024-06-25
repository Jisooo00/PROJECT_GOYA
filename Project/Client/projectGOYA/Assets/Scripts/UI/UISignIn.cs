using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISignIn : MonoBehaviour
{

    [Header("Sign In / Sign Up UI")] 
    public GameObject mGoSignUI;
    public TMP_Text mTextIDLabel;
    public TMP_Text mTextPWLabel;
    public TMP_Text mTextPWCheckLabel;
    public TMP_Text mTextPWCheckNotice;
    public TMP_InputField mInputID;
    public TMP_InputField mInputPW;
    public TMP_InputField mInputPWCheck;
    public GameObject mGoSingUp;
    public RectTransform mRectID;
    public RectTransform mRectPW;
    public Button mBtnSignIn;
    public Button mBtnSignUp;
    public TMP_Text mTextBtnSingIn;
    public TMP_Text mTextBtnSignUp;
    public Action mDelLoginAfter;
    private bool mIsOnGoingSignUp;

    [Header("Set UID UI")] 
    public GameObject mGoUIDUI;
    public TMP_Text mTextUIDLabel;
    public TMP_Text mTextDescUID;
    public TMP_InputField mInputUID;
    public Button mBtnSetUID;
    public TMP_Text mTextBtnSetUID;

    

    public void Start()
    {
        mTextIDLabel.text = "ID";
        mTextPWLabel.text = "PW";
        mTextPWCheckLabel.text = "PW'";
        mTextBtnSingIn.text = "로그인";
        mTextBtnSignUp.text = "회원가입";
        mTextUIDLabel.text = "닉네임";
        mTextDescUID.text = "1~5글자를 입력해주세요.";
        mTextBtnSetUID.text = "확인";

        if (PlayerPrefs.HasKey(Global.KEY_USER_ID) && PlayerPrefs.HasKey(Global.KEY_USER_PW))
        {
            mGoSignUI.SetActive(false);
            mGoUIDUI.SetActive(true);
        }
        else
        {
            mGoSignUI.SetActive(true);
            mGoUIDUI.SetActive(false);
        }
        
        mGoSingUp.SetActive(false);
        
        mBtnSignIn.onClick.AddListener(delegate
        {
            if (string.IsNullOrEmpty(mInputID.text))
            {
                PopupManager.Instance.OpenPopupNotice("아이디를 입력하세요.");
                return;
            }
            if (string.IsNullOrEmpty(mInputPW.text))
            {
                PopupManager.Instance.OpenPopupNotice("패스워드를 입력하세요.");
                return;
            }

            var req = new ReqLogin();
            req.id = mInputID.text;
            req.pw = mInputPW.text;

            WebReq.Instance.Request(req, delegate(ReqLogin.Res res)
            {
                if (res.IsSuccess)
                {
                    SignInComplete();
                    
                }
                else
                {
                    PopupManager.Instance.OpenPopupNotice(res.responseMessage+string.Format("\n에러코드 : {0}",res.statusCode));
                }
                
            });

/*
            if (PlayerPrefs.HasKey(Global.KEY_USER_ID) && PlayerPrefs.HasKey(Global.KEY_USER_PW))
            {
                if (mInputID.text != PlayerPrefs.GetString(Global.KEY_USER_ID))
                {
                    PopupManager.Instance.OpenPopupNotice("아이디를 다시 확인하세요.");
                    return;
                }
                if (mInputPW.text != PlayerPrefs.GetString(Global.KEY_USER_PW))
                {
                    PopupManager.Instance.OpenPopupNotice("비밀번호를 다시 확인하세요.");
                    return;
                }
                SignInComplete();

            }
            else
            {
                PopupManager.Instance.OpenPopupNotice("회원가입을 먼저 진행해주세요.");
            }*/


        });
        mBtnSignUp.onClick.AddListener(delegate
        {
            if (!mIsOnGoingSignUp)
            {
                if (string.IsNullOrEmpty(mInputID.text))
                {
                    PopupManager.Instance.OpenPopupNotice("아이디를 입력하세요.");
                    return;
                }
                if (string.IsNullOrEmpty(mInputPW.text))
                {
                    PopupManager.Instance.OpenPopupNotice("패스워드를 입력하세요.");
                    return;
                }
                
                if (PlayerPrefs.HasKey(Global.KEY_USER_ID) && PlayerPrefs.HasKey(Global.KEY_USER_PW))
                {
                    if (mInputID.text == PlayerPrefs.GetString(Global.KEY_USER_ID))
                    {
                        PopupManager.Instance.OpenPopupNotice("가입된 아이디 입니다.");
                        return;
                    }
                }
                
                SetSingUpUI();
            }
            else
            {
                if (string.IsNullOrEmpty(mInputID.text))
                {
                    PopupManager.Instance.OpenPopupNotice("아이디를 입력하세요.");
                    return;
                }
                if (string.IsNullOrEmpty(mInputPW.text))
                {
                    PopupManager.Instance.OpenPopupNotice("패스워드를 입력하세요.");
                    return;
                }

                if (string.IsNullOrEmpty(mInputPWCheck.text))
                {
                    PopupManager.Instance.OpenPopupNotice("패스워드를 확인하세요.");
                    return;
                }

                if (mInputPW.text != mInputPWCheck.text)
                {
                    PopupManager.Instance.OpenPopupNotice("패스워드가 일치하지 않습니다.");
                    return;
                }
                
                if (PlayerPrefs.HasKey(Global.KEY_USER_ID) && PlayerPrefs.HasKey(Global.KEY_USER_PW))
                {
                    if (mInputID.text == PlayerPrefs.GetString(Global.KEY_USER_ID))
                    {
                        PopupManager.Instance.OpenPopupNotice("가입된 아이디입니다.");
                        SetSingInUI();
                        return;
                    }
                }

                var req = new ReqSignUp();
                req.id = mInputID.text;
                req.pw = mInputPW.text;
                
                WebReq.Instance.Request(req, delegate(ReqSignUp.Res res)
                {
                    if(res.IsSuccess)
                        SignUpComplete();
                    else
                    {
                        PopupManager.Instance.OpenPopupNotice(res.responseMessage+string.Format("\n에러코드 : {0}",res.statusCode));
                    }
                
                });
            }
        });
        mBtnSetUID.onClick.AddListener(delegate
        {
            if (string.IsNullOrEmpty(mInputUID.text))
            {
                PopupManager.Instance.OpenPopupNotice("닉네임을 입력하세요.");
                return;
            }
            
            var req = new ReqCreateUserInfo();
            req.nickname = mInputUID.text;
                
            WebReq.Instance.Request(req, delegate(ReqCreateUserInfo.Res res)
            {
                if(res.IsSuccess)
                    SetNameComplete();
                else
                {
                    PopupManager.Instance.OpenPopupNotice(res.responseMessage+string.Format("\n에러코드 : {0}",res.statusCode));
                }
                
            });
            
            
        });
    }

    public void Init(Action del)
    {
        mDelLoginAfter = del;
    }

    public void Update()
    {
        if (mIsOnGoingSignUp)
        { 
            if (mInputPW.text != mInputPWCheck.text && mTextPWCheckNotice.text != "X")
            {
                mTextPWCheckNotice.text = "X";
                mTextPWCheckNotice.color = Color.red;
            }
            if (mInputPW.text == mInputPWCheck.text && mTextPWCheckNotice.text != "O")
            {
                mTextPWCheckNotice.text = "O";
                mTextPWCheckNotice.color = Color.green;
            }
            if(string.IsNullOrEmpty(mTextPWCheckNotice.text))
                mTextPWCheckNotice.text = "";
        }
    }

    public void SignInComplete()
    {
        Debug.Log("로그인 완료!");
        
        RequestUserInfo(delegate
        {
            if (string.IsNullOrEmpty(UserData.myData.user_name))
            {
                SetUIDUI();
            }
            else
            {
                mDelLoginAfter();
            }
        });
        
    }

    public void SetSingUpUI()
    {
        mIsOnGoingSignUp = true;
        var newPos = mRectID.position;
        mRectID.position = new Vector3(newPos.x, newPos.y + 36, newPos.z);
        newPos = mRectPW.position;
        mRectPW.position = new Vector3(newPos.x, newPos.y + 36, newPos.z);
        mGoSingUp.SetActive(true);
        mBtnSignIn.gameObject.SetActive(false);
        var rectBtn = mBtnSignUp.gameObject.GetComponent<RectTransform>();
        rectBtn.localPosition = new Vector3(0, 0, 0);
    }
    public void SetSingInUI()
    {
        mIsOnGoingSignUp = false;
        var newPos = mRectID.position;
        mRectID.position = new Vector3(newPos.x, newPos.y - 36, newPos.z);
        newPos = mRectPW.position;
        mRectPW.position = new Vector3(newPos.x, newPos.y - 36, newPos.z);
        mGoSingUp.SetActive(false);
        mBtnSignIn.gameObject.SetActive(true);
        var rectBtn = mBtnSignUp.gameObject.GetComponent<RectTransform>();
        rectBtn.localPosition = new Vector3(-90f, 0, 0);
    }

    public void SetUIDUI()
    {
        mGoSignUI.SetActive(false);
        mGoUIDUI.SetActive(true);
    }

    public void SignUpComplete()
    {
        PlayerPrefs.SetString(Global.KEY_USER_ID,UserData.myData.user_id);
        PlayerPrefs.SetString(Global.KEY_USER_PW,UserData.myData.user_pw);
        PopupManager.Instance.OpenPopupNotice("회원가입이 완료되었습니다.");
        Debug.Log("회원가입 완료! ID : "+PlayerPrefs.GetString(Global.KEY_USER_ID)+" PW : "+PlayerPrefs.GetString(Global.KEY_USER_PW));
       
        var req = new ReqLogin();
        req.id = UserData.myData.user_id;
        req.pw = UserData.myData.user_pw;

        WebReq.Instance.Request(req, delegate(ReqLogin.Res res)
        {
            if (res.IsSuccess)
            {
                RequestUserInfo(delegate
                {
                    if (string.IsNullOrEmpty(UserData.myData.user_name))
                    {
                        SetUIDUI();
                    }
                    else
                    {
                        mDelLoginAfter();
                    }
                });
            }

            else
            {
                PopupManager.Instance.OpenPopupNotice(res.responseMessage+string.Format("\n에러코드 : {0}",res.statusCode));
            }
        });

    }

    public void SetNameComplete()
    {
        PlayerPrefs.SetString(Global.KEY_USER_NAME,mInputUID.text);
        PopupManager.Instance.OpenPopupNotice("닉네임이 등록되었습니다.");
        mDelLoginAfter();
    }

    public void RefreshAfterSignOut()
    {
        mInputID.text = "";
        mInputPW.text = "";
        mGoSignUI.SetActive(true);
        mGoUIDUI.SetActive(false);
    }

    public void RequestUserInfo(Action del)
    {
                        
        WebReq.Instance.Request(new ReqUserInfo(), delegate(ReqUserInfo.Res res)
        {
            del();
        });
    }
    
    
    
}
