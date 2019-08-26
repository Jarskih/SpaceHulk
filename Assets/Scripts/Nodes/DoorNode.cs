using System.Collections;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DoorNode : MonoBehaviour, IOpenable, IWalkable
{
    public RandomAudioEvent doorClose;
    public RandomAudioEvent doorOpen;
    private SpriteRenderer _spriteRenderer;
    private AudioSource _audioSource;
    private bool _isOpen;
    
    public Unit unitOnNode;
    
    public bool isOpen => _isOpen;
    // Start is called before the first frame update
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _audioSource = GetComponent<AudioSource>();
    }

    public void Open()
    {
        if (_isOpen) return;
        _isOpen = true;
        _spriteRenderer.enabled = false;
        doorOpen.Play(_audioSource);
    }

    public void Close()
    {
        if (!_isOpen) return;
        _isOpen = false;
        _spriteRenderer.enabled = true;
        doorClose.Play(_audioSource);
    }

    public void Walk()
    {
        throw new System.NotImplementedException();
    }
}
