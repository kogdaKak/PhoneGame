using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RichRun
{
    public enum GameState { Ready, Running, Finished }

    /// <summary>
    /// Владелец состояния забега: богатство, ступень, множитель, поток уровня.
    /// Никто, кроме него, не меняет эти значения — остальные слушают события.
    /// </summary>
    [DefaultExecutionOrder(-100)]
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] GameConfig config;
        [SerializeField] PlayerController player;

        [Header("Уровни")]
        [Tooltip("Объект со скриптом LevelManager. Он и спавнит префаб уровня.")]
        [SerializeField] ButchersGames.LevelManager levelManager;

        [Header("Отладка")]
        [Tooltip("Стартовать забег сразу по Play, без касания экрана.")]
        [SerializeField] bool autoStart;

        public GameConfig Config => config;
        public PlayerController Player => player;

        public GameState State { get; private set; } = GameState.Ready;
        public int Wealth { get; private set; }
        public int TierIndex { get; private set; }
        public int Multiplier { get; private set; } = 1;
        public int LevelNumber => Save.Level + 1;
        public int Reward => Wealth * Multiplier * config.CoinsPerWealth;

        public event Action<GameState> StateChanged;
        /// <summary>(итоговое богатство, дельта)</summary>
        public event Action<int, int> WealthChanged;
        public event Action<int> TierChanged;
        public event Action<int> MultiplierChanged;

        /// <summary>Заполненность полоски внутри текущей ступени, 0..1.</summary>
        public float TierProgress
        {
            get
            {
                var tiers = config.Tiers;
                if (TierIndex >= tiers.Length - 1) return 1f;
                int from = tiers[TierIndex].Threshold;
                int to = tiers[TierIndex + 1].Threshold;
                return to > from ? Mathf.Clamp01((Wealth - from) / (float)(to - from)) : 1f;
            }
        }

        public WealthTier CurrentTier => config.Tiers[TierIndex];

        void Awake()
        {
            Instance = this;
            Wealth = config.StartWealth;
            TierIndex = TierFor(Wealth);
        }

        // Уровень собираем в Start: к этому моменту Awake игрока уже отработал
        // и не затрёт точку старта. Порядок гарантирует DefaultExecutionOrder.
        void Start() => LoadLevel();

        void LoadLevel()
        {
            if (!levelManager)
            {
                Debug.LogError("[RichRun] GameManager: не назначен Level Manager.", this);
                return;
            }

            int count = levelManager.Levels.Count;
            if (count == 0)
            {
                Debug.LogError("[RichRun] В списке Lvls List нет уровней.", levelManager);
                return;
            }

            // Прогресс ведёт Save.Level: у пакета сеттер CurrentLevel сломан
            // (PlayerPrefs.GetInt вместо SetInt), и его счётчик не сохраняется.
            // indexCheck: false обходит GetCorrectedIndex, который на двух
            // и более уровнях возвращает не тот индекс.
            levelManager.SelectLevel(Save.Level % count, false);

            var info = levelManager.GetComponentInChildren<LevelInfo>();
            if (!info)
            {
                Debug.LogError("[RichRun] В уровне нет LevelInfo. Повесь его на корень префаба " +
                               "и заполни Player Start.", levelManager);
                return;
            }

            player.Place(info.PlayerStart,
                info.RoadHalfWidth > 0f ? info.RoadHalfWidth : config.RoadHalfWidth);
        }

        // --- поток игры -------------------------------------------------

        // Запуск живёт здесь, а не в UI: панель со стартом может отсутствовать,
        // а игра всё равно обязана начинаться по касанию.
        void Update()
        {
            if (State != GameState.Ready) return;
            if (autoStart || SwipeInput.Tapped) StartRun();
        }

        public void StartRun()
        {
            if (State != GameState.Ready) return;
            SetState(GameState.Running);
        }

        public void Finish()
        {
            if (State != GameState.Running) return;
            SetState(GameState.Finished);
        }

        /// <summary>Забрать награду и перейти к следующему уровню.</summary>
        public void Claim(int extraMultiplier = 1)
        {
            Save.Coins += Reward * Mathf.Max(1, extraMultiplier);
            Save.Level++;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        // --- экономика --------------------------------------------------

        public void AddWealth(int delta)
        {
            if (State != GameState.Running || delta == 0) return;

            Wealth = Mathf.Max(0, Wealth + delta);
            WealthChanged?.Invoke(Wealth, delta);

            int tier = TierFor(Wealth);
            if (tier == TierIndex) return;
            TierIndex = tier;
            TierChanged?.Invoke(tier);
        }

        public void SetMultiplier(int value)
        {
            if (value <= Multiplier) return;
            Multiplier = value;
            MultiplierChanged?.Invoke(value);
        }

        int TierFor(int wealth)
        {
            var tiers = config.Tiers;
            int result = 0;
            for (int i = 1; i < tiers.Length && wealth >= tiers[i].Threshold; i++) result = i;
            return result;
        }

        void SetState(GameState state)
        {
            State = state;
            StateChanged?.Invoke(state);
        }
    }
}
