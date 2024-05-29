
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainScene : BaseScene
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
