using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    public AudioSource playerSource;

    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip loginClip;
    [SerializeField]
    private AudioClip townClip;
    [SerializeField]
    private AudioClip battleClip;

    private AudioClip currentClip;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (GameManager.isPlayGame)
        {
            UpdateAudioClip();
        }
    }
    private void UpdateAudioClip()
    {
        AudioClip newClip = null;

        if (GameManager.isTown && !GameManager.isBattle)
        {
            newClip = townClip;
        }
        else if (!GameManager.isTown && GameManager.isBattle)
        {
            newClip = battleClip;
        }

        // 새로운 클립이 현재 클립과 다르면 변경
        if (newClip != null && newClip != currentClip)
        {
            currentClip = newClip;
            audioSource.clip = currentClip;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    public void changeSound(float volume)
    {
        audioSource.volume = volume;

        if(playerSource != null)
        {
            playerSource.volume = volume;
        }
    }

}
