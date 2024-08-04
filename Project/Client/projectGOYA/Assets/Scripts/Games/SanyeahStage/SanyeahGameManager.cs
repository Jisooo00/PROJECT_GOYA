using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
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
    [SerializeField] private AudioClip mEffectClip;
    [SerializeField] private AudioClip mEffectMissClip;
    [SerializeField] private Animator mAniSanyeahRhythm;
    // Tool 이용해 NoteData 제작
    [SerializeField] private NoteData mNoteDataLeft;
    [SerializeField] private NoteData mNoteDataRight;
    
    [Header("InGame UI")]
    [SerializeField] private TMP_Text mTextStartTimer;
    [SerializeField] private GameObject mGoBeforeStart;
    [SerializeField] private GameObject mGoGameResult;
    [SerializeField] private TMP_Text mTextResult;
    [SerializeField] private TMP_Text mTextScore;
    [SerializeField] private Button mBtnR;
    [SerializeField] private Button mBtnL;
    [SerializeField] private Image mImgGaugeValue;
    
    private AudioSource mAudioSrc;
    private AudioSource mEffectSrc;
    private AudioSource mEffectMissSrc;

    private float mFDropLeftLastTime = 0f;
    private float mFDropRightLastTime = 0f;
    private bool mIsAllNoteDrop = false;
    
    //배경음과의 Sync를 위해 게임 시작 시점과 Note활성화 시점 분리
    private bool bStart = false;
    private bool bDropNote = false;

    private int mITotalScore = 0;
    
    public enum eScore
    {
        PERFECT = 100,
        GREAT = 75,
        GOOD = 50,
        BAD = -25,
        MISS = -50,
        
    }
    
    protected override void InitScene()
    {
        base.InitScene();
        m_eSceneType = GameData.eScene.IntroScene;
        AudioManager.Instance.StopBgm();

        //m_uiManager.Init(m_eSceneType);
    }
    
    public override void DelFunc()
    {
        
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
        
        if (mEffectClip != null)
        {
            mEffectSrc = gameObject.AddComponent<AudioSource>();
            mEffectSrc.clip = mEffectClip;
        }
        if (mEffectMissClip != null)
        {
            mEffectMissSrc = gameObject.AddComponent<AudioSource>();
            mEffectMissSrc.clip = mEffectMissClip;
        }
        
        mNoteMgrLeft.SetDelJudgeMiss(delegate
        {
            mITotalScore += (int) eScore.MISS;
            mImgGaugeValue.fillAmount += (int) eScore.MISS / 1000f;
            mTextScore.text = string.Format("Score : {0:#,###}", mITotalScore);
            mEffectMissSrc.Play();
            PlayRhythmAniLeft(eScore.MISS);
        });
        mNoteMgrRight.SetDelJudgeMiss(delegate
        {
            mITotalScore += (int) eScore.MISS;
            mImgGaugeValue.fillAmount += (int) eScore.MISS / 1000f;
            mTextScore.text = string.Format("Score : {0:#,###}", mITotalScore);
            mEffectMissSrc.Play();
            PlayRhythmAniRight(eScore.MISS);
        });

        StartCoroutine("StartGame");
    }

    private void Update()
    {

#if UNITY_EDITOR

        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayerPrefs.SetString(string.Format("{0}_{1}", GameData.myData.user_uid, "Dl_0006"), "true");
            PlayerPrefs.Save();
            GameData.SetDialogPlayed("np_0002","Dl_0006");
            mITotalScore = 5000;
            SetGameClear();
        }
        
#endif
        
        if (!bStart)
            return;
        
        if(mImgGaugeValue.fillAmount == 0)
            SetGameOver();
        
        if (bStart && !mAudioSrc.isPlaying)
        {
            if(mITotalScore >= 5000)
                SetGameClear();
            else
            {
                SetGameOver();
            }
        }

        if (!mIsAllNoteDrop)
        {
            bool bDelay = mFDropRightLastTime > 1f || mFDropLeftLastTime > 1f;
            mImgGaugeValue.fillAmount -= Time.deltaTime / (bDelay ? 20f : 10f);

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

    private void SetGameOver()
    {
        mGoGameResult.SetActive(true);
        mTextResult.text = "Game Over";
        bStart = false;
        if(!mIsAllNoteDrop)
            StopCoroutine("DropNoteData");
        mAudioSrc.Stop();
        GameManager.Instance.bClearSanyeah = false;
        Invoke("GoToSanyeahScene",2f);
    }
    
    private void SetGameClear()
    {
        mGoGameResult.SetActive(true);
        mTextResult.text = "Game Clear";
        bStart = false;
        GameManager.Instance.bClearSanyeah = true;
        Invoke("GoToSanyeahScene",2f);
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

            if (fStartTime < 1.25f && !bDropNote)
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
        float leftLastTime = 0f;

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
                    mFDropLeftLastTime = 0f;
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
                    mFDropRightLastTime = 0f;
                }
                else
                {
                    bRightDone = true;
                }
            }

            if (bLeftDone && bRightDone)
            {
                mIsAllNoteDrop = true;
                yield break;
            }

            currentTime += Time.deltaTime;
            mFDropLeftLastTime += Time.deltaTime;
            mFDropRightLastTime += Time.deltaTime;
            yield return null;
        }
    }

    public void JudgeLeft()
    {        
        float result = mNoteMgrLeft.GetJudgeDistance();
        if (result > Global.SANYEAH_NOTE_JUDGE_MISS || result < 0)
        {
            mEffectMissSrc.Play();
            return;
        }
        mEffectSrc.Play();
        PlayRhythmAniLeft(GetScore(result));
    }

    public void JudgeRight()
    {
        float result = mNoteMgrRight.GetJudgeDistance();
        if (result > Global.SANYEAH_NOTE_JUDGE_MISS || result < 0)
        {
            mEffectMissSrc.Play();
            return;
        }
        mEffectSrc.Play();
        PlayRhythmAniRight(GetScore(result));
    }

    public void PlayRhythmAniLeft(eScore score)
    {
        mGoEffectL.SetActive(false);
        if(score != eScore.BAD && score!= eScore.MISS)
            mGoEffectL.SetActive(true);
        
        TextEffectEnable(mTextLeft,score,"TextEffectDisableLeft");
        
        if (score == eScore.MISS)
            mAniSanyeahRhythm.SetTrigger("RhythmStumble");
        else
            mAniSanyeahRhythm.SetTrigger("RhythmLeft");
        
    }
    
    public void PlayRhythmAniRight(eScore score)
    {
        
        mGoEffectR.SetActive(false);
        if(score != eScore.BAD && score!= eScore.MISS)
            mGoEffectR.SetActive(true);
        
        TextEffectEnable(mTextRight,score,"TextEffectDisableRight");

        if (score == eScore.MISS)
            mAniSanyeahRhythm.SetTrigger("RhythmStumble");
        else
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
        mImgGaugeValue.fillAmount += (float) score / 1000f;
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
        StartCoroutine("GoToSanyeahSceneAfter");
    }

    IEnumerator GoToSanyeahSceneAfter()
    {
        if (GameManager.Instance.bClearSanyeah)
        {
            var req = new ReqQuestAction();
            req.type = ReqQuestAction.eType.Game.ToString();
            req.target = "Np_0002";
            req.count = mITotalScore;
            bool bReqComplete = false;
            WebReq.Instance.Request(req, delegate(ReqQuestAction.Res res)
            {
                bReqComplete = true;
                if (res.IsSuccess)
                {
                }
            });
            while (!bReqComplete)
            {
                yield return null;
            }
        }
        
        GameManager.Instance.Scene.LoadScene(GameData.eScene.SanyeahScene);
    }

    public override void Clear(Action del)
    {
        Debug.Log("SanyeahGameScene is clear.");
        del();
    }
}
