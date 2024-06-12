using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SanyeahScene : BaseScene
{
    protected override void InitScene()
    {
        base.InitScene();
        m_eSceneType = GameData.eScene.MainScene;
        SetUIManager(m_eSceneType);
        if (GameManager.Instance.bClearSanyeah)
            Player.instance.transform.localPosition = Vector3.zero;
        Player.instance.PlayEffect("Chara_APPEAR");
        

    }
    
    public override void Clear(Action del)
    {
        Debug.Log("SanyeahScene is clear.");
        del();
    }
}
