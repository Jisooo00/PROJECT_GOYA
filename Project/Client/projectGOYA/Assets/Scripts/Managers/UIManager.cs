using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Button mBtnDialog;
    public List<GameObject> mListImage;
    [NonSerialized] public bool mIsDialogEnable = false;
    
    [SerializeField] private GameObject mActionUI;
    [SerializeField] private GameObject mDialogUI;
    [SerializeField] private UIVirtualJoyStick mJoyStick;
    public Button mBtnSetting;

    //public SanyeahGame mSanyeahGame;
    private UIDialog mDialogSystem;
    
    public GameObject m_goTutoPointer_dialog1 = null;
    public GameObject m_goTutoPointer_dialog2 = null;

    public Action delTuto = null;
    
    void Start()
    {
    }

    public void Init()
    {
        mActionUI.SetActive(true);
        mDialogUI.SetActive(false);
        SetDialogEnable(false);
        RefreshDialogBtn();
        if(mDialogUI!= null)
            mDialogSystem = mDialogUI.GetComponent<UIDialog>();
        mBtnSetting.onClick.AddListener(delegate
        {
            AudioManager.Instance.PlayClick();
            PopupManager.Instance.OpenPopupSetting(delegate
            {
                RefreshDialogBtn();
            });
            
        });
        
        m_goTutoPointer_dialog1.SetActive(false);
        m_goTutoPointer_dialog2.SetActive(false);
    }

    public void SetPrologScene()
    {
        mActionUI.SetActive(false);
        mDialogUI.SetActive(false);
    }

    public void SetTutorialAction()
    {
        Debug.Log("tutoAction");
        mActionUI.SetActive(true);
        mJoyStick.SetTutorialPointer();
        mDialogUI.SetActive(false);
        mBtnSetting.gameObject.SetActive(false);
    }

    
    // Update is called once per frame
    void Update()
    {
        var data = Player.instance.GetScannedMonster();
        if (data != null && !mIsDialogEnable)
        {
            SetDialogEnable(true,data.mObjectID.Contains("Obj"));
        }
        else if ((data == null) && mIsDialogEnable)
        {
            SetDialogEnable(false);
        }
    }

    public void RefreshDialogBtn()
    {
        if(!mIsDialogEnable)
            mBtnDialog.transform.parent.gameObject.SetActive(GameData.myData.IS_SHOW_UI_BTN);
    }
    public void DialogKeyOnClick()
    {
        AudioManager.Instance.PlayClick();
        if (delTuto != null)
        {
            delTuto();
            return;
        }

        Player.instance.bIsDialogPlaying = true;
        PlayDialog();
    }

    public void SetDialogEnable(bool bEnable,bool bObj = false)
    {
        mIsDialogEnable = bEnable;
        
        if (!bEnable)
        {
            foreach (var image in mListImage)
            {
                image.SetActive(false);   
            }
        }
        else
        {
            mListImage[0].SetActive(!bObj);
            mListImage[1].SetActive(bObj);
        }
        
        if(mBtnDialog != null)
            mBtnDialog.gameObject.SetActive(mIsDialogEnable);
        if (!GameData.myData.IS_SHOW_UI_BTN)
        {
            mBtnDialog.transform.parent.gameObject.SetActive(mIsDialogEnable);
        }
        m_goTutoPointer_dialog2.gameObject.SetActive(bEnable&&Player.instance.bIsOnGoingTutorial);
    }

    public void PlayDialog()
    {
        mActionUI.SetActive(false);
        mDialogUI.SetActive(true);
        if (mDialogSystem != null)
        {
            var monster = Player.instance.GetScannedMonster();
            if (Player.instance.GetScannedMonster() != null)
            {
                mDialogSystem.Init(monster, delegate { EndDialog(); });

            }

        }
    }
    
    public void PlayDialogForce(string DialogID, Action del)
    {
        Debug.Log("PlayDialogForce");
        mActionUI.SetActive(false);
        mDialogUI.SetActive(true);
        Player.instance.bIsDialogPlaying = true;
        if (mDialogSystem != null)
        {
            var data = SaveDataManager.Instance.GetDialogByID(DialogID);
            if (data == null)
            {
                Debug.Log("dialog "+DialogID+"is null");
                return;
            }
            
            mDialogSystem.Init(data, delegate { EndDialog();
                del();
            });
            
            m_goTutoPointer_dialog1.SetActive(Player.instance.bIsOnGoingTutorial);
        }
    }

    public void EndDialog()
    {
        mActionUI.SetActive(true);
        mDialogUI.SetActive(false);
        m_goTutoPointer_dialog1.SetActive(false);
            
        Player.instance.bIsDialogPlaying = false;
        
        if (Player.instance.mScanObject != null)
        {
            var data =Player.instance.mScanObject.GetComponent<MonsterBase>();
            data.RefreshData();
        }
        

    }

    public void SetDelTuto(Action del = null)
    {
        if(del != null)
            delTuto = del;
        else
            delTuto = null;
    }
    
    public void ForceJoystickPointerUp()
    {
        mJoyStick.ForcePointerUp();
    }

}

