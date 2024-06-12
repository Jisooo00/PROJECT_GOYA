using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class SanyeahGameManager : BaseScene
{
    [Header("Game Management")]
    // 양쪽 Note Object가 개별적으로 관리됨 (Left/Right).
    [SerializeField] private NoteManager mNoteMgrLeft;
    [SerializeField] private NoteManager mNoteMgrRight;
    [SerializeField] private GameObject mGoEffectL;
    [SerializeField] private GameObject mGoEffectR;
    [SerializeField] private TMP_Text mTextLeft;
    [SerializeField] private TMP_Text mTextRight;
    [SerializeField] private AudioClip mAudioClip;
    [SerializeField] private Animator mAniSanyeahRhythm;
    // Tool 이용해 NoteData 제작
    [SerializeField] private NoteData mNoteDataLeft;
    [SerializeField] private NoteData mNoteDataRight;
    
    [Header("InGame UI")]
    [SerializeField] private TMP_Text mTextStartTimer;
    [SerializeField] private GameObject mGoBeforeStart;
    [SerializeField] private GameObject mGoGameResult;
    [SerializeField] private TMP_Text mTextScore;
    [SerializeField] private Button mBtnR;
    [SerializeField] private Button mBtnL;
    
    private AudioSource mAudioSrc;
    
    //배경음과의 Sync를 위해 게임 시작 시점과 Note활성화 시점 분리
    private bool bStart = false;
    private bool bDropNote = false;

    private int mITotalScore = 0;
    
    public enum eScore
    {
        PERFECT = 100,
        GREAT = 80,
        GOOD = 60,
        BAD = 20,
        MISS = 0,
        
    }
    
    protected override void InitScene()
    {
        base.InitScene();
        m_eSceneType = GameData.eScene.IntroScene;

        //m_uiManager.Init(m_eSceneType);
    }

    private void Start()
    {
        mGoEffectR.SetActive(false);
        mGoEffectL.SetActive(false);
        mGoBeforeStart.SetActive(true);
        mGoGameResult.SetActive(false);
        mTextScore.text = string.Format("Score : {0:#,###}", 0);
        TextEffectDisableRight();
        TextEffectDisableLeft();
        
        if (mAudioClip != null)
        {
            mAudioSrc = gameObject.AddComponent<AudioSource>();
            mAudioSrc.clip = mAudioClip;
        }

        StartCoroutine("StartGame");
    }

    private void Update()
    {
        if (!bStart)
            return;
        
        if (bStart && !mAudioSrc.isPlaying)
        {
            mGoGameResult.SetActive(true);
            bStart = false;
            GameManager.Instance.bClearSanyeah = true;
            Invoke("GoToSanyeahScene",2f);
        }
        
#if UNITY_EDITOR
        if(Input.GetKeyDown(KeyCode.A))
        {
            JudgeLeft();
        }
        if(Input.GetKeyDown(KeyCode.D))
        {
            JudgeRight();
        }
#endif
    }

    private IEnumerator StartGame()
    {
        if(bStart)
            yield break;
        
        float fStartTime = 5f;
        int iTimer = 5;
        
        while (fStartTime > 0f)
        {
            fStartTime -= Time.deltaTime;
            if ((int) fStartTime != iTimer)
            {
                iTimer = (int)fStartTime;
                mTextStartTimer.text = iTimer.ToString();
            }

            if (fStartTime < 1f && !bDropNote)
                StartCoroutine("DropNoteData");
            
            yield return null;
        }
        mAudioSrc.Play();
        bStart = true;
        mGoBeforeStart.SetActive(false);
        
    }

    private IEnumerator DropNoteData()
    {
        bDropNote = true;
        int indexLeft = 0;
        int indexRight = 0;
        float currentTime = 0f;

        bool bLeftDone = false;
        bool bRightDone = false;
        
        while (true)
        {
            if (!bLeftDone && currentTime >= mNoteDataLeft.noteDropTiming[indexLeft])
            {
                indexLeft++;

                if (mNoteDataLeft.noteDropTiming.Count > indexLeft)
                {
                    mNoteMgrLeft.DropNoteObject();
                }
                else
                {
                    bLeftDone = true;
                }
            }
            
            if (!bRightDone && currentTime >= mNoteDataRight.noteDropTiming[indexRight])
            {
                indexRight++;

                if (mNoteDataRight.noteDropTiming.Count > indexRight)
                {
                    mNoteMgrRight.DropNoteObject();
                }
                else
                {
                    bRightDone = true;
                }
            }
            
            if(bLeftDone && bRightDone)
                yield break;
            
            currentTime += Time.deltaTime;
            yield return null;
        }
    }

    public void JudgeLeft()
    {
        float result = mNoteMgrLeft.GetJudgeDistance();
        if (result > Global.SANYEAH_NOTE_JUDGE_MISS || result <0)
            return;
        
        PlayRhythmAniLeft(GetScore(result));
    }

    public void JudgeRight()
    {
        float result = mNoteMgrRight.GetJudgeDistance();
        if (result > Global.SANYEAH_NOTE_JUDGE_MISS || result < 0)
            return;
        
        PlayRhythmAniRight(GetScore(result));
    }

    public void PlayRhythmAniLeft(eScore score)
    {
        if (score == eScore.MISS)
            return;
        
        mGoEffectL.SetActive(false);
        if(score != eScore.BAD)
            mGoEffectL.SetActive(true);
        
        TextEffectEnable(mTextLeft,score,"TextEffectDisableLeft");
        
        mAniSanyeahRhythm.SetTrigger("RhythmLeft");
        
    }
    
    public void PlayRhythmAniRight(eScore score)
    {
        TextEffectEnable(mTextRight,score,"TextEffectDisableRight");
        if (score == eScore.MISS)
            return;
        
        mGoEffectR.SetActive(false);
        if(score != eScore.BAD)
            mGoEffectR.SetActive(true);

        mAniSanyeahRhythm.SetTrigger("RhythmRight");
    }

    public eScore GetScore(float result)
    {
        eScore score = eScore.PERFECT;

        if (result < Global.SANYEAH_NOTE_JUDGE_PERFECT)
        {
            score = eScore.PERFECT;
        }
        else if (result < Global.SANYEAH_NOTE_JUDGE_GREAT)
        {
            score = eScore.GREAT;
        }
        else if (result < Global.SANYEAH_NOTE_JUDGE_GOOD)
        {
            score = eScore.GOOD;
        }
        else if (result < Global.SANYEAH_NOTE_JUDGE_BAD)
        {
            score = eScore.BAD;
        }
        else
        {
            score = eScore.MISS;
        }
        
        mITotalScore += (int) score;
        //mImgGaugeValue.fillAmount = (float) mITotalScore / MAX_SCORE;
        mTextScore.text = string.Format("Score : {0:#,###}", mITotalScore);

        return score;
    }

    void TextEffectEnable(TMP_Text text, eScore score,string strInvokeMethod)
    {
        CancelInvoke(strInvokeMethod);
        text.text = string.Format("{0}!",score.ToString());
        
        switch (score)
        {
            case eScore.PERFECT:
                text.color = Color.magenta;
                break;
            case eScore.GREAT:
                text.color = Color.yellow;
                break;
            case eScore.GOOD:
                text.color = Color.green;
                break;
            case eScore.BAD:
                text.color = Color.red;
                break;
            case eScore.MISS:
                text.color = Color.gray;
                break;
        }
        Invoke(strInvokeMethod,1f);
    }
    
    void TextEffectDisableLeft()
    {
        mTextLeft.text = "";
    }
    
    void TextEffectDisableRight()
    {
        mTextRight.text = "";
    }

    void GoToSanyeahScene()
    {
        GameManager.Instance.Scene.LoadScene(GameData.eScene.SanyeahScene);
    }

    public override void Clear(Action del)
    {
        Debug.Log("SanyeahGameScene is clear.");
        del();
    }
}