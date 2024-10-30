using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class UIPopupSignUp : UIPopup
{
    public TMP_Text mTextIDLabel;
    public TMP_Text mTextPWLabel;    
    public TMP_Text mTextPWCheckLabel;
    public TMP_Text mTextPWCheckNotice;
    public TMP_InputField mInputID;
    public TMP_InputField mInputPW;
    public TMP_InputField mInputPWCheck;
    public Button mBtnSignIn;
    public Button mBtnPrev;
    public TMP_Text mTextBtnSingUp;
    private bool isBusy = false;

    void Start()
    {
    }

    private void Update()
    {
        if (string.IsNullOrEmpty(mInputPW.text))
        {
            if(!string.IsNullOrEmpty(mTextPWCheckNotice.text))
                mTextPWCheckNotice.text = "";
            return;
        }
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
    }

    public void Init(Action del)
    {
        base.Init(del);
#if UNITY_WEBGL
        WebGLInput.captureAllKeyboardInput = false;
#endif
        mTextIDLabel.text = "ID"; //TODO 로컬 적용
        mTextPWLabel.text = "PW"; //TODO 로컬 적용
        mTextPWCheckLabel.text = "PW'"; //TODO 로컬 적용
        
        mBtnPrev.onClick.AddListener(delegate
        {
            if (isBusy)
                return;
            AudioManager.Instance.PlayClick();
            isBusy = true;
            m_delClose += delegate
            {
                PopupManager.Instance.OpenPopupAccount();
            };
            OnClose();
        });
        
        mTextBtnSingUp.text = "회원가입"; //TODO 로컬 적용
        mBtnSignIn.onClick.AddListener(delegate
        {
            if (isBusy)
                return;
            
            AudioManager.Instance.PlayClick();
            
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

            StartCoroutine(RequestSignUp());
        });
        
        mInputID.onValueChanged.AddListener(
            (word) => mInputID.text = Regex.Replace(word, @"[^0-9a-zA-Z]", "")
        );
        mInputPW.onValueChanged.AddListener(
            (word) => mInputPW.text = Regex.Replace(word, @"[^0-9a-zA-Z!@#$%^&*]", "")
        );
        mInputPWCheck.onValueChanged.AddListener(
            (word) => mInputPWCheck.text = Regex.Replace(word, @"[^0-9a-zA-Z!@#$%^&*]", "")
        );
        
    }
    

    IEnumerator RequestSignUp()
    {
        isBusy = true;
        yield return null;

        bool bSignInSuccess = false;
        
        var req = new ReqSignUp();
        req.id = mInputID.text;
        req.pw = mInputPW.text;

        WebReq.Instance.Request(req, delegate(ReqSignUp.Res res)
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
                        bSignInSuccess = true;
                    }
                    else
                    {
                        PopupManager.Instance.OpenPopupNotice(res2.responseMessage );
                    }
                });
            }
            else
            {
                PopupManager.Instance.OpenPopupNotice(res.responseMessage );
                isBusy = false;
            }
        });
            
        while (isBusy)
        {
            yield return null;
        }

        if (bSignInSuccess)
        {
            OnClose();
        }
    }

}