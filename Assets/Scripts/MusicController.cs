using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public AudioClip[] songs;

    private AudioSource player;
    private int index;

    void Start()
    {
        player = GetComponent<AudioSource>();
        index = 0;
        player.clip = songs[index];
        player.Play();
    }

    void Update()
    {
        if(!player.isPlaying)
        {
            index++;

            if (index >= songs.Length) index = 0;

            player.clip = songs[index];
            player.Play();
        }
    }
}
