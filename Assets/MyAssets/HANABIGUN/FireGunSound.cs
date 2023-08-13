using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireGunSound : MonoBehaviour
{
    [SerializeField] UnityEngine.AudioSource audioSource;
    [SerializeField] AudioClip audioClip;

    void Start()
    {
        //sound = GetComponent<AudioSource>();
    }

    void Update()
    {
    }

    // Update is called once per frame
    void PlaySound()
    {
        audioSource.PlayOneShot(audioClip);
    }
}
