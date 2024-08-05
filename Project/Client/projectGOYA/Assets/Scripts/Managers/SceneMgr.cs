using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMgr
{
    public GameData.eScene prevScene= GameData.eScene.FirstScene;
    public BaseScene currentScene
    {
        get
        {
            return GameObject.FindObjectOfType<BaseScene>();
        }
    }

    private string GetSceneName(GameData.eScene sceneType)
    {
        string strSceneName = System.Enum.GetName(typeof(GameData.eScene), sceneType);
        return strSceneName;
    }
    
    public string GetCurrentSceneID()
    {
        var sceneType = currentScene.m_eSceneType;
        switch (sceneType)
        {
            case GameData.eScene.PrologScene :
                return "Ma_0000";
            case GameData.eScene.MainScene :
                return "Ma_0001";
            case GameData.eScene.SanyeahScene:
                return "Ma_0002";
            default:
                break;
        }
        return "";
    }

    public void LoadSceneByID(string id)
    {
        var scene = GameData.eScene.MainScene;
        switch (id)
        {
            case "Ma_0000" :
                scene = GameData.eScene.PrologScene;
                break;
            case "Ma_0001" :
                scene = GameData.eScene.MainScene;
                break;
            case "Ma_0002" :
                scene = GameData.eScene.SanyeahScene;
                break;
            default:
                break;
        }
        
        LoadScene(scene);
        
    }

    public void LoadScene(GameData.eScene sceneType, Action del = null)
    {
        currentScene.Clear(delegate
        {
            prevScene = currentScene.m_eSceneType;
            SceneManager.LoadScene(GetSceneName(sceneType));
            if (del != null)
                del();
        });

    }
    
}
