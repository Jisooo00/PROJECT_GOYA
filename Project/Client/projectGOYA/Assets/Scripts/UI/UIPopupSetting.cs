using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupSetting : UIPopup
{
    // Start is called before the first frame update
    public TMPro.TMP_Text m_textTitle;
    public Button m_btnConfirm;
    public CheckButton mBgmBtn;
    public CheckButton mEffectBtn;
    public Slider mSliderVol;
    
    [Serializable]
    public class CheckButton
    {
        public Button m_btn;
        public bool IsChecked;
        public GameObject mGOChecked;

        public void Init(bool isChecked)
        {
            IsChecked = isChecked;
            SetChecked();
            m_btn.onClick.AddListener(delegate
            {
                IsChecked = !IsChecked;
                SetChecked();
            });
        }

        public void SetChecked()
        {
            mGOChecked.SetActive(IsChecked);
        }

    }

    void Start()
    {
        mBgmBtn.Init(GameData.myData.IS_BGM_ON);
        mEffectBtn.Init(GameData.myData.IS_EFFECT_ON);
        mSliderVol.value = GameData.myData.SET_VOLUME;
    }

    public void Init(Action del, string msg, string title = "")
    {
        del += delegate
        {
            if (GameData.myData.IS_BGM_ON != mBgmBtn.IsChecked)
            {
                GameData.myData.SetBgmOn(mBgmBtn.IsChecked);
                if (!mBgmBtn.IsChecked)
                {
                    AudioManager.Instance.StopBgm();
                }
                else
                {
                    AudioManager.Instance.PlayBgm();
                }
            }

            if (GameData.myData.IS_EFFECT_ON != mEffectBtn.IsChecked)
            {
                GameData.myData.SetEffectOn(mEffectBtn.IsChecked);
            }

            if (!Equals(GameData.myData.SET_VOLUME, mSliderVol.value))
            {
                GameData.myData.SetVolume(mSliderVol.value);
                AudioManager.Instance.SetVolume();
            }
        };

        base.Init(del);

        if (string.IsNullOrEmpty(title))
            title = "설정";
        m_textTitle.text = title;
        m_textTitle.gameObject.SetActive(!string.IsNullOrEmpty(title));

        m_btnConfirm.onClick.AddListener(delegate
        {
            m_delClose();
            this.gameObject.SetActive(false);

        });

        mSliderVol.onValueChanged.AddListener(delegate { AudioManager.Instance.SetVolumeCheck(mSliderVol.value); });

    }

    // Update is called once per frame
    void Update()
    {

    }

}