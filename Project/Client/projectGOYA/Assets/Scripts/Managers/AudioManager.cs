using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip mAudioBGMClip;
    [SerializeField] private AudioClip mFootstepClip;
    [SerializeField] private AudioClip mClickClip;
    [SerializeField] private AudioClip mSanyeahGameClip;
    [SerializeField] private AudioClip mEffectClip;
    [SerializeField] private AudioClip mEffectMissClip;
    [SerializeField] private AudioClip mEffectFeverClip;
    private AudioSource src_bgm;
    
    private static AudioManager s_inst;
    private static Func<AudioManager> s_inst_func = Load;
    
    private Dictionary<string, float> played_list = new Dictionary<string, float>();
    private List<AudioSource> doing = new List<AudioSource>();
    private List<AudioSource> wait = new List<AudioSource>();
    
    public static AudioManager Instance
    {
        get { return s_inst_func(); }
    }

    private static AudioManager Load()
    {
        var o = Resources.Load("Prefabs/UI/AudioManager");
        if (o == null)
        {
            Debug.LogError("AudioManager.Load fail");
            return null;
        }

        var go = GameObject.Instantiate(o);
        s_inst = go.GetComponent<AudioManager>();
        if (s_inst == null)
        {
            Debug.LogError("AudioManager.GetComponent fail");
            return null;
        }

        go.name = "=AudioManager";
        GameObject.DontDestroyOnLoad(go);

        s_inst_func = Get;
        return s_inst;
    }

    private static AudioManager Get()
    {
        return s_inst;
    }
    
    public void SetVolume()
    {
        src_bgm.volume = GameData.myData.SET_VOLUME;
    }
    
    public void SetVolumeCheck(float volume)
    {
        src_bgm.volume = volume;
    }
    void Awake()
    {
        src_bgm = gameObject.AddComponent<AudioSource>();
        src_bgm.clip = mAudioBGMClip;
        src_bgm.Play();
        src_bgm.Stop();
    }
    
    private void PlayClip(AudioClip c)
    {

        if(played_list.ContainsKey(c.name))
        {
            var dt = Time.realtimeSinceStartup - played_list[c.name];
            if (dt < 0.03f)
            {
                return;
            }
        }
        played_list[c.name] = Time.realtimeSinceStartup;


        if (doing.Count > 0)
        {
            if (!doing[0].isPlaying)
            {
                var s = doing[0];
                doing.RemoveAt(0);
                wait.Add(s);
            }
        }

        if (wait.Count < 1)
        {
            var s = gameObject.AddComponent<AudioSource>();
            wait.Add(s);
        }

        var src = wait[0];
        wait.RemoveAt(0);

        src.clip = c;
        src.Play();
        src.volume = 0.8f * GameData.myData.SET_VOLUME;
        doing.Add(src);
    }

    public void PlayClick()
    {
        if (!GameData.myData.IS_EFFECT_ON)
            return;
        PlayClip(mClickClip);
    }

    public void PlayFootstep()
    {
        if (!GameData.myData.IS_EFFECT_ON)
            return;
        PlayClip(mFootstepClip);
    }

    public void PlayBgm()
    {
        if (src_bgm.isPlaying)
            return;
        if (!GameData.myData.IS_BGM_ON)
            return;
        src_bgm.volume = GameData.myData.SET_VOLUME;
        src_bgm.loop = true;
        src_bgm.Play();
    }
    public void StopBgm()
    {
        if (!src_bgm.isPlaying)
            return;
        src_bgm.Stop();
    }

}
