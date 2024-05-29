using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDialog : MonoBehaviour
{
    [SerializeField] private TMP_Text mText;
    private int mIndex = 0;
    private string[] mListDialog;
    private Action mDelDialogAfter;
    
    public void Init(GameData.DialogData data, Action delDialogAfter)
    {
        mIndex = 0;
        if (data.mListDialog.Length== 0)
        {
            delDialogAfter();
        }
        else
        {
            mListDialog = data.mListDialog;
            mText.text = data.mListDialog[0];
            mDelDialogAfter = delDialogAfter;
        }
    }

    public void OnClick()
    {
        if (mIndex >= mListDialog.Length)
        {
            mDelDialogAfter();
        }
        else
        {
            mText.text = mListDialog[mIndex++];
        }
    }

}
