using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    
    public float moveSpeed;
    public Rigidbody2D rb;
    
    private Vector2 movement;
    private Vector2 dest;
    
    private Vector3 dirVec;
    
    private bool bMoving = false;

    public bool IS_MOVING
    {
        get
        {
            return bMoving;
        }
    }

    
    void FixedUpdate()
    {
       
        if (rb != null && bMoving)
        {
            var dir = dest - rb.position;
            if (dir.magnitude <= 0.1f)
            {
                rb.position  = dest;
                bMoving = false;
                return;
            }
            else
            {
                rb.MovePosition(rb.position + dir.normalized * moveSpeed * Time.fixedDeltaTime);
                dirVec = CharacterAppearance.instance.GetDirection();
            }
            
        }
    }

    public void MoveTo(Vector2 destination)
    {
        bMoving = true;
        dest = destination;
    }


    
}