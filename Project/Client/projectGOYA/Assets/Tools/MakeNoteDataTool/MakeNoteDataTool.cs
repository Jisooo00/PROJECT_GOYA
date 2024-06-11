using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class MakeNoteDataTool : MonoBehaviour
{
    public AudioClip mAudioClip;
    public NoteData mNoteDataLeft;
    public NoteData mNoteDataRight;
    public TMP_Text mText;
    public TMP_Text mTextLeft;
    public TMP_Text mTextRight;
    private AudioSource mAudioSrc;
    private bool bStart;
    private int mISaveIdxLeft = 0;
    private int mISaveIdxRight = 0;
    private float mFTiming = 0f;

    
    void Start()
    {
        if (mAudioClip != null)
        {
            mAudioSrc = gameObject.AddComponent<AudioSource>();
            mAudioSrc.clip = mAudioClip;
        }

        bStart = false;
        mText.text = "Press Space-Key<br>To Make Note Data";

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(!bStart)
                StartCoroutine("StartMakeNote");
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (mAudioSrc.isPlaying)
            {
                if (mNoteDataLeft.noteDropTiming[mISaveIdxLeft] < 0)
                    mNoteDataLeft.noteDropTiming[mISaveIdxLeft] =  mFTiming;
                else
                {
                    mNoteDataLeft.noteDropTiming[mISaveIdxLeft] = (mNoteDataLeft.noteDropTiming[mISaveIdxLeft] + mFTiming)/2 ;
                }
                mTextLeft.text = String.Format("index : {0}\ntiming : {1}",mISaveIdxLeft,mFTiming.ToString());
                mISaveIdxLeft++;
            }
        }
        
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (mAudioSrc.isPlaying)
            {
                if (mNoteDataRight.noteDropTiming[mISaveIdxRight] < 0)
                    mNoteDataRight.noteDropTiming[mISaveIdxRight] = mFTiming;
                else
                {
                    mNoteDataRight.noteDropTiming[mISaveIdxRight] = (mNoteDataLeft.noteDropTiming[mISaveIdxRight] + mFTiming)/2 ;
                }

                mTextRight.text = String.Format("index : {0}\ntiming : {1}",mISaveIdxRight,mFTiming.ToString());
                mISaveIdxRight++;
            }
        }
        
    }

    private IEnumerator StartMakeNote()
    {
        bStart = true;
        float fStartTime = 5f;
        int iTimer = 5;
        while (fStartTime > 0f)
        {
            fStartTime -= Time.deltaTime;
            if ((int)fStartTime != iTimer)
            {
                iTimer = (int)fStartTime;
                mText.text = iTimer.ToString();
            }

            yield return null;
        }

        mText.text = "On Progress...";
        mFTiming = 0f;
        mAudioSrc.Play();

        while (mAudioSrc.isPlaying)
        {
            mFTiming += Time.deltaTime;
            yield return null;
        }

        mText.text = "Press Space-Key<br>To Make Note Data";
        mTextLeft.text = "LEFT";
        mTextLeft.text = "RIGHT";
        bStart = false;
    }
    
}
