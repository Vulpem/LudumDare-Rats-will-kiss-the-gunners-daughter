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

    float currentVolume;
    float desiredVolume;
    bool changing = false;
    float changeSpeed;

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
    }

    void Update()
    {
        if(changing)
        {
            currentVolume += changeSpeed * Time.deltaTime;
            if(Mathf.Abs(currentVolume - desiredVolume) < Mathf.Abs(changeSpeed))
            {
                currentVolume = desiredVolume;
                changing = false;
            }
            foreach(AudioSource src in musics)
            {
                src.volume = currentVolume;
            }
        }
    }

    public void ChangeMusicVolume(float _volume)
    {
        desiredVolume = _volume;
        changing = true;
        changeSpeed = (_volume - currentVolume) / 2.0f;
    }

    public void PlaySound(SOUNDS sound)
    {
        AudioSource toPlay = null;
        foreach (AudioSource source in sources)
        {
            if (source.isPlaying == false)
            {
                toPlay = source;
                break;
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
