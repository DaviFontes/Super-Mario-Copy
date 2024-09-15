using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

	public static AudioManager instance;

	public float effectsVolume = 1f; 
	public Sound[] sounds;
	private Sound music;
	private bool isPlaying = false; 

	void Awake()
	{
		if (instance != null)
		{
			Destroy(gameObject);
		}
		else
		{
			instance = this;			
		}

		DontDestroyOnLoad(gameObject);

		foreach (Sound s in sounds)
		{
			s.source = gameObject.AddComponent<AudioSource>();
			s.source.clip = s.clip;
			s.source.loop = s.loop;
		}
	}
		
	public void SetVolume(float value)
	{
		effectsVolume = value;
		if(music != null)
		{
			music.source.volume = value;
		}
	}

    public void PlayMusic(string sound)
    {
		if (isPlaying) music.source.Stop();

        music = Array.Find(sounds, item => item.name == sound);
        if (music == null)
        {
            return;
        }

        music.source.volume = music.volume * effectsVolume;
        music.source.pitch = music.pitch;

        music.source.Play();
		isPlaying = true;
    }

	public void PauseMusic()
	{
		music.source.Pause();
	}

    public void ResumeMusic()
    {
        music.source.Play();
    }

    public void Play(string sound)
	{
		Sound s = Array.Find(sounds, item => item.name == sound);
		if (s == null)
		{
			return;
		}

		s.source.volume = s.volume * effectsVolume;
		s.source.pitch = s.pitch;

		s.source.Play();
	}

}
