using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Button mBtnDialog;
    [NonSerialized] public bool mIsDialogEnable = false;

    [SerializeField] private GameObject mLoginUI;
    [SerializeField] private GameObject mActionUI;
    [SerializeField] private GameObject mDialogUI;
    public Button mBtnSetting;

    //public SanyeahGame mSanyeahGame;
    private UIDialog mDialogSystem;

    void Start()
    {
        Init();
    }

    public void Init()
    {
        mActionUI.SetActive(true);
        mDialogUI.SetActive(false);
        SetDialogEnable(false);
        if(mDialogUI!= null)
            mDialogSystem = mDialogUI.GetComponent<UIDialog>();
        mBtnSetting.onClick.AddListener(delegate
        {
            AudioManager.Instance.PlayClick();
            PopupManager.Instance.OpenPopupSetting();
        });
    }

    
    // Update is called once per frame
    void Update()
    {
        var data = Player.instance.GetScannedMonster();
        if (data != null && !mIsDialogEnable)
        {
            SetDialogEnable(true);
        }
        else if ((data == null) && mIsDialogEnable)
        {
            SetDialogEnable(false);
        }
    }
    public void DialogKeyOnClick()
    {
        AudioManager.Instance.PlayClick();
        Player.instance.bIsDialogPlaying = true;
        PlayDialog();
    }

    public void SetDialogEnable(bool bEnable)
    {
        mIsDialogEnable = bEnable;
        if(mBtnDialog != null)
            mBtnDialog.gameObject.SetActive(mIsDialogEnable);
    }

    public void PlayDialog()
    {
        mActionUI.SetActive(false);
        mDialogUI.SetActive(true);
        if (mDialogSystem != null)
        {
            var monster = Player.instance.GetScannedMonster();
            if (Player.instance.GetScannedMonster() != null)
            {
                mDialogSystem.Init(monster, delegate { EndDialog(); });

            }

        }
    }

    public void EndDialog()
    {
        mActionUI.SetActive(true);
        mDialogUI.SetActive(false);
        Player.instance.bIsDialogPlaying = false;
        
        /*
        if (GameManager.Instance.Scene.currentScene.name == "@SanyeahScene" && data.mIndex == 1)
         
        {
            mSanyeahGame.gameObject.SetActive(true);
            mSanyeahGame.mDelAfterGame = delegate
            {
                Player.instance.mScanObject.GetComponent<MonsterBase>().RefreshData();
            };

        }*/

    }

}

