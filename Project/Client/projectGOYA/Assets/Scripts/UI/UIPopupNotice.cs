using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupNotice : UIPopup
{
    // Start is called before the first frame update
    public TMPro.TMP_Text m_textTitle;
    public Button m_btnConfirm;
    public Button m_btnClose;
    public TMPro.TMP_Text m_textConfirm;
    public TMPro.TMP_Text m_textNotice;

    void Start()
    {
    }

    public void Init(Action del, string msg, string title = "", bool bNoBtn = false)
    {
        base.Init(del);

        if (string.IsNullOrEmpty(title))
            title = "알림";
        m_textTitle.text = title;
        m_textTitle.gameObject.SetActive(!string.IsNullOrEmpty(title));


        m_textNotice.text = msg;

        m_textConfirm.text = "확인";
        
        m_btnConfirm.onClick.AddListener(delegate
        {
            m_delClose();
            this.gameObject.SetActive(false);

        });

        m_btnClose.gameObject.SetActive(bNoBtn);
        m_btnClose.onClick.AddListener(delegate
        {
            AudioManager.Instance.PlayClick();
            PopupManager.Instance.SetClosePopup();
        });
        
    }

    // Update is called once per frame
    void Update()
    {

    }

}

