using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the sound effects for the game.
/// Uses a singleton pattern to ensure only one instance exists.
/// </summary>
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private AudioSource sfxSource;
    
    [Header("Audio Events")]
    [SerializeField] private List<AudioEvent> availableSounds;
    
    private Dictionary<string, AudioEvent> soundDictionary = new Dictionary<string, AudioEvent>();

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            
            // Initialize sound dictionary
            foreach (var sound in availableSounds)
            {
                soundDictionary.Add(sound.name, sound);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void PlaySFX(string soundName)
    {
        if (soundDictionary.TryGetValue(soundName, out AudioEvent audioEvent))
        {
            audioEvent.Play(sfxSource);
        }
        else
        {
            Debug.LogWarning($"Sound {soundName} not found!");
        }
    }

    public void StopSFX()
    {
        sfxSource.Stop();
    }
} 