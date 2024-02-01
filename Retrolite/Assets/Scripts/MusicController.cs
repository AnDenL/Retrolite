using System.Collections;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    private float volume;
    private int lastSong = 0;
    public AudioSource[] music;

    public void ChangeMusic(int i) => StartCoroutine(Change(i));

    private IEnumerator Change(int musicPlayed)
    {
        if(lastSong != musicPlayed)
        {
            volume = 0;
            music[musicPlayed].enabled = true;
            while (volume > 0)
            {
                volume += Time.deltaTime / 2;
                music[lastSong].volume = 1 - volume;
                music[musicPlayed].volume = volume;
                yield return null;
            }
            music[lastSong].enabled = false;
            lastSong = musicPlayed;
        }
    }
}
