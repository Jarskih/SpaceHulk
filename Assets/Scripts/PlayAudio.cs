using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudio : MonoBehaviour
{
    [SerializeField]
    private string eventName;
    private AudioSource audioSource;
    public RandomAudioEvent sound;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    
    void OnEnable()
    {
        EventManager.StartListening(eventName, PlaySound);
    }

    void OnDisable() {
        EventManager.StopListening(eventName, PlaySound);
    }

    private void PlaySound()
    {
        sound.Play(audioSource);   
    }
}
