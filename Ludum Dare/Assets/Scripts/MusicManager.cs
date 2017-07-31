using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SOUNDS
{
    door,
    bell,
    paper,
    coin,
    click,
    music,
    bgSound
}

public class MusicManager : MonoBehaviour {

    public AudioClip door;
    public AudioClip bell;
    public AudioClip paper;
    public AudioClip coin;
    public AudioClip click;
    public AudioClip music;
    public AudioClip bgSound;

    List<AudioSource> sources;

    List<AudioSource> musics;

    public float musicMaxVolume = 0.5f;
    public float bgMaxVolume = 0.3f;

    public float currentVolume;
    float desiredVolume;
    bool changing = false;
    float changeSpeed;

    void Awake()
    {
        sources = new List<AudioSource>();
        musics = new List<AudioSource>();
    }

    public void RestartMusic()
    {
        if (musics.Count > 0)
        {
            musics[0].Stop();
            musics[0].Play();
        }
    }

    void Start()
    {
        AudioSource[] src = gameObject.GetComponents<AudioSource>();
        if (src.Length > 0)
        {
            foreach(AudioSource s in src)
            {
                sources.Add(s);
            }
        }

        if (music != null)
        {
            AudioSource m1 = gameObject.AddComponent<AudioSource>();
            m1.clip = music;
            m1.loop = true;
            m1.Play();
            musics.Add(m1);
        }

        if (bgSound != null)
        {
            AudioSource m2 = gameObject.AddComponent<AudioSource>();
            m2.clip = bgSound;
            m2.loop = true;
            m2.Play();
            musics.Add(m2);
        }

        currentVolume = musicMaxVolume;
        musics[0].volume = Mathf.Min(musicMaxVolume, currentVolume);
        musics[1].volume = Mathf.Min(bgMaxVolume, currentVolume);
    }

    void Update()
    {
        if(changing && musics.Count > 0)
        {
            currentVolume += changeSpeed * Time.deltaTime;
            if(Mathf.Abs(currentVolume - desiredVolume) < Mathf.Abs(changeSpeed))
            {
                currentVolume = desiredVolume;
                changing = false;
            }

            musics[0].volume = Mathf.Min(musicMaxVolume, currentVolume);
            musics[1].volume = Mathf.Min(bgMaxVolume, currentVolume);
        }
    }

    public void ChangeMusicVolume(float _volume)
    {

        desiredVolume = Mathf.Min(_volume, musicMaxVolume);
        changing = true;
        changeSpeed = (_volume - currentVolume) / 2.0f;
    }

    public void PlaySound(SOUNDS sound)
    {
        AudioSource toPlay = null;
        if (sources.Count > 0)
        {
            foreach (AudioSource source in sources)
            {
                if (source.isPlaying == false)
                {
                    toPlay = source;
                    break;
                }
            }
        }
        if (toPlay == null)
        {
            toPlay = gameObject.AddComponent<AudioSource>();
            sources.Add(toPlay);
        }

        switch (sound)
        {
            case SOUNDS.door: if(door != null) toPlay.PlayOneShot(door); break;
            case SOUNDS.bell: if (bell != null) toPlay.PlayOneShot(bell); break;
            case SOUNDS.paper: if (paper != null) toPlay.PlayOneShot(paper); break;
            case SOUNDS.coin: if (coin != null) toPlay.PlayOneShot(coin); break;
            case SOUNDS.click: if (click != null) toPlay.PlayOneShot(click); break;
        }
    }

}
