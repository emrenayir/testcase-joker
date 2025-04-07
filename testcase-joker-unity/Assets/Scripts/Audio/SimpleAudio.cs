using System;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// A simple audio event that plays a single audio clip.
/// </summary>
[CreateAssetMenu(fileName = "NewSimpleAudio", menuName = "Audio/Simple Audio")]
public class SimpleAudio : AudioEvent
{
    public AudioClip clip;
    [Range(0,1)] public float volume = 1f;
    [Range(0.5f,1.5f)] public float pitch = 1f;
    [Range(0,1)] public float delay;
    
    public async override void Play(AudioSource audio)
    {
        // First apply the delay if any
        if (delay > 0)
        {
            await Task.Delay(TimeSpan.FromSeconds(delay));
        }
        
        // Then setup and play the audio
        audio.volume = volume;
        audio.pitch = pitch;
        audio.clip = clip;
        audio.Play();
    }
}