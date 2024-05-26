using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

public abstract class BaseScene : MonoBehaviour
{
   public GameData.eScene m_eSceneType { get; protected set; } = GameData.eScene.None;

   private void Awake()
   {
      InitScene();
   }

   protected virtual void InitScene()
   {
      
      GameManager mgr = GameManager.Instance;
         
      Object obj = FindObjectOfType(typeof(EventSystem));
      if (obj == null)
      {
         GameObject objEvtSystem = new GameObject("EventSystem");
         objEvtSystem.AddComponent<EventSystem>();
      }
      
   }

   public abstract void Clear();
}
