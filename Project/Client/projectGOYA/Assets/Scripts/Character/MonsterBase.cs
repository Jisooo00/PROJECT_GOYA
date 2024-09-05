using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public abstract class MonsterBase : MonoBehaviour{
    
    public SpriteRenderer mQuestionMark;
    private bool mIsInteractable = false;
    public string monsterID;
    public Animator animator;
    public List<GameData.DialogData> mData;
    public List<Sprite> mListSprites;

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
        //if (Player.instance.mScanObject != null && Player.instance.mScanObject.name == this.name && !mIsInteractable)
        //{
        //    mIsInteractable = true;
        //}else if (Player.instance.mScanObject == null || Player.instance.mScanObject.name != this.name && mIsInteractable)
        //{
        //    mIsInteractable = false;
        //}
        //SetQuestionMark();

    }

    public void SetQuestionMark()
    {
        if (mQuestionMark == null)
            return;
        
        if (CurDialog!=null)
        {
            mQuestionMark.gameObject.SetActive(true);
            mQuestionMark.sprite = CurDialog.m_bReplay ? mListSprites[0] : mListSprites[1];
        }
        else
        {
            mQuestionMark.gameObject.SetActive(false);
        }
        
    }

    protected virtual void InitMonster()
    {
    }

    public void RefreshData()
    {
        SetQuestionMark();
        //mData = GameData.GetDialog(monsterID);
    }
    
    public float moveSpeed;
    public Rigidbody2D rb;
    
    private Vector2 movement;
    private Vector2 dest;
    
    private Vector3 dirVec;
    
    private bool bMoving = false;
    private float footstep = 0f;

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
                SetQuestionMark();
                return;
            }
            else
            {
                rb.MovePosition(rb.position + dir.normalized * moveSpeed * Time.fixedDeltaTime);
                dirVec = CharacterAppearance.instance.GetDirection();
                if(mQuestionMark.gameObject.activeSelf)
                    mQuestionMark.gameObject.SetActive(false);
            }
            
            footstep += Time.deltaTime;
            if (footstep > 0.5f)
            {
                AudioManager.Instance.PlayFootstep();
                footstep = 0;
            }
            
        }
    }

    public void MoveTo(Vector2 destination)
    {
        bMoving = true;
        dest = destination;
    }


    
}