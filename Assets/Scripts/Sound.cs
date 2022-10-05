using UnityEngine.Audio;
using UnityEngine;


namespace AudioManagement
{
    public enum SoundType
    {
        Music,
        SFX
    }

    [System.Serializable, CreateAssetMenu(fileName = "New Sound", menuName = "Audio/Sound")]
    public class Sound : ScriptableObject
    {
        // public new string name;
        public AudioClip clip;

        private float volume = 1f;
        [Range(0.0f, 10.0f)] public float volumeMultiplier = 1f;
        public float Volume { get { return volume * volumeMultiplier; } }

        [Range(0.1f, 3.0f)] public float pitch = 1f;
        public bool loop = false;
        public bool playOnAwake = true;
        public SoundType soundType;
        [HideInInspector] public AudioSource source;
    }
}