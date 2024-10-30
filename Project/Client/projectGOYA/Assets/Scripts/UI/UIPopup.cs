using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPopup : MonoBehaviour
{
    public Action m_delClose = null;
    private Image m_bg;

    public virtual void Init(Action del = null)
    {
        m_bg = gameObject.AddComponent<Image>();
        var clr = Color.black;
        clr.a = 0f;
        m_bg.color = clr;
        
        if (del != null)
            m_delClose = del;
        m_delClose += delegate
        {
            PopupManager.Instance.SetClosePopup();

        };
    }

    public void SetBG(bool bSet)
    {
        var clr = Color.black;
        if (bSet)
        {
            clr.a = 0.5f;
            m_bg.color = clr;
        }
        else
        {

            clr.a = 0f;
            m_bg.color = clr;
        }
    }

    public void OnClose()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        if(!WebGLInput.captureAllKeyboardInput)
            WebGLInput.captureAllKeyboardInput = true;
#endif
        m_delClose();
        gameObject.SetActive(false);
    }
    
    
}
