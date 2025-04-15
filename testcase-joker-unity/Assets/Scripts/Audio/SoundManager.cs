using System.Collections.Generic;
using Singleton;
using UnityEngine;

namespace Audio
{
    /// <summary>
    /// Simple sound manager that plays audio clips directly.
    /// </summary>
    public class SoundManager : Singleton<SoundManager>
    {
        [SerializeField] private AudioSource sfxSource;
    
        [Header("Audio Clips")]
        [SerializeField] private List<AudioClip> audioClips;
    
        private Dictionary<string, AudioClip> soundDictionary = new Dictionary<string, AudioClip>();

        protected override void Awake()
        {
            base.Awake();
            InitializeSounds();
        }

        private void InitializeSounds()
        {
            foreach (var clip in audioClips)
            {
                if (clip != null)
                {
                    soundDictionary[clip.name] = clip;
                }
            }
        }

        public void PlaySound(string soundName, float volume = 1f, float pitch = 1f)
        {
            if (soundDictionary.TryGetValue(soundName, out AudioClip clip))
            {
                sfxSource.pitch = pitch;
                sfxSource.volume = volume;
                sfxSource.clip = clip;
                sfxSource.Play();
            }
            else
            {
                Debug.LogWarning($"Sound {soundName} not found!");
            }
        }

        public void StopSound()
        {
            sfxSource.Stop();
        }
    }
} 