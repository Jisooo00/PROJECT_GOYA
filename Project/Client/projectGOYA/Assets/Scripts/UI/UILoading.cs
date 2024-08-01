using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILoading : MonoBehaviour
{
    public TMPro.TMP_Text mTxtLoading;
    private string text = "돗가비 이동중";
    void Start()
    {
        
    }

    private float fWaitTime = 0;
    private int cnt = 0;
    void Update()
    {
        fWaitTime += Time.deltaTime;
        if (fWaitTime > 0.3f)
        {
            fWaitTime = 0f;
            string dot = "";
            for (int i = 0; i < cnt; i++)
            {
                dot += ".";
            }
            mTxtLoading.text = text + dot;

            cnt = cnt >= 3 ? 0 : cnt + 1;
        }
    }
}
