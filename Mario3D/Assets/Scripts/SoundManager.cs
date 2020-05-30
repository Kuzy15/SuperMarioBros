using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private AudioSource audio;
    void Start()
    {
        audio = this.gameObject.GetComponent<AudioSource>();
        audio.Play();
    }
}
