using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupSetNickname : UIPopup
{
    public TMP_Text mTextNicknameLabel;
    public TMP_InputField mInputNickname;
    public Button mBtnConfirm;
    public TMP_Text mTextConfirm;
    public Button mBtnPrev;
    private bool isBusy = false;

    void Start()
    {
    }

    public void Init(Action del)
    {
        base.Init(del);
#if UNITY_WEBGL && !UNITY_EDITOR
        WebGLInput.captureAllKeyboardInput = false;
#endif
        mTextNicknameLabel.text = "도깨비의 이름을 정해주세요."; //TODO 로컬 적용
        mBtnConfirm.onClick.AddListener(delegate
        {
            if (isBusy)
                return;
            AudioManager.Instance.PlayClick();
            if (string.IsNullOrEmpty(mInputNickname.text))
            {
                //PopupManager.Instance.OpenPopupNotice("도깨비의 이름을 입력하세요."); //TODO 로컬 적용
                //return;
                mInputNickname.text = "돗가비";
            }
            StartCoroutine(RequestSetNickname());
        });
        
        mInputNickname.onValueChanged.AddListener(
            (word) => mInputNickname.text = Regex.Replace(word, @"[^0-9a-zA-Z가-힇ㄱ-ㅎㅏ-ㅣㆍᆞᆢ]", "")
        );
        mBtnPrev.onClick.AddListener(delegate
        {
            if (isBusy)
                return;
            AudioManager.Instance.PlayClick();
            PopupManager.Instance.SetClosePopup();
            gameObject.SetActive(false);
        });
        
        mTextConfirm.text = "확인"; //TODO 로컬 적용
        
    }

    IEnumerator RequestSetNickname()
    {
        isBusy = true;
        yield return null;

        bool bSetNicknameSuccess = false;
        
        var req = new ReqCreateUserInfo();
        req.nickname = mInputNickname.text;

        WebReq.Instance.Request(req, delegate(ReqCreateUserInfo.Res res)
        {
            if (res.IsSuccess)
            {
                bSetNicknameSuccess = true;
            }
            else
            {
                PopupManager.Instance.OpenPopupNotice(res.responseMessage );
            }
            isBusy = false;
        });
            
        while (isBusy)
        {
            yield return null;
        }

        if (bSetNicknameSuccess)
        {
            OnClose();
        }
    }

}