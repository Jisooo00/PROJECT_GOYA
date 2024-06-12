using System;
using UnityEngine;
using UnityEngine.UI;

public class UITempTweenGauge : MonoBehaviour
{
    public Image mImg;
    public Color mClr1;
    public Color mClr2;

    private bool bFlag = true;
    private float mTime = 0;
    private void Update()
    {
        mTime += Time.deltaTime;
        if (mImg.fillAmount < 0.05f)
            return;
        
        if (mTime > 1f)
        {
            if (bFlag)
                mImg.color = mClr2;
            else
            {
                mImg.color = mClr1;
            }

            bFlag = !bFlag;
        }

    }
}