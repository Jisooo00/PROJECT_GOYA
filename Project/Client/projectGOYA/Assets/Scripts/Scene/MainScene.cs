
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainScene : BaseScene
{
    protected override void InitScene()
    {
        base.InitScene();
        m_eSceneType = GameData.eScene.MainScene;
        SetUIManager(m_eSceneType);
    }
    
    public override void Clear(Action del)
    {
        Player.instance.PlayEffect("Chara_DISAPPEAR");
        if (del != null)
        {
            StartCoroutine(ClearSceneAfter(del));
        }
        Debug.Log("MainScene is clear.");

    }
        
    IEnumerator ClearSceneAfter(Action del)
    {
        yield return new WaitForSeconds(0.5f);
        del();
    }
}
