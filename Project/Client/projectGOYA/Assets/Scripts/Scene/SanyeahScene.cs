using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SanyeahScene : BaseScene
{
    protected override void InitScene()
    {
        base.InitScene();
        m_eSceneType = GameData.eScene.MainScene;
    }
    
    public override void Clear()
    {
        Debug.Log("MainScene is clear.");
    }
}
