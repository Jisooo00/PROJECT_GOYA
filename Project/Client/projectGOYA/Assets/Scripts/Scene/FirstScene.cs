using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class FirstScene : BaseScene
{

    public bool bTestGuestMode = false;
    public SaveDataManager SaveDataManager;
    protected override void InitScene()
    {
        base.InitScene();
        m_eSceneType = GameData.eScene.FirstScene;
        //m_uiManager.Init(m_eSceneType);
    }
    
    public override void Clear(Action del)
    {
        Debug.Log("IntroScene is clear.");
        del();
    }
    
    public override void DelFunc()
    {
        
    }
    
    public Image m_imgTeamLogo;

    void Awake()
    {
        
//        DontDestroyOnLoad(SaveDataManager.gameObject);
    }
    void Start()
    {
#if UNITY_EDITOR

        if (bTestGuestMode)
        {
            SaveDataManager.InitializeData();
        }
        
#endif

        if(Application.internetReachability == NetworkReachability.NotReachable){
            
            PopupManager.Instance.OpenPopupNotice("네트워크 연결이 원활하지 않습니다.", delegate()
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            });
        }
        
        StartCoroutine(StartAfter());
    }
    IEnumerator StartAfter()
    {
        GameManager.Instance.saveData = SaveDataManager;
        yield return new WaitForSeconds(1.5f);
        WebReq.Instance.CheckNoticeData();
        while (!GameData.bInitNoticeData)
        {
            yield return null;
        }
        GameManager.Instance.Scene.LoadScene(GameData.eScene.IntroScene);
        
    }
}
