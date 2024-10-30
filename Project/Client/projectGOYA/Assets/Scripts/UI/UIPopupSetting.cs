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
    public CheckButton mInteractionBtn;
    public CheckButton mJoyStickBtn;
    public Slider mSliderCam;
    public TMPro.TMP_Text m_textSetCam;
    public Button mBtnHome;
    public RectTransform m_rectSound;
    public RectTransform m_rectUI;
    
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
        mSliderCam.value = GameData.myData.SET_CAM;
        mInteractionBtn.Init(GameData.myData.IS_SHOW_UI_BTN);
        mJoyStickBtn.Init(GameData.myData.IS_HIDE_JOYSTICK);
    }

    public void Init(Action del, string msg, string title = "")
    {
        del = delegate
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
            
            if (GameData.myData.IS_SHOW_UI_BTN != mInteractionBtn.IsChecked)
            {
                GameData.myData.SetUIBtnOn(mInteractionBtn.IsChecked);
            }
            
            if (GameData.myData.IS_HIDE_JOYSTICK != mJoyStickBtn.IsChecked)
            {
                GameData.myData.SetJoyStickOn(mJoyStickBtn.IsChecked);
            }
            
            if (!Equals(GameData.myData.SET_CAM, mSliderCam.value))
            {
                GameData.myData.SetCamSpeed(mSliderCam.value);
            }
            
        }+del;
        

        base.Init(del);

        if (string.IsNullOrEmpty(title))
            title = "설정";
        m_textTitle.text = title;
        m_textTitle.gameObject.SetActive(!string.IsNullOrEmpty(title));

        m_btnConfirm.onClick.AddListener(delegate
        {
            AudioManager.Instance.PlayClick();
            m_delClose();
            this.gameObject.SetActive(false);

        });
        
        mBtnHome.onClick.AddListener(delegate
        {
            AudioManager.Instance.PlayClick();
            AudioManager.Instance.StopBgm();
            m_delClose();
            GameManager.Instance.Scene.LoadScene(GameData.eScene.IntroScene);
        });

        mSliderVol.onValueChanged.AddListener(delegate { AudioManager.Instance.SetVolumeCheck(mSliderVol.value); });
        
        mSliderCam.onValueChanged.AddListener(delegate { m_textSetCam.text = mSliderCam.value.ToString();});
        

        bool isIntroScene = GameManager.Instance.Scene.currentScene.m_eSceneType == GameData.eScene.IntroScene;
        
        mBtnHome.gameObject.SetActive(!isIntroScene);

        if (isIntroScene)
        {
            m_rectSound.position = new Vector2(m_rectSound.position.x, m_rectSound.position.y-47f);
            m_rectUI.position = new Vector2(m_rectUI.position.x, m_rectUI.position.y-47f);
        }

    }

}