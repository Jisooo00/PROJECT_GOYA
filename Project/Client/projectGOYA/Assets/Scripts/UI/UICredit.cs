using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICredit : MonoBehaviour
{
    public Button mBtnClose;
    public bool IsActive
    {
        get
        {
            return gameObject.activeSelf;
        }
    }

    private void Start()
    {
        mBtnClose.onClick.AddListener(delegate
        {
            if(IsActive)
                gameObject.SetActive(false);
            
            Debug.Log("buttonClose");
        });
    }
}