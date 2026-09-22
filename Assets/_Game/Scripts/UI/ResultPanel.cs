using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RichRun
{
    /// <summary>
    /// Экран победы: награда = богатство × множитель дома.
    /// Кнопка с рекламой умножает её ещё раз по случайному сектору колеса.
    /// </summary>
    public class ResultPanel : MonoBehaviour
    {
        [SerializeField] GameObject root;
        [SerializeField] float showDelay = 1.2f;

        [Header("Тексты")]
        [SerializeField] TMP_Text titleText;
        [SerializeField] TMP_Text rewardText;
        [SerializeField] TMP_Text bonusRewardText;
        [SerializeField] TMP_Text bonusLabel;

        [Header("Колесо бонуса")]
        [SerializeField] int[] wheelMultipliers = { 2, 3, 4, 5 };
        [SerializeField] RectTransform wheelNeedle;
        [SerializeField] float[] wheelAngles = { 60f, 20f, -20f, -60f };

        [Header("Кнопки")]
        [SerializeField] Button claimButton;
        [SerializeField] Button bonusButton;
        [SerializeField] AudioClip winSfx;

        GameManager _game;
        float _timer = -1f;
        int _bonus = 2;

        void Start()
        {
            _game = GameManager.Instance;
            _game.StateChanged += OnStateChanged;

            root.SetActive(false);
            claimButton.onClick.AddListener(() => _game.Claim());
            bonusButton.onClick.AddListener(() => _game.Claim(_bonus));
        }

        void OnDestroy()
        {
            if (_game != null) _game.StateChanged -= OnStateChanged;
        }

        void OnStateChanged(GameState state)
        {
            if (state == GameState.Finished) _timer = showDelay;
        }

        void Update()
        {
            if (_timer < 0f) return;
            _timer -= Time.deltaTime;
            if (_timer > 0f) return;

            _timer = -1f;
            Show();
        }

        void Show()
        {
            int index = Random.Range(0, wheelMultipliers.Length);
            _bonus = wheelMultipliers[index];

            if (wheelNeedle && index < wheelAngles.Length)
                wheelNeedle.localRotation = Quaternion.Euler(0f, 0f, wheelAngles[index]);

            titleText.SetText("Уровень {0}\nЗАВЕРШЕНО", _game.LevelNumber);
            rewardText.SetText("{0}", _game.Reward);
            bonusRewardText.SetText("{0}", _game.Reward * _bonus);
            if (bonusLabel) bonusLabel.SetText("ПОЛУЧИТЬ ×{0}", _bonus);

            root.SetActive(true);
            Sfx.Play(winSfx);
        }
    }
}
