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
        GameData.DialogData[] list = new[] {new GameData.DialogData("�ָ�����", 1,new[] { "����� �ǰ� �ʹٰ�?", "������ ���ٰ�.", "�꿹�� ������ ��������."}, false,3) };
        GameData.dicDialogData.Add("�ָ�����", list);
        list = new[] {new GameData.DialogData("�꿹", 1,new[] { "�� ���� ����ٴ�!", "�� ����ִ� �Ҹ��� ��������!", "���� �ų��� ����!"}, false,-1),new GameData.DialogData("�꿹", 2,new[] {"���� �̷��� ��ġ�� �ϴٴ�...", "[�꿹�� ����]�� �����!"}, false,2) };
        GameData.dicDialogData.Add("�꿹", list);
        
        mBtnNext.onClick.AddListener(delegate
        {
            GameManager.Instance.Scene.LoadScene(GameData.eScene.MainScene);
        });
    }

    
}