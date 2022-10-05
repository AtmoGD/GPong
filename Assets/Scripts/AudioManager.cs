using UnityEngine.Audio;
using UnityEngine;
using System.Collections.Generic;

/*
THANK YOU BRACKEYS
*/

namespace AudioManagement
{
    public class AudioManager : MonoBehaviour
    {
        private static AudioManager instance;
        public static AudioManager Instance { get { return instance; } }
        public List<Sound> sounds = new List<Sound>();

        void Awake()
        {
            if (AudioManager.instance != null)
            {
                Destroy(gameObject);
                Debug.LogError("There can only be one AudioManager");
                return;
            }

            DontDestroyOnLoad(gameObject);

            AudioManager.instance = this;

            foreach (Sound sound in sounds)
            {
                sound.source = gameObject.AddComponent<AudioSource>();
                sound.source.clip = sound.clip;
                sound.source.volume = sound.Volume;
                sound.source.pitch = sound.pitch;
                sound.source.loop = sound.loop;
                if (sound.playOnAwake)
                    sound.source.Play();
            }
        }

        public void Play(string _name)
        {
            Sound sound = sounds.Find(sounds => sounds.name == _name);
            if (sound != null)
                sound.source.Play();
            else
                Debug.LogError("No sound with name " + _name + " exists.");
        }

        public void ChangeVolume(string _name, float _volume)
        {
            Sound sound = sounds.Find(sounds => sounds.name == _name);
            if (sound != null)
                sound.source.volume = _volume;
            else
                Debug.LogError("No sound with name " + _name + " exists.");
        }

        public void ChangeVolume(SoundType _soundType, float _volume)
        {
            sounds.FindAll(sounds => sounds.soundType == _soundType)
                .ForEach(sound => sound.source.volume = _volume);
        }
    }
}