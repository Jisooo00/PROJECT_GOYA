using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupSignIn : UIPopup
{
    public TMP_Text mTextIDLabel;
    public TMP_Text mTextPWLabel;
    public TMP_InputField mInputID;
    public TMP_InputField mInputPW;
    public Button mBtnSignIn;
    public Button mBtnPrev;
    public TMP_Text mTextBtnSingIn;
    private bool isBusy = false;

    void Start()
    {
    }

    public void Init(Action del)
    {
        base.Init(del);
        
        mTextIDLabel.text = "ID"; //TODO 로컬 적용
        mTextPWLabel.text = "PW"; //TODO 로컬 적용
        
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
        
        mTextBtnSingIn.text = "로그인"; //TODO 로컬 적용
        mBtnSignIn.onClick.AddListener(delegate
        {
            if (isBusy)
                return;
            
            AudioManager.Instance.PlayClick();
            if (string.IsNullOrEmpty(mInputID.text))
            {
                PopupManager.Instance.OpenPopupNotice("아이디를 입력하세요."); //TODO 로컬 적용
                return;
            }
            if (string.IsNullOrEmpty(mInputPW.text))
            {
                PopupManager.Instance.OpenPopupNotice("패스워드를 입력하세요."); //TODO 로컬 적용
                return;
            }

            StartCoroutine(RequestLogin());
        });
        
    }
    

    IEnumerator RequestLogin()
    {
        isBusy = true;
        yield return null;

        bool bLoginSuccess = false;
        
        var req = new ReqLogin();
        req.id = mInputID.text;
        req.pw = mInputPW.text;

        WebReq.Instance.Request(req, delegate(ReqLogin.Res res)
        {
            isBusy = false;
            if (res.IsSuccess)
            {
                bLoginSuccess = true;
            }
            else
            {
                PopupManager.Instance.OpenPopupNotice(res.responseMessage);
            }
        });
            
        while (isBusy)
        {
            yield return null;
        }

        if (bLoginSuccess)
        {
            OnClose();
        }
    }

}