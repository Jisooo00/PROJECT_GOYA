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

    private SceneMgr sceneMgr = new SceneMgr();
    public SceneMgr Scene { get { return Instance.sceneMgr; } }
    void Start()
    {
        Init();
    }

    public bool bClearSanyeah = false;

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


