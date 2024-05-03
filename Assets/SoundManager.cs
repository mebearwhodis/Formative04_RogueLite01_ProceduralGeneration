using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [Serializable]
    public struct Sound
    {
        public string name;
        public AudioClip clip;
    }
    public List<Sound> sounds;
    
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _sfxSource;
    [SerializeField] private AudioClip _backgroundMusic;

    private void Start()
    {
        if (_backgroundMusic != null)
        {
            PlayBackgroundMusic();
        }
    }

    private void PlayBackgroundMusic()
    {
        if (_backgroundMusic != null)
        {
            _musicSource.clip = _backgroundMusic;
            _musicSource.loop = true;
            _musicSource.Play();
        }
    }
    
    // Play a sound by name
    public void PlaySound(string soundName)
    {
        Sound sound = sounds.Find(s => s.name == soundName);
        if (sound.clip != null)
        {
            _sfxSource.PlayOneShot(sound.clip);
        }
        else
        {
            Debug.LogWarning("Sound not found: " + soundName);
        }
    }
}
