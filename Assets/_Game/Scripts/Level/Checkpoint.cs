using UnityEngine;

namespace RichRun
{
    /// <summary>
    /// Рубеж с флагштоками. Флаги авторятся в префабе стоя, скрипт кладёт их
    /// на старте и поднимает, когда игрок пересекает полосу: по очереди,
    /// с перелётом через вертикаль — отсюда пружинка, как в оригинале.
    /// </summary>
    public class Checkpoint : MonoBehaviour, ITrackTrigger
    {
        [SerializeField] Transform[] flags;

        [Header("Подъём")]
        [Tooltip("Ось наклона в собственных осях флага. (0,0,1) — наклон по Z поверх " +
                 "авторского разворота: флаг с Rotation Y = 180 ляжет как (0, 180, 90).")]
        [SerializeField] Vector3 fallAxis = Vector3.forward;
        [Tooltip("Наклон в лежачем положении. Падают не в ту сторону — смени знак.")]
        [SerializeField] float downAngle = 90f;
        [SerializeField] float liftDuration = 0.45f;
        [Tooltip("Задержка между соседними флагами, секунды.")]
        [SerializeField] float stagger = 0.08f;
        [Tooltip("Сила перелёта через вертикаль. 0 — подъём без пружинки.")]
        [SerializeField] float overshoot = 1.7f;

        [Header("Эффекты")]
        [SerializeField] ParticleSystem vfx;
        [SerializeField] AudioClip sfx;
        [SerializeField] Collider trigger;

        Quaternion[] _up;
        Quaternion[] _down;
        float _time = -1f;
        float _total;

        void Awake()
        {
            int count = flags.Length;
            _up = new Quaternion[count];
            _down = new Quaternion[count];

            // Домножаем СПРАВА: наклон ложится поверх собственного разворота флага.
            // Флаг с Rotation Y = 180 получает (0, 180, 90), а не (180, 0, 90) —
            // авторская настройка остаётся на месте, а штоки валятся зеркально.
            var fall = Quaternion.AngleAxis(downAngle, fallAxis.normalized);

            for (int i = 0; i < count; i++)
            {
                _up[i] = flags[i].localRotation;
                _down[i] = _up[i] * fall;
                flags[i].localRotation = _down[i];
            }

            _total = liftDuration + stagger * Mathf.Max(0, count - 1);
        }

        public void OnPlayerEnter(PlayerController player)
        {
            if (trigger) trigger.enabled = false;
            if (_time >= 0f) return;

            _time = 0f;
            if (vfx) Vfx.Play(vfx, transform.position);
            Sfx.Play(sfx);
        }

        void Update()
        {
            if (_time < 0f) return;

            _time += Time.deltaTime;

            for (int i = 0; i < flags.Length; i++)
            {
                float p = Mathf.Clamp01((_time - stagger * i) / liftDuration);
                flags[i].localRotation = Quaternion.SlerpUnclamped(_down[i], _up[i], BackOut(p));
            }

            if (_time < _total) return;

            for (int i = 0; i < flags.Length; i++) flags[i].localRotation = _up[i];
            _time = -1f;
            enabled = false;   // отработали — Update больше не нужен
        }

        /// <summary>Ease-out-back: проскакивает 1 и возвращается. При overshoot = 0 — обычное замедление.</summary>
        float BackOut(float p)
        {
            float s = p - 1f;
            return 1f + s * s * ((overshoot + 1f) * s + overshoot);
        }
    }
}
