using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip mAudioClip;
    private AudioSource src;
    void Start()
    {
        if (mAudioClip != null)
        {
            src = gameObject.AddComponent<AudioSource>();
            src.clip = mAudioClip;
            src.Play();
        }

    }

}
