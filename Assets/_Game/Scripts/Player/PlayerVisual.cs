using UnityEngine;

namespace RichRun
{
    /// <summary>
    /// Всё, что видно у персонажа: аниматор, комплект одежды по ступени богатства,
    /// эффекты апгрейда и звуки шагов. Геймплей сюда не заглядывает.
    /// </summary>
    public class PlayerVisual : MonoBehaviour
    {
        /// <summary>Набор шагов для одной ступени: кроссовки внизу, каблуки наверху.</summary>
        [System.Serializable]
        public class FootstepSet
        {
            public AudioClip[] Clips;
        }

        [SerializeField] Animator animator;

        [Header("Одежда по ступеням (индекс = индекс ступени)")]
        [Tooltip("Объекты с SkinnedMeshRenderer на ОДНОМ скелете. Активен ровно один.")]
        [SerializeField] GameObject[] outfits;

        [Header("Походка по ступеням (опционально, индекс = индекс ступени)")]
        [Tooltip("AnimatorOverrideController на каждую ступень: от обычного бега до подиумного шага.")]
        [SerializeField] RuntimeAnimatorController[] tierControllers;

        [Header("Эффекты")]
        [SerializeField] ParticleSystem tierUpVfx;
        [SerializeField] AudioClip tierUpSfx;
        [Tooltip("Индекс = индекс ступени. Если ступеней больше — берётся последний набор.")]
        [SerializeField] FootstepSet[] footstepsByTier;

        static readonly int SpeedHash = Animator.StringToHash("Speed");
        static readonly int TierHash = Animator.StringToHash("Tier");
        static readonly int HitHash = Animator.StringToHash("Hit");
        static readonly int WinHash = Animator.StringToHash("Win");

        GameManager _game;
        int _tier;
        int _step;

        void Start()
        {
            _game = GameManager.Instance;
            _game.StateChanged += OnStateChanged;
            _game.TierChanged += OnTierChanged;

            ApplyTier(_game.TierIndex, false);
            OnStateChanged(_game.State);
        }

        void OnDestroy()
        {
            if (_game == null) return;
            _game.StateChanged -= OnStateChanged;
            _game.TierChanged -= OnTierChanged;
        }

        void OnStateChanged(GameState state)
        {
            // Аниматора может не быть: геймплей проверяется на капсуле до появления модели.
            if (!animator) return;
            animator.SetFloat(SpeedHash, state == GameState.Running ? 1f : 0f);
            if (state == GameState.Finished) animator.SetTrigger(WinHash);
        }

        void OnTierChanged(int tier) => ApplyTier(tier, true);

        void ApplyTier(int tier, bool celebrate)
        {
            _tier = tier;
            for (int i = 0; i < outfits.Length; i++)
                if (outfits[i]) outfits[i].SetActive(i == tier);

            SwapController(tier);
            if (animator) animator.SetInteger(TierHash, tier);

            if (!celebrate) return;
            if (tierUpVfx) Vfx.Play(tierUpVfx, transform.position);
            Sfx.Play(tierUpSfx);
        }

        /// <summary>Смена контроллера без рывка: восстанавливаем фазу текущего состояния.</summary>
        void SwapController(int tier)
        {
            if (!animator || tierControllers == null || tier >= tierControllers.Length) return;
            var next = tierControllers[tier];
            if (!next || next == animator.runtimeAnimatorController) return;

            var state = animator.GetCurrentAnimatorStateInfo(0);
            animator.runtimeAnimatorController = next;
            animator.Play(state.shortNameHash, 0, state.normalizedTime % 1f);
        }

        public void PlayHit()
        {
            if (animator) animator.SetTrigger(HitHash);
        }

        /// <summary>Вызывается Animation Event'ом на кадрах касания стопы.</summary>
        public void Step()
        {
            if (footstepsByTier.Length == 0) return;

            var clips = footstepsByTier[Mathf.Min(_tier, footstepsByTier.Length - 1)].Clips;
            if (clips.Length == 0) return;

            _step = (_step + 1) % clips.Length;
            Sfx.Play(clips[_step], 0.5f);
        }
    }
}
