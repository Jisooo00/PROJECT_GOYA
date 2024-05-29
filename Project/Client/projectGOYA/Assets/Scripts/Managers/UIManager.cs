using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Button mBtnDialog;
    [NonSerialized] public bool mIsDialogEnable = false;

    [SerializeField] private GameObject mButtonUI;
    [SerializeField] private GameObject mDialogUI;

    public SanyeahGame mSanyeahGame;

    private UIDialog mDialogSystem;
    void Start()
    {
        SetDialogEnable(false);
        if(mDialogUI!= null)
            mDialogSystem = mDialogUI.GetComponent<UIDialog>();
    }

    // Update is called once per frame
    void Update()
    {
        var data = Player.instance.GetScannedMonster();
        if (data != null&& !data.bIsPlayed && !mIsDialogEnable)
        {
            SetDialogEnable(true);
        }else if ((data == null || data.bIsPlayed) && mIsDialogEnable)
        {
            SetDialogEnable(false);
        }
    }

    public void UpKeyDown()
    {
        Player.instance.bInputUp = true;
    }
    public void UpKeyUp()
    {
        Player.instance.bInputUp = false;
    }
    public void DownKeyDown()
    {
        Player.instance.bInputDown = true;
    }
    public void DownKeyUp()
    {
        Player.instance.bInputDown = false;
    }
    public void LeftKeyDown()
    {
        Player.instance.bInputLeft = true;
    }
    public void LeftKeyUp()
    {
        Player.instance.bInputLeft = false;
    }
    public void RightKeyDown()
    {
        Player.instance.bInputRight = true;
    }
    public void RightKeyUp()
    {
        Player.instance.bInputRight = false;
    }

    public void DialogKeyOnClick()
    {
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
        mButtonUI.SetActive(false);
        mDialogUI.SetActive(true);
        if (mDialogSystem != null)
        {
            var monster = Player.instance.GetScannedMonster();
            if (Player.instance.GetScannedMonster() != null)
            {
                mDialogSystem.Init(monster, delegate
                {
                    if (Player.instance.GetScannedMonster() != null)
                    {
                        GameData.SetDialogEnd(Player.instance.GetScannedMonster().mObjectID,Player.instance.GetScannedMonster().mIndex-1);
                        EndDialog(Player.instance.GetScannedMonster());
                    }

                });
            }

        }
    }

    public void EndDialog(GameData.DialogData data = null)
    {
        mButtonUI.SetActive(true);
        mDialogUI.SetActive(false);
        Player.instance.bIsDialogPlaying = false;
        if (data != null)
        {
            GameData.SetDialogEnd(data.mObjectID,data.mIndex-1);
            if(data.mMoveScene >= 0 )
                GameManager.Instance.Scene.LoadScene((GameData.eScene) data.mMoveScene);
        }
        
        if (GameManager.Instance.Scene.currentScene.name == "@SanyeahScene" && data.mIndex == 1)
        {
            mSanyeahGame.gameObject.SetActive(true);
            mSanyeahGame.mDelAfterGame = delegate
            {
                Player.instance.mScanObject.GetComponent<MonsterBase>().RefreshData();
            };

        }

    }

}

