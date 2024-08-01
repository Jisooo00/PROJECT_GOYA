
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
    
    [NonSerialized] public bool bInputUp = false;
    [NonSerialized] public bool bInputDown = false;
    [NonSerialized] public bool bInputLeft = false;
    [NonSerialized] public bool bInputRight = false;
    [NonSerialized] public bool bIsDialogPlaying = false;

    public Animation mAniEffect;
    public GameObject mGoEffect;

    private bool bIsMoving = false;
    public Vector3 dirVec;
    [NonSerialized] public GameObject mScanObject = null;

    private float mFPlayFootstep = 0f;

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
        mFPlayFootstep += Time.deltaTime;
        
        if (bIsDialogPlaying)
            return;
        CheckMovement();

        if (bIsMoving)
        {
            if (mFPlayFootstep > 0.4f)
            {
                AudioManager.Instance.PlayFootstep();
                mFPlayFootstep = 0;
            }
            movement.x = bInputLeft ? -1 : bInputRight ? 1: 0;
            movement.y = bInputUp ? 1 : bInputDown ? -1 : 0;
/*
            if(movement.x != 0)
                dirVec = bInputLeft ? Vector3.left : Vector3.right;
            else
                dirVec = bInputUp ? Vector3.up : Vector3.down;
*/

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
        /*
        bool bMoveX = (bInputLeft && bInputRight) || (!bInputRight && !bInputLeft);
        bool bMoveY = (bInputUp && bInputDown) || (!bInputUp && !bInputDown);

        bIsMoving = (bMoveX && !bMoveY) || (!bMoveX && bMoveY);
        */

        bool bMoveX = (bInputLeft || bInputRight);
        bool bMoveY = (bInputUp || bInputDown);

        bIsMoving = (bMoveX  || bMoveY);
        
    }
    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        
        Debug.DrawRay(rb.position,dirVec,new Color(0,1,0));
        RaycastHit2D rayHit = Physics2D.Raycast(rb.position, dirVec, 1f, LayerMask.GetMask("InteractiveObject"));
        if (rayHit.collider != null)
        {
            mScanObject = rayHit.collider.gameObject;
        }
        else
        {
            mScanObject = null;
        }
    }

    public void PlayEffect(string AniClip)
    {
        mGoEffect.SetActive(false);
        mGoEffect.transform.localPosition = transform.localPosition;
        mGoEffect.SetActive(true);
        mAniEffect.Play(AniClip);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == 7)
        {
            GameManager.Instance.Scene.LoadScene(GameData.eScene.SanyeahScene);
        }
    }

    public void SetInputPos(Vector2 inputPos)
    {
        float moveX = inputPos.x;
        float moveY = inputPos.y;
        Debug.Log(inputPos);
        
        
        bInputUp    = moveX == 0 ? moveY > 0 : moveY > 0.25f; //Math.Abs(moveX) < Math.Abs(moveY) && moveY > 0;
        bInputDown = moveX == 0 ? moveY < 0 : moveY < -0.25f; //Math.Abs(moveX) < Math.Abs(moveY) && moveY < 0;
        bInputLeft  = moveY == 0? moveX < 0 : moveX < -0.25f; //Math.Abs(moveX) >= Math.Abs(moveY) && moveX < 0;
        bInputRight = moveY == 0? moveX > 0 : moveX > 0.25f; //Math.Abs(moveX) >= Math.Abs(moveY) && moveX > 0;
        
    }
    
}
