using System;
using System.Collections.Generic;
using UnityEngine;


public class NoteManager : MonoBehaviour
{
       private List<NoteObject> mListNotesPool = new List<NoteObject>();
       [SerializeField] private NoteObject mNotePrefab;
       [SerializeField] private Transform mTmParent;
       [SerializeField] private RectTransform mRTEndPoint;
       [SerializeField] private RectTransform mRTStartPoint;
       [SerializeField] private NoteObject.eSide mESide;
       public Action mDelJudgeMiss;
       
       private int mIFirstObjectIdx = 0;
       private int mILastObjectIdx = -1;

       private void Awake()
       {
              mListNotesPool = new List<NoteObject>();
              for (int i = 0; i < Global.SANYEAH_NOTE_POOLING_CNT ; i++)
              {
                     MakeNoteObject();
              }
       }

       public void SetDelJudgeMiss(Action del)
       {
              mDelJudgeMiss = del;
       }

       private void MakeNoteObject()
       {
              NoteObject obj = Instantiate(mNotePrefab, mTmParent);
              obj.Init(mESide,mRTEndPoint,mRTStartPoint, delegate
              {
                     // 활성화 되어있는 Object중 제일 앞에 있는 Object Index 판별
                     mIFirstObjectIdx = mIFirstObjectIdx + 1 == mListNotesPool.Count ? 0 : mIFirstObjectIdx + 1;

              }, delegate
              {
                     // 활성화 되어있는 Object중 제일 뒤에 있는 Object Index 판별
                     mILastObjectIdx = mILastObjectIdx + 1 == mListNotesPool.Count ? 0 : mILastObjectIdx + 1; 
              }, delegate
              {
                     mDelJudgeMiss();
              });
              
              mListNotesPool.Add(obj);

       }

       public void DropNoteObject()
       {
              int iNextIdx = mILastObjectIdx + 1 == mListNotesPool.Count ? 0 : mILastObjectIdx + 1;
              mListNotesPool[iNextIdx].SetShow(true);
       }

       public float GetJudgeDistance()
       {
              return mIFirstObjectIdx==-1? -1 : mListNotesPool[mIFirstObjectIdx].JudgePosX;
       }

       public void ForceAllNoteStop()
       {
              foreach (var note in mListNotesPool)
              {
                     if(note.IsShow)
                            note.SetShow(false);
              }
       }

}
