using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class FirstScene : BaseScene
{

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
    void Start()
    {
        StartCoroutine(StartAfter());
    }
    IEnumerator StartAfter()
    {
        yield return new WaitForSeconds(1.5f);
        GameManager.Instance.Scene.LoadScene(GameData.eScene.IntroScene);
    }
}
