using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager s_instance;
    public static GameManager Instance
    {
        get
        {
            Init();
            return s_instance;
        }
    }

    private _SceneManager _sceneMgr = new _SceneManager();
    public _SceneManager Scene { get { return Instance._sceneMgr; } }
    void Start()
    {
        Init();
    }

    static void Init()
    {
        if(s_instance == null)
        {
            GameObject obj = GameObject.Find("GameManager");
            if (obj == null)
            {
                obj = new GameObject { name = "@GameManager" };
                obj.AddComponent<GameManager>();
            }
            
            DontDestroyOnLoad(obj);
            s_instance = obj.GetComponent<GameManager>();
        }
    }
    
}


