using UnityEngine;

namespace RichRun
{
    /// <summary>
    /// Один AudioSource на всю игру. PlayOneShot не создаёт объектов,
    /// в отличие от AudioSource.PlayClipAtPoint.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    [DefaultExecutionOrder(-50)]
    public class Sfx : MonoBehaviour
    {
        static Sfx _instance;
        AudioSource _source;

        void Awake()
        {
            _instance = this;
            _source = GetComponent<AudioSource>();
            _source.playOnAwake = false;
        }

        void OnDestroy()
        {
            if (_instance == this) _instance = null;
        }

        public static void Play(AudioClip clip, float volume = 1f)
        {
            if (clip && _instance != null) _instance._source.PlayOneShot(clip, volume);
        }
    }
}
