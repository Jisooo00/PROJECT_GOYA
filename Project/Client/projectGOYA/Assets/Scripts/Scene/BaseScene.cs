using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

public abstract class BaseScene : MonoBehaviour
{
   public GameData.eScene m_eSceneType { get; protected set; } = GameData.eScene.None;
   public UIManager m_uiManager = null;
   public Canvas mCanvas;

   private void Awake()
   {
      InitScene();
   }

   protected virtual void InitScene()
   {
         
      Object obj = FindObjectOfType(typeof(EventSystem));
      if (obj == null)
      {
         GameObject objEvtSystem = new GameObject("EventSystem");
         objEvtSystem.AddComponent<EventSystem>();
      }
      
   }

   public void SetUIManager(Action del = null)
   {
      if (m_uiManager == null)
      {
         GameObject go = Resources.Load("Prefabs/UI/UIManager") as GameObject;
         if (go != null)
         {
            m_uiManager = go.GetComponent<UIManager>();
            GameObject ui = Instantiate(go);
            if (ui != null)
            {
               ui.transform.SetParent(mCanvas.transform);
               ui.transform.SetAsFirstSibling();
               var rect = ui.GetComponent<RectTransform>();
               rect.offsetMin = Vector2.zero;
               rect.offsetMax = Vector2.zero;
               rect.localScale = Vector2.one;
            }
            else
            {
               Debug.LogError("Instantiation of UIManager is Failed");
            }
         }
      }
      
      m_uiManager.Init();

      if (del != null)
         del();

   }
   

   public abstract void Clear(Action del);

   public abstract void DelFunc();

}
