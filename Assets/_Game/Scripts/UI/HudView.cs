using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RichRun
{
    /// <summary>
    /// Верхняя панель и полоска богатства. Обновляется только по событиям —
    /// в Update здесь нет ничего.
    /// </summary>
    public class HudView : MonoBehaviour
    {
        [Header("Верх экрана")]
        [SerializeField] TMP_Text levelText;
        [SerializeField] TMP_Text coinsText;
        [SerializeField] TMP_Text wealthText;

        [Header("Полоска ступени")]
        [SerializeField] GameObject tierGroup;
        [SerializeField] TMP_Text tierTitle;
        [SerializeField] Image tierFill;

        [Header("Множитель")]
        [SerializeField] TMP_Text multiplierText;

        GameManager _game;

        void Start()
        {
            _game = GameManager.Instance;
            _game.StateChanged += OnStateChanged;
            _game.WealthChanged += OnWealthChanged;
            _game.TierChanged += OnTierChanged;
            _game.MultiplierChanged += OnMultiplierChanged;

            levelText.SetText("Уровень {0}", _game.LevelNumber);
            coinsText.SetText("{0}", Save.Coins);
            OnTierChanged(_game.TierIndex);
            OnWealthChanged(_game.Wealth, 0);
            OnMultiplierChanged(_game.Multiplier);
            OnStateChanged(_game.State);
        }

        void OnDestroy()
        {
            if (_game == null) return;
            _game.StateChanged -= OnStateChanged;
            _game.WealthChanged -= OnWealthChanged;
            _game.TierChanged -= OnTierChanged;
            _game.MultiplierChanged -= OnMultiplierChanged;
        }

        void OnStateChanged(GameState state)
        {
            bool running = state == GameState.Running;
            if (tierGroup) tierGroup.SetActive(running);
            if (wealthText) wealthText.gameObject.SetActive(running);
            if (levelText) levelText.gameObject.SetActive(running);
        }

        void OnWealthChanged(int wealth, int delta)
        {
            wealthText.SetText("{0}", wealth);
            tierFill.fillAmount = _game.TierProgress;
        }

        void OnTierChanged(int tier)
        {
            var data = _game.CurrentTier;
            tierTitle.SetText(data.Title);
            tierTitle.color = data.BarColor;
            tierFill.color = data.BarColor;
        }

        void OnMultiplierChanged(int multiplier)
        {
            if (!multiplierText) return;
            multiplierText.gameObject.SetActive(multiplier > 1);
            multiplierText.SetText("×{0}", multiplier);
        }
    }
}
