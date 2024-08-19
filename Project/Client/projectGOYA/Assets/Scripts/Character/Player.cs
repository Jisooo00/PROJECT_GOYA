
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Player : MonoBehaviour
{

    public static Player instance;
    public float moveSpeed = 10f; //조절 필요
    public Rigidbody2D rb;
    
    private Vector2 movement;
    private Vector2 dest;
    
    [NonSerialized] public bool bInputUp = false;
    [NonSerialized] public bool bInputDown = false;
    [NonSerialized] public bool bInputLeft = false;
    [NonSerialized] public bool bInputRight = false;
    [NonSerialized] public bool bIsDialogPlaying = false;
    public bool bIsOnGoingTutorial
    {
        get
        {
            return (GameData.QuestDatas.ContainsKey("Qu_0000") && GameData.QuestDatas["Qu_0000"].GetState() !=
                GameData.QuestData.eState.FINISHED);
        }
    }

    public Animation mAniEffect;
    public GameObject mGoEffect;

    private bool bIsMoving = false;
    public Vector3 dirVec;
    [NonSerialized] public GameObject mScanObject = null;

    public GameData.DialogData GetScannedMonster()
    {
        if (mScanObject == null)
        {
            return null;
        }

        var data = mScanObject.GetComponent<MonsterBase>();
        return data.CurDialog;
    }
    
    public List<GameData.DialogData> GetScannedMonsterRefresh()
    {
        if (mScanObject == null)
        {
            return null;
        }

        var data = mScanObject.GetComponent<MonsterBase>();
        data.RefreshData();
        return data.mData;
    }
    
    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        
        if (bIsDialogPlaying)
            return;
        CheckMovement();

        if (bIsMoving)
        {
            movement.x = bInputLeft ? -1 : bInputRight ? 1: 0;
            movement.y = bInputUp ? 1 : bInputDown ? -1 : 0;
            GameManager.Instance.saveData.CurPos = instance.transform.localPosition;
        }
        else
        {
            movement = Vector2.zero;
            
#if UNITY_EDITOR
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");
#endif
        }
        
        dirVec = CharacterAppearance.instance.GetDirection();

    }

    void CheckMovement()
    {

        bool bMoveX = (bInputLeft || bInputRight);
        bool bMoveY = (bInputUp || bInputDown);

        bIsMoving = (bMoveX  || bMoveY);
        
    }
    void FixedUpdate()
    {
        float debuf = movement.x != 0f && movement.y != 0f ? 0.8f : 1.0f; 
        rb.MovePosition(rb.position + movement * moveSpeed*debuf * Time.fixedDeltaTime);
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 6)
        {
            mScanObject = other.transform.parent.gameObject;
        }
        if (other.gameObject.layer == 9)
        {
            mScanObject = other.gameObject;
        }
        
        if (other.gameObject.layer == 7)
        {
            
            if(bIsDialogPlaying)
            {
                SetForceStop();
                return;
            }
            
            foreach (var dialog in GameData.GetDialog("np_0001"))
            {
                if (dialog.m_strDialogID == "Dl_0002")
                {
                    if (dialog.m_bPlayed)
                    {
                        GameManager.Instance.Scene.LoadScene(GameData.eScene.SanyeahScene);
                        return;
                    }
                }
            }
            
            SetForceStop();
            var uiManager = GameManager.Instance.Scene.currentScene.m_uiManager;
            uiManager.ForceJoystickPointerUp();
            
            uiManager.PlayDialogForce("Dl_0014",delegate{
                transform.localPosition = new Vector3(-13,-3.8f,0f);
            });
            
        }
        if (other.gameObject.layer == 8)
        {
            GameManager.Instance.Scene.LoadScene(GameData.eScene.MainScene);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == 6 && other.transform.parent.gameObject)
        {
            mScanObject = null;
        }
        
        if (other.gameObject.layer == 9 && other.gameObject)
        {
            mScanObject = null;
        }
    }

    public void SetInputPos(Vector2 inputPos)
    {
        float moveX = inputPos.x;
        float moveY = inputPos.y;

        bInputUp    = moveX == 0 ? moveY > 0 : moveY > 0.25f; 
        bInputDown = moveX == 0 ? moveY < 0 : moveY < -0.25f; 
        bInputLeft  = moveY == 0? moveX < 0 : moveX < -0.25f;
        bInputRight = moveY == 0? moveX > 0 : moveX > 0.25f; 
        
    }

    public void SetSleep(bool bSleep)
    {
        CharacterAppearance.instance.SetSleep(bSleep);
    }
    
    public void SetForceStop(){
        bInputUp    = false; 
        bInputDown = false; 
        bInputLeft  = false; 
        bInputRight = false; 
    }
    
    
}
