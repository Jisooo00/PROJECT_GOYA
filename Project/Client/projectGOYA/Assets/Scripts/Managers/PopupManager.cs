using Protocols;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class PopupManager : MonoBehaviour
{

    private static PopupManager instance;
    private static Canvas m_canvas;
    private static RectTransform m_rectTransform;
    private static Image m_bg;
    public static PopupManager Instance
    {
        get
        {
            if (instance != null)
            {
                return instance;
            }

            instance = FindObjectOfType<PopupManager>();

            if (instance == null)
            {
                var go = new GameObject("Popup Manager");
                instance = go.AddComponent<PopupManager>();
                InitCanvas(go);
            }

            return instance;
        }
    }

    private Stack<UIPopup> m_stkOpenPopup = new Stack<UIPopup>();
    private bool isSettingPopupOpen = false;

    private static void InitCanvas(GameObject go)
    {
        m_canvas = go.AddComponent<Canvas>();
        m_canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        m_canvas.sortingOrder = 10;
        go.AddComponent<GraphicRaycaster>();
        var canvasScaler = go.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1600, 900);
        canvasScaler.matchWidthOrHeight = 0;
        m_rectTransform = m_canvas.GetComponent<RectTransform>();

    }

    public void SetOpenPopup(UIPopup popup)
    {
        if(m_stkOpenPopup.Count > 0)
            m_stkOpenPopup.Peek().SetBG(false);
        m_stkOpenPopup.Push(popup);
        UpdatePopupManager();
    }

    public void SetClosePopup()
    {
        var popup = m_stkOpenPopup.Pop();
        Destroy(popup.gameObject);
        UpdatePopupManager();
    }

    private void UpdatePopupManager()
    {

        if(m_stkOpenPopup.Count > 0)
            m_stkOpenPopup.Peek().SetBG(true);
        gameObject.SetActive(m_stkOpenPopup.Count != 0);
        
    }

    public void OpenPopupNotice(string msg, Action del = null, string title = "",bool bNoBtn = false)
    {
        var go = Load("Prefabs/UI/UIPopupNotice", m_rectTransform);
        var popupNotice= go.GetComponent<UIPopupNotice>();
        popupNotice.Init(delegate
        {
            if (del != null)
                del();
        }, msg, title,bNoBtn);
        popupNotice.gameObject.SetActive(true);
        SetOpenPopup(popupNotice);
    }
    
    public void OpenPopupSetting(Action del = null)
    {
        if(isSettingPopupOpen)
            return;
        var go = Load("Prefabs/UI/UIPopupSetting", m_rectTransform);
        var popupSetting= go.GetComponent<UIPopupSetting>();
        popupSetting.Init(delegate
        {
            if (del != null) del();
            
            isSettingPopupOpen = false;
        }, "", "설정");
        popupSetting.gameObject.SetActive(true);
        SetOpenPopup(popupSetting);
        isSettingPopupOpen = true;
    }

    public void OpenPopupAccount(Action del = null)
    {
        var go = Load("Prefabs/UI/UIPopupAccount", m_rectTransform);
        var popupAccount= go.GetComponent<UIPopupAccount>();
        popupAccount.Init(delegate
        {
            if (del != null)
                del();
        });
        popupAccount.gameObject.SetActive(true);
        SetOpenPopup(popupAccount);
    }
    public void OpenPopupSignIn(Action del = null)
    {
        var go = Load("Prefabs/UI/UIPopupSignIn", m_rectTransform);
        var popupSignIn= go.GetComponent<UIPopupSignIn>();
        popupSignIn.Init(delegate
        {
            if (del != null)
                del();
        });
        popupSignIn.gameObject.SetActive(true);
        SetOpenPopup(popupSignIn);
    }
    
    public void OpenPopupSignUp(Action del = null)
    {
        var go = Load("Prefabs/UI/UIPopupSignUp", m_rectTransform);
        var popupSignUp= go.GetComponent<UIPopupSignUp>();
        popupSignUp.Init(delegate
        {
            if (del != null)
                del();
        });
        popupSignUp.gameObject.SetActive(true);
        SetOpenPopup(popupSignUp);
    }
    public void OpenPopupSetNickname(Action del = null)
    {
        var go = Load("Prefabs/UI/UIPopupSetNickname", m_rectTransform);
        var popupSetNickname= go.GetComponent<UIPopupSetNickname>();
        popupSetNickname.Init(delegate
        {
            if (del != null)
                del();
        });
        popupSetNickname.gameObject.SetActive(true);
        SetOpenPopup(popupSetNickname);
    }


    private GameObject Load(string name, RectTransform rect)
    {
        var o = Resources.Load(name) as GameObject;

        var go = GameObject.Instantiate(o);
        go.SetActive(false);
		
        var tm = go.GetComponent<RectTransform>();
        tm.SetParent(rect, false);
        tm.anchoredPosition = Vector2.zero;
        tm.sizeDelta.Set(rect.rect.width, rect.rect.height);

        return go;
    }

    
}