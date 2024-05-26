using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroScene : BaseScene
{
    public Button mBtnNext;
    protected override void InitScene()
    {
        base.InitScene();
        m_eSceneType = GameData.eScene.IntroScene;
    }
    
    public override void Clear()
    {
        Debug.Log("IntroScene is clear.");
    }

    private void Start()
    {
        GameData.DialogData[] list = new[] {new GameData.DialogData("주막주인", 1,new[] { "사람이 되고 싶다고?", "과제를 내줄게.", "산예의 수염을 가져오렴."}, false,3) };
        GameData.dicDialogData.Add("주막주인", list);
        list = new[] {new GameData.DialogData("산예", 1,new[] { "내 잠을 깨우다니!", "너 재미있는 소리를 가졌구나!", "나를 신나게 해줘!"}, false,-1),new GameData.DialogData("산예", 2,new[] {"나를 이렇게 지치게 하다니...", "[산예의 수염]을 얻었다!"}, false,2) };
        GameData.dicDialogData.Add("산예", list);
        
        mBtnNext.onClick.AddListener(delegate
        {
            GameManager.Instance.Scene.LoadScene(GameData.eScene.MainScene);
        });
    }

    
}