using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using TMPro;
using Unity.VisualScripting;
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
    [SerializeField] private Image mGradeLeft;
    [SerializeField] private Image mGradeRight;
    [SerializeField] private Sprite[] mListGrade;
    [SerializeField] private AudioClip mAudioClip;
    [SerializeField] private AudioClip mEffectClip;
    [SerializeField] private AudioClip mEffectMissClip;
    [SerializeField] private AudioClip mEffectFeverClip;
    private Animator mAniSanyeahRhythm;
    [SerializeField] private Animator mAniNormalSanyeahRhythm;
    [SerializeField] private Animator mAniMasterSanyeahRhythm;
    // Tool 이용해 NoteData 제작
    [SerializeField] private NoteData mNoteDataLeft;
    [SerializeField] private NoteData mNoteDataRight;
    
    [Header("InGame UI")]
    [SerializeField] private GameObject mGoBeforeStart;
    [SerializeField] private GameObject mGoWebtuto;
    [SerializeField] private Button mBtnStart;
    [SerializeField] private Button mBtnReplay;
    [SerializeField] private Button mBtnExitClear;
    [SerializeField] private Button mBtnExitOver;
    [SerializeField] private Button mBtnPause;
    [SerializeField] private Button mBtnPauseReplay;
    [SerializeField] private GameObject mGoGameResult;
    [SerializeField] private GameObject mGoGamePause;
    [SerializeField] private GameObject mGoGameResultClear;
    [SerializeField] private GameObject mGoGameResultOver;
    [SerializeField] private TMP_Text mTextScore;
    [SerializeField] private Button mBtnR;
    [SerializeField] private Button mBtnL;
    [SerializeField] private Image mImgGaugeValue;
    [SerializeField] private GameObject[] mListGoNormal;
    [SerializeField] private GameObject[] mListGoFever;
    
    private AudioSource mAudioSrc;
    private AudioSource mEffectSrc;
    private AudioSource mEffectMissSrc;
    private AudioSource mEffectFeverSrc;

    private float mFDropLeftLastTime = 0f;
    private float mFDropRightLastTime = 0f;
    private bool mIsAllNoteDrop = false;
    private bool bCheckFiver = false;
    private bool bFiverMode = false;
    private bool bDelayForce = false;
    
    //배경음과의 Sync를 위해 게임 시작 시점과 Note활성화 시점 분리
    private bool bStart = false;
    private bool bDropNote = false;
    private bool bPause = false;

    private int mITotalScore = 0;
    
    public enum eScore
    {
        PERFECT = 100,
        //GREAT = 75,
        GOOD = 50,
        //BAD = -25,
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
        SetInitGame();
        
        
        if (mAudioClip != null)
        {
            mAudioSrc = gameObject.AddComponent<AudioSource>();
            mAudioSrc.clip = mAudioClip;
            mAudioSrc.volume = 0.7f;// * GameData.myData.SET_VOLUME;
        } 
        
        if (mEffectClip != null)
        {
            mEffectSrc = gameObject.AddComponent<AudioSource>();
            mEffectSrc.clip = mEffectClip;
            mEffectSrc.volume = 1f;
        }
        if (mEffectMissClip != null)
        {
            mEffectMissSrc = gameObject.AddComponent<AudioSource>();
            mEffectMissSrc.clip = mEffectMissClip;
        }

        if (mEffectFeverClip != null)
        {
            mEffectFeverSrc = gameObject.AddComponent<AudioSource>();
            mEffectFeverSrc.clip = mEffectFeverClip;
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
        
        mBtnStart.onClick.AddListener(delegate
        {
            AudioManager.Instance.PlayClick();
            mGoBeforeStart.SetActive(false);
            StartCoroutine("StartGame");
        });
        
        mBtnReplay.onClick.AddListener(delegate
        {
            AudioManager.Instance.PlayClick();
            StopAllCoroutines();
            SetInitGame();
            mGoBeforeStart.SetActive(true);
#if UNITY_WEBGL
            mGoWebtuto.SetActive(true);
#endif
            
        });
        mBtnExitClear.onClick.AddListener(delegate
        {
            AudioManager.Instance.PlayClick();
            StopAllCoroutines();
            //SetInitGame();
            //mGoBeforeStart.SetActive(true);
            GoToSanyeahScene();
        });
        mBtnExitOver.onClick.AddListener(delegate
        {
            AudioManager.Instance.PlayClick();
            StopAllCoroutines();
            //SetInitGame();
            //mGoBeforeStart.SetActive(true);
            GoToSanyeahScene();
        });
        
        mBtnPause.onClick.AddListener(delegate
        {
            mAudioSrc.Pause();
            bPause = true;
            AudioManager.Instance.PlayClick();
            Time.timeScale = 0;
            mGoGamePause.SetActive(true);

        });
        
        mBtnPauseReplay.onClick.AddListener(delegate
        {
            AudioManager.Instance.PlayClick();
            Time.timeScale = 1;
            mGoGamePause.SetActive(false);
            mAudioSrc.UnPause();
            bPause = false;

        });
        
    }
    
    void SetInitGame()
    {
        mGoEffectR.SetActive(false);
        mGoEffectL.SetActive(false);
        mGoBeforeStart.SetActive(true);
#if UNITY_WEBGL
            mGoWebtuto.SetActive(true);
#endif
        mGoGameResult.SetActive(false);
        mTextScore.text = string.Format("Score : {0:#,###}", 0);
        TextEffectDisableRight();
        TextEffectDisableLeft();
        mFDropLeftLastTime = 0f;
        mFDropRightLastTime = 0f;
        mIsAllNoteDrop = false;
        bCheckFiver = false;
        bFiverMode = false;
        bStart = false;
        bDelayForce = false;
        bDropNote = false;
        mITotalScore = 0;
        mImgGaugeValue.fillAmount = 0.5f;
        mAniSanyeahRhythm = mAniNormalSanyeahRhythm;
        SetFeverMode(false);
    }

    private void Update()
    {

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //PlayerPrefs.SetString(string.Format("{0}_{1}", GameData.myData.user_uid, "Dl_0006"), "true");
            //PlayerPrefs.Save();
            GameData.SetDialogPlayed("np_0002","Dl_0006");
            mITotalScore = 5000;
            SetGameClear();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            //Debug.Log("Go Fiver");
            bFiverMode = true;
            mEffectFeverSrc.Play();
            SetFeverMode(true);
        }
#endif        

#if UNITY_WEBGL || UNITY_EDITOR
        if(Input.GetKeyDown(KeyCode.A))
        {
            JudgeLeft();
        }
        if(Input.GetKeyDown(KeyCode.D))
        {
            JudgeRight();
        }
#endif
        


        if(bPause)
            return;
        
        if (!bStart)
            return;

        if (bCheckFiver && !bFiverMode && mImgGaugeValue.fillAmount > 0.6f)
        {
            //Debug.Log("Go Fiver");
            bFiverMode = true;
            mEffectFeverSrc.Play();
            SetFeverMode(true);
        }
        
        if(mImgGaugeValue.fillAmount == 0)
            SetGameOver();
        
        if (bStart && !mAudioSrc.isPlaying)
        {
            if(mImgGaugeValue.fillAmount >= 0.6f)
                SetGameClear();
            else
            {
                SetGameOver();
            }
        }

        if (!mIsAllNoteDrop)
        {
            bool bDelay = mFDropRightLastTime > 1f || mFDropLeftLastTime > 1f || bDelayForce;
            mImgGaugeValue.fillAmount -= Time.deltaTime / (bDelay ? 20f : 6f);

        }

    }

    private void SetGameOver()
    {
        mGoGameResult.SetActive(true);
        mGoGameResultOver.SetActive(true);
        mGoGameResultClear.SetActive(false);
        if(!mIsAllNoteDrop)
            StopCoroutine("DropNoteData");
        mNoteMgrLeft.ForceAllNoteStop();
        mNoteMgrRight.ForceAllNoteStop();
        bStart = false;
        mAudioSrc.Stop();
        GameManager.Instance.bClearSanyeah = false;
    }
    
    private void SetGameClear()
    {
        mGoGameResult.SetActive(true);
        mGoGameResultOver.SetActive(false);
        mGoGameResultClear.SetActive(true);
        bStart = false;
        GameManager.Instance.bClearSanyeah = true;
        
    }

    private IEnumerator StartGame()
    {
        if(bStart)
            yield break;
        
        float fStartTime = 0f;
        

        if (!bDropNote)
            StartCoroutine("DropNoteData");
        
        while (fStartTime < 1.25f)
        {
            fStartTime += Time.deltaTime*Time.timeScale;
            yield return null;
        }
        
        bStart = true;
        bDelayForce = true;

        mAudioSrc.Play();
        
        fStartTime = 0f;
        while (fStartTime < 30f)
        {
            fStartTime += Time.deltaTime*Time.timeScale;
            if(fStartTime > 10f && bDelayForce)
                bDelayForce = false;
            yield return null;
        }

        bCheckFiver = true;
        
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
        if (mIsAllNoteDrop)
            return;
        float result = mNoteMgrLeft.GetJudgeDistance();
        //if (result > Global.SANYEAH_NOTE_JUDGE_MISS*2f || result < -1*Global.SANYEAH_NOTE_JUDGE_GOOD)
        //{
        //    mEffectMissSrc.Play();
        //    return;
        //}
        PlayRhythmAniLeft(GetScore(result));
    }

    public void JudgeRight()
    {
        if (mIsAllNoteDrop)
            return;
        float result = mNoteMgrRight.GetJudgeDistance();
        //if (result > Global.SANYEAH_NOTE_JUDGE_MISS*2f || result < -1*Global.SANYEAH_NOTE_JUDGE_GOOD)
        //{
        //    mEffectMissSrc.Play();
        //    return;
        //}
        PlayRhythmAniRight(GetScore(result));
    }

    public void PlayRhythmAniLeft(eScore score)
    {
        mGoEffectL.SetActive(false);
        if( score!= eScore.MISS)
            mGoEffectL.SetActive(true);
        
        TextEffectEnable(mGradeLeft,score,"TextEffectDisableLeft");

        if (score == eScore.MISS)
        {
            mEffectMissSrc.Play();
            mAniSanyeahRhythm.SetTrigger("RhythmStumble");
        }
        else
        {
            mEffectSrc.Play();
            mAniSanyeahRhythm.SetTrigger("RhythmLeft");
        }

    }
    
    public void PlayRhythmAniRight(eScore score)
    {
        
        mGoEffectR.SetActive(false);
        if(score!= eScore.MISS)
            mGoEffectR.SetActive(true);
        
        TextEffectEnable(mGradeRight,score,"TextEffectDisableRight");

        if (score == eScore.MISS)
        {
            mEffectMissSrc.Play();
            mAniSanyeahRhythm.SetTrigger("RhythmStumble");
        }
        else
        {
            mEffectSrc.Play();
            mAniSanyeahRhythm.SetTrigger("RhythmRight");
        }
    }

    public eScore GetScore(float result)
    {
        eScore score = eScore.PERFECT;
        //Debug.Log("result"+result);

        if (result <= Global.SANYEAH_NOTE_JUDGE_PERFECT && result > -1*Global.SANYEAH_NOTE_JUDGE_PERFECT)
        {
            score = eScore.PERFECT;
        }
        //else if (result < Global.SANYEAH_NOTE_JUDGE_GREAT)
        //{
        //    score = eScore.GREAT;
        //}
        else if (result <= Global.SANYEAH_NOTE_JUDGE_GOOD && result > -1*Global.SANYEAH_NOTE_JUDGE_GOOD)
        {
            score = eScore.GOOD;
        }
        //else if (result < Global.SANYEAH_NOTE_JUDGE_BAD)
        //{
        //    score = eScore.BAD;
        //}
        else
        {
            score = eScore.MISS;
        }
        //Debug.Log("score"+score);
        
        mITotalScore += (int) score;
        mImgGaugeValue.fillAmount += (float) score / 1000f;
        mTextScore.text = string.Format("Score : {0:#,###}", mITotalScore);

        return score;
    }

    void TextEffectEnable(Image image, eScore score,string strInvokeMethod)
    {
        CancelInvoke(strInvokeMethod);
        image.gameObject.SetActive(false);
        
        switch (score)
        {
            case eScore.PERFECT:
                image.sprite = mListGrade[0];
                break;
            //case eScore.GREAT:
            //    text.color = Color.yellow;
            //    break;
            case eScore.GOOD:
                image.sprite = mListGrade[1];
                break;
            //case eScore.BAD:
            //    text.color = Color.red;
            //    break;
            case eScore.MISS:
                image.sprite = mListGrade[2];
                break;
        }
        
        image.gameObject.SetActive(true);
        Invoke(strInvokeMethod,1f);
    }
    
    void TextEffectDisableLeft()
    {
        mGradeLeft.gameObject.SetActive(false);
    }
    
    void TextEffectDisableRight()
    {
        mGradeRight.gameObject.SetActive(false);
    }

    void SetFeverMode(bool isFever)
    {
        foreach (var normal in mListGoNormal)
        {
            normal.SetActive(!isFever);
        }
        foreach (var fever in mListGoFever)
        {
            fever.SetActive(isFever);
        }

        if(isFever)
            mAniSanyeahRhythm = mAniMasterSanyeahRhythm;
    }

    void GoToSanyeahScene()
    {
        StartCoroutine("GoToSanyeahSceneAfter");
    }

    IEnumerator GoToSanyeahSceneAfter()
    {
        if (GameManager.Instance.bClearSanyeah)
        {
            /*var req = new ReqQuestAction();
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
            */
            bool bReqComplete = false;
            var req = new ReqQuestClear();
            req.questId = Global.KEY_QUEST_SANYEAH;
            WebReq.Instance.Request(req, delegate(ReqQuestClear.Res res)
            {
                bReqComplete = true;
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
