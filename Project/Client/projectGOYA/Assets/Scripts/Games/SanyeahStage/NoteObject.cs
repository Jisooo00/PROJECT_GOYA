using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NoteObject : MonoBehaviour
{
    
    [SerializeField] private RectTransform mRectTransform;
    [SerializeField] private Image mImg = null;
    private RectTransform mRTEndPoint;
    private RectTransform mRTStartPoint;
    private Action mDelInitAfter = null;
    private Action mDelShowAfter = null;
    private Action mDelJudgeMiss = null;
    private Color mClrInvisible = new Color(1, 1, 1, 0);
    private Color mClrVisible = new Color(1, 1, 1, 1);
    
    private bool bShow = false;
    private bool bVisible = false;

    public bool IsShow
    {
        get { return bShow;}
    }

    public float JudgePosX
    {
        get
        {
            float x = Mathf.Abs(mRectTransform.position.x-mRTEndPoint.position.x);
            if(x <= Global.SANYEAH_NOTE_JUDGE_MISS)
                SetShow(false);
            return x;
        }
    }
    public enum eSide
    {
        LEFT = 1,
        RIGHT = -1,
    }

    private eSide mESide;
    private Vector2 mV2Dir;

    void Update()
    {
        if (!bShow)
            return;
        
        mRectTransform.anchoredPosition += mV2Dir * Global.SANYEAH_NOTE_DROP_SPEED * Time.deltaTime;
        float posX = mRectTransform.position.x;
        if (mRTEndPoint != null)
        {
            if (((mESide == eSide.LEFT && posX >= mRTEndPoint.position.x + 75f) ||
                (mESide == eSide.RIGHT && posX <= mRTEndPoint.position.x - 75f)) && bVisible)
            {
                SetInvisible();
            }
            if ((mESide == eSide.LEFT && posX >= mRTEndPoint.position.x + (Global.SANYEAH_NOTE_JUDGE_MISS)) ||
                (mESide == eSide.RIGHT && posX <= mRTEndPoint.position.x - (Global.SANYEAH_NOTE_JUDGE_MISS)))
            {
                SetShow(false);
                if (mDelJudgeMiss != null)
                    mDelJudgeMiss();
            }
        }
        
    }

    public void Init(eSide eSide, RectTransform rtEnd, RectTransform rtStart, Action delInit = null, Action delShow = null,Action delMiss = null)
    {
        if (delInit != null)
            mDelInitAfter = delInit;

        if (delShow != null)
            mDelShowAfter = delShow;

        if (delMiss != null)
            mDelJudgeMiss = delMiss;

        mESide = eSide;
        mRTEndPoint = rtEnd;
        mRTStartPoint = rtStart;
        mRectTransform.anchorMin = mRTStartPoint.anchorMin;
        mRectTransform.anchorMax = mRTStartPoint.anchorMax;
        
        mV2Dir = new Vector2((int) mESide, 0f);
        mRectTransform.anchoredPosition = mRTStartPoint.anchoredPosition;
        bShow = false;
        gameObject.SetActive(bShow);

    }

    public void SetInvisible()
    {
        mImg.color = mClrInvisible;
        bVisible = true;
    }

    public void SetShow(bool _bShow)
    {
        bShow = _bShow;
        mRectTransform.anchoredPosition = mRTStartPoint.anchoredPosition;
        gameObject.SetActive(_bShow);
        if (bShow && mDelShowAfter != null)
        {
            bVisible = true;
            mImg.color = mClrVisible;
            mDelShowAfter();
        }

        if (!bShow && mDelInitAfter!= null)
        {
            mDelInitAfter();
        }
    }

}
