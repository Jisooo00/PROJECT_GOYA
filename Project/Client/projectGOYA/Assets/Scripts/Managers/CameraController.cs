using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;
using Vector3 = UnityEngine.Vector3;

public class CameraController : MonoBehaviour
{

    [SerializeField] private float mCenterX;
    [SerializeField] private float mCenterY;
    [SerializeField] private float mMapSizeX;
    [SerializeField] private float mMapSizeY;

    private Vector3 mCenter;
    private Vector3 mMapSize;

    private float mHeight;
    private float mWidth;

    private void Awake()
    {
        transform.position = Player.instance.rb.position;
    }

    void Start()
    {
        mHeight = Camera.main.orthographicSize;
        mWidth = mHeight * Screen.width / Screen.height;

        mCenter = new Vector3(mCenterX, mCenterY, 0.0f);
        mMapSize = new Vector3(mMapSizeX, mMapSizeY, 0.0f);
    }
    
    private void FixedUpdate()
    {
        LimitCameraArea();
    }

    void LimitCameraArea()
    {
        var PlayerPos = Player.instance.rb.position;
        Vector3 v3NewPos = new Vector3(PlayerPos.x, PlayerPos.y, -10);
        transform.position = Vector3.Lerp(transform.position,v3NewPos,Time.deltaTime*GameData.myData.SET_CAM);
        
        float lx = mMapSize.x - mWidth;
        float clampX = Mathf.Clamp(transform.position.x, -lx + mCenter.x, lx + mCenter.x);

        float ly = mMapSize.y - mHeight;
        float clampY = Mathf.Clamp(transform.position.y, -ly + mCenter.y, ly + mCenter.y);

        transform.position = new Vector3(clampX, clampY, -10f);
        
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(mCenter, mMapSize * 2);
    }
#endif

}
