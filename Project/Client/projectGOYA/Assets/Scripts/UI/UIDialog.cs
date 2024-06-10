using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDialog : MonoBehaviour
{
    [SerializeField] private TMP_Text mTextScript;
    [SerializeField] private TMP_Text mTextName;
    [SerializeField] private Image mImgPortrait;

    private int mIndex = 0;
    private KeyValuePair<string,string>[] mListDialog;
    private Action mDelDialogAfter;
    private GameData.DialogData mData;
    private Dictionary<string, Sprite> mDicPortraits;
    
    public void Init(GameData.DialogData data, Action delDialogAfter)
    {
        mIndex = 0;
        if (data.mListDialog.Length== 0)
        {
            delDialogAfter();
        }
        else
        {
            mData = data;
            mListDialog = data.mListDialog;
            mDelDialogAfter = delDialogAfter;
            mDicPortraits = new Dictionary<string, Sprite>();
            LoadSpritePortraits();
            SetDialog();
            
            
        }
    }

    public void OnClick()
    {
        mIndex++;
        if (mIndex >= mListDialog.Length)
        {
            mDelDialogAfter();
        }
        else
        {
            SetDialog();
        }
    }

    public void SetDialog()
    {
        mTextScript.text = mData.mListDialog[mIndex].Value;
        mTextName.text = mData.mListDialog[mIndex].Key.Substring(0,mData.mListDialog[mIndex].Key.Length-3);
        if(mDicPortraits.ContainsKey(mData.mListDialog[mIndex].Key))
            mImgPortrait.sprite = mDicPortraits[mData.mListDialog[mIndex].Key];
    }
    
    private void LoadSpritePortraits()
    {
        string spriteFolder = "Portraits/";
        string spriteFilepath = spriteFolder + "img_portrait_";
        foreach (var kv in mListDialog)
        {
            string spriteFileName = kv.Key;
            if(mDicPortraits.ContainsKey(spriteFileName))
                continue;
            
            spriteFilepath += spriteFileName;
            var sprite = Resources.Load<Sprite>(spriteFilepath);
            if (sprite == null)
            {
                Debug.Log("File Load Fail. " +spriteFileName);
                spriteFilepath = spriteFolder + "img_portrait_"+spriteFileName.Substring(0,spriteFileName.Length-2)+"01";
                sprite = Resources.Load<Sprite>(spriteFilepath);
            }
            mDicPortraits.Add(spriteFileName,sprite);
        }
    }

}
