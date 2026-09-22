using TMPro;
using UnityEngine;

namespace RichRun
{
    /// <summary>
    /// Дом-чекпоинт «×N». Триггер вытянут назад по Z, поэтому решение принимается
    /// на подходе и створки успевают распахнуться до того, как игрок добежит.
    /// Не хватило богатства — створки остаются закрытыми, игрок упирается в них,
    /// и только там забег заканчивается.
    /// </summary>
    public class MultiplierGate : MonoBehaviour, ITrackTrigger
    {
        [SerializeField] int requiredWealth = 80;
        [SerializeField] int multiplier = 2;

        [Header("Створки")]
        [SerializeField] Transform leftLeaf;
        [SerializeField] Transform rightLeaf;
        [SerializeField] float openAngle = 100f;
        [SerializeField] float openDuration = 0.35f;
        [Tooltip("За сколько метров до плоскости ворот игрок упирается в закрытые створки.")]
        [SerializeField] float stopMargin = 1f;

        [Header("Вид и звук")]
        [SerializeField] TMP_Text label;
        [SerializeField] ParticleSystem passVfx;
        [SerializeField] AudioClip passSfx;
        [SerializeField] AudioClip blockSfx;
        [SerializeField] Collider trigger;

        public int RequiredWealth => requiredWealth;

        Quaternion _leftClosed, _leftOpen;
        Quaternion _rightClosed, _rightOpen;
        Transform _player;
        float _open = -1f;      // прогресс распахивания; <0 — створки стоят
        bool _blocking;

        void Awake()
        {
            if (label) label.SetText("×{0}", multiplier);

            if (leftLeaf)
            {
                _leftClosed = leftLeaf.localRotation;
                _leftOpen = _leftClosed * Quaternion.Euler(0f, -openAngle, 0f);
            }
            if (rightLeaf)
            {
                _rightClosed = rightLeaf.localRotation;
                _rightOpen = _rightClosed * Quaternion.Euler(0f, openAngle, 0f);
            }
        }

        public void OnPlayerEnter(PlayerController player)
        {
            if (trigger) trigger.enabled = false;

            var game = GameManager.Instance;
            if (game.Wealth >= requiredWealth)
            {
                game.SetMultiplier(multiplier);
                if (passVfx) Vfx.Play(passVfx, transform.position);
                Sfx.Play(passSfx);
                _open = 0f;
            }
            else
            {
                _player = player.transform;
                _blocking = true;
            }
        }

        void Update()
        {
            if (_blocking) TickBlock();
            if (_open >= 0f) TickOpen();
        }

        /// <summary>Ждём, пока игрок упрётся в створки — обрывать забег на входе в триггер рано.</summary>
        void TickBlock()
        {
            if (_player.position.z < transform.position.z - stopMargin) return;

            _blocking = false;
            Sfx.Play(blockSfx);

            var game = GameManager.Instance;
            game.Player.Visual.PlayHit();
            game.Finish();
            enabled = false;
        }

        void TickOpen()
        {
            _open += Time.deltaTime / openDuration;
            float k = _open >= 1f ? 1f : _open * _open * (3f - 2f * _open);   // smoothstep

            if (leftLeaf) leftLeaf.localRotation = Quaternion.Slerp(_leftClosed, _leftOpen, k);
            if (rightLeaf) rightLeaf.localRotation = Quaternion.Slerp(_rightClosed, _rightOpen, k);

            if (_open < 1f) return;
            _open = -1f;
            enabled = false;   // отработали — Update больше не нужен
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            if (label) label.SetText("×{0}", multiplier);
        }
#endif
    }
}
