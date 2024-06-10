using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class _SceneManager
{
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

    public void LoadScene(GameData.eScene sceneType)
    {
        currentScene.Clear(delegate
        {
            SceneManager.LoadScene(GetSceneName(sceneType));
        });

    }
    
}
