using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; // For using Array.Find()
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioMixer mixer = null;
    [SerializeField]
    private AudioMixerGroup mixerGroup = null;
    [SerializeField]
    private Sound[] sounds = null;

    private float globalVolume = 1f;
    private bool  mute = false;

    public static AudioManager instance;

    private void Awake()
    {
        // Singleton Thing
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        // Adds an AudioSource component for each Sound
        foreach (Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;

            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.outputAudioMixerGroup = mixerGroup;
        }
    }

    public void Play(SoundClips name)
    {
        if(mute)    return;

        // Finds the Sound with the specified Name
        Sound s = Array.Find(sounds, sound => sound.name == name);

        // Checks if the Sound is null, to avoid errors
        if (s == null)
        {
            print("Sound: [" + name + "] not found!");
            return;
        }

        // Actually plays the Sound
        s.source.PlayOneShot(s.clip, s.volume * globalVolume); /// doesn't cut off other sounds
        /// s.source.Play();

    }


    public void ToggleMute()
    {
        mute = !mute;
        print("mute: " + mute);

        float volume = mute? -80f : 0f;
        mixer.SetFloat("volume", volume);
    }
}
