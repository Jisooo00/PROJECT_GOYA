using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButton : MonoBehaviour
{
    public Button mBtn;
    private Color mColor;
    private bool IsPressed;

    public bool Pressed
    {
        get
        {
            return IsPressed;
        }
    }

    private void Start()
    {
        mColor = mBtn.image.color;
    }

    public void OnPointerEnter()
    {
        mBtn.image.color = mColor*mBtn.colors.pressedColor;
        IsPressed = true;
    }
    
    public void OnPointerExit()
    {
        if (IsPressed)
        {
            mBtn.image.color = mColor*mBtn.colors.normalColor;
            IsPressed = false;
        }

    }

}
