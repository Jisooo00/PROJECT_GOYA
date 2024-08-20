using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIVirtualJoyStick : MonoBehaviour,IPointerDownHandler,IDragHandler,IPointerUpHandler,IBeginDragHandler,IEndDragHandler
{
    public GameObject mGobJoyStick;
    public Image mImgBg;
    public Image mImgController;
    private Vector3 mV3Origin;
    private RectTransform mRect;
    public GameObject m_goTutoPointer_action = null;
    private bool bIgnoreDrag = false;


    public void Awake()
    {
        m_goTutoPointer_action.SetActive(false);
    }

    public void Start()
    {

        mRect = mGobJoyStick.GetComponent<RectTransform>();
        mV3Origin = mRect.position;
        
    }

    public void SetTutorialPointer()
    {
        //Debug.Log("Set Pointer");
        m_goTutoPointer_action.SetActive(true);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("on Pointer Down");
        if(eventData.position.x < Screen.width*0.5f)
            mGobJoyStick.transform.position = eventData.position;

        
        if(m_goTutoPointer_action.activeSelf)
            m_goTutoPointer_action.SetActive(false);
        /*
        if (!mGobJoyStick.activeSelf)
        {
            mGobJoyStick.transform.position = eventData.position;
            mGobJoyStick.SetActive(true);
        }*/

    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        Vector2 touchPosition  = eventData.position;
        bIgnoreDrag = touchPosition.x > Screen.width * 0.5f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (bIgnoreDrag)
            return;
        
        Vector2 touchPosition = Vector2.zero;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(mImgBg.rectTransform, eventData.position,
                eventData.pressEventCamera, out touchPosition))
        {
            
            touchPosition.x = touchPosition.x / mImgBg.rectTransform.sizeDelta.x;
            touchPosition.y = touchPosition.y / mImgBg.rectTransform.sizeDelta.y;

            touchPosition = new Vector2(touchPosition.x * 2 , touchPosition.y * 2 );
            touchPosition = touchPosition.magnitude > 1 ? touchPosition.normalized : touchPosition;

            mImgController.rectTransform.anchoredPosition = new Vector2(
                touchPosition.x * mImgBg.rectTransform.sizeDelta.x / 2,
                touchPosition.y * mImgBg.rectTransform.sizeDelta.y / 2);
            
            Player.instance.SetInputPos(touchPosition);
            
            
        }

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        bIgnoreDrag = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {  
        mRect.position = mV3Origin;
        mImgController.rectTransform.anchoredPosition = new Vector2(0,0);
        Player.instance.SetInputPos(Vector2.zero);
        
    }
    public void ForcePointerUp()
    {  
        mRect.position = mV3Origin;
        mImgController.rectTransform.anchoredPosition = new Vector2(0,0);
        Player.instance.SetInputPos(Vector2.zero);
        
    }
    
    
}
