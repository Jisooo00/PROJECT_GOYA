using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SanyeahGame : MonoBehaviour
{

    public GameObject mBeforeGame;
    public GameObject mInGame;
    public GameObject mEndGame;

    public Action mDelAfterGame;
    // Start is called before the first frame update
    void Start()
    {
        mBeforeGame.SetActive(true);
        mInGame.SetActive(false);
        mEndGame.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickTmpStart()
    {
        mBeforeGame.SetActive(false);
        mInGame.SetActive(false);
        mEndGame.SetActive(true);
    }

    public void OnClickTmpEnd()
    {
        mDelAfterGame();
        this.gameObject.SetActive(false);
    }
    
}
