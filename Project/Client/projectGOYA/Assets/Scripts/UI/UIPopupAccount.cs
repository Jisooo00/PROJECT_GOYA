using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupAccount : UIPopup
{
    public Button m_btnGuest;
    public TMPro.TMP_Text m_textGuest;
    public Button m_btnSignUp;
    public TMPro.TMP_Text m_textSignUp;
    public Button m_btnSignIn;
    public TMPro.TMP_Text m_textSignIn;
    public TMPro.TMP_Text m_textNotice;

    private bool isBusy = false;

    void Start()
    {
    }

    public void Init(Action del)
    {
        base.Init(del);
        
        m_textNotice.text = "이미 계정을 가지고 계시다면?"; //TODO 로컬 적용

        m_textGuest.text = "게스트 모드"; //TODO 로컬 적용
        m_btnGuest.onClick.AddListener(delegate
        {
            if (isBusy)
                return;
            StartCoroutine(RequestGuestAfter());
        });
        
        m_textSignUp.text = "회원 가입"; //TODO 로컬 적용
        m_btnSignUp.onClick.AddListener(delegate
        {
            if (isBusy)
                return;
            
            m_delClose += delegate
            {
                PopupManager.Instance.OpenPopupSignUp(del);
            };
            
            OnClose();
        });
        
        m_textSignIn.text = "로그인"; //TODO 로컬 적용
        m_btnSignIn.onClick.AddListener(delegate
        {
            if (isBusy)
                return;
            
            m_delClose += delegate
            {
                PopupManager.Instance.OpenPopupSignIn(del);
            };
            
            OnClose();
        });
    }
    
    IEnumerator RequestGuestAfter()
    {
        isBusy = true;
        yield return null;

        bool bLoginSuccess = false;
        
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
                        bLoginSuccess = true;
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
                
                isBusy = false;
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