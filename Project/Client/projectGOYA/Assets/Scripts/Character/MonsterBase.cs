using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class MonsterBase : MonoBehaviour{
    
    public GameObject mQuestionMark;
    private bool mIsInteractable = false;
    public string monsterID;
    public GameData.DialogData mData;
    private void Awake()
    {
        InitMonster();
        SetQuestionMark();
        
    }

    private void Update()
    {
        if (mData==null || mData.bIsPlayed)
            return;
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

    public void SetDialogEnd()
    {
        mData = GameData.SetDialogEnd(monsterID,mData.mIndex-1);
    }

    public void RefreshData()
    {
        mIsInteractable = false;
        SetQuestionMark();
        mData = GameData.GetDialog(monsterID);
    }
}