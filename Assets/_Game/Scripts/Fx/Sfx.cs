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
        [Range(0f, 1f)]
        [SerializeField] float masterVolume = 1f;

        static Sfx _instance;
        AudioSource _source;

        void Awake()
        {
            _instance = this;
            _source = GetComponent<AudioSource>();
            _source.playOnAwake = false;
            _source.spatialBlend = 0f;   // 2D: громкость не падает с расстоянием
            _source.loop = false;
        }

        void OnDestroy()
        {
            if (_instance == this) _instance = null;
        }

        public static void Play(AudioClip clip, float volume = 1f)
        {
            if (!clip) return;

            // Раньше без компонента в сцене звук пропадал молча — и искать это
            // было нечем. Теперь поднимаем источник сами и предупреждаем.
            if (_instance == null) CreateFallback();

            _instance._source.PlayOneShot(clip, volume * _instance.masterVolume);
        }

        static void CreateFallback()
        {
            var host = new GameObject("Sfx (создан автоматически)");
            host.AddComponent<Sfx>();   // Awake проставит _instance и _source

            Debug.LogWarning("[RichRun] В сцене не было компонента Sfx — создал временный. " +
                             "Повесь Sfx на объект Systems, чтобы управлять громкостью.", host);
        }
    }
}
