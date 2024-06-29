using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class MonsterBase : MonoBehaviour{
    
    public GameObject mQuestionMark;
    private bool mIsInteractable = false;
    public string monsterID;
    public Animator animator;
    public List<GameData.DialogData> mData;

    public GameData.DialogData CurDialog
    {
        get
        {
            if (mData != null)
            {
                foreach (var dialog in mData)
                {
                    if (!dialog.m_bPlayed)
                    {
                        return dialog;
                    }
                }
            }
            return null;
        }
        
    }
    private void Awake()
    {
        InitMonster();
        SetQuestionMark();
        
    }

    private void Update()
    {
        //if (mData==null || mData.bIsPlayed)
        //    return;
        if (Player.instance.mScanObject != null && Player.instance.mScanObject.name == this.name && !mIsInteractable)
        {
            mIsInteractable = true;
        }else if (Player.instance.mScanObject == null || Player.instance.mScanObject.name != this.name && mIsInteractable)
        {
            mIsInteractable = false;
        }
        SetQuestionMark();
    }

    private void SetQuestionMark()
    {
        if(mQuestionMark!=null)
            mQuestionMark.SetActive(mIsInteractable);
    }

    protected virtual void InitMonster()
    {
    }
    

    public void RefreshData()
    {
        mIsInteractable = false;
        SetQuestionMark();
        //mData = GameData.GetDialog(monsterID);
    }
}