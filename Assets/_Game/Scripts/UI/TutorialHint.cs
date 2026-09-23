using UnityEngine;

namespace RichRun
{
    /// <summary>
    /// Подсказка «проведите по экрану, чтобы повернуть»: рука ходит вдоль стрелки,
    /// пока забег не начался. Прячется сама по смене состояния — как и StartPanel,
    /// на запуск игры не влияет.
    /// </summary>
    public class TutorialHint : MonoBehaviour
    {
        [SerializeField] GameObject root;
        [SerializeField] RectTransform hand;

        [Header("Движение руки")]
        [Tooltip("Размах в каждую сторону, пиксели канваса.")]
        [SerializeField] float travel = 130f;
        [SerializeField] float period = 1.8f;
        [SerializeField] float tilt = 8f;
        [Tooltip("Насколько рука прижимается в крайних точках.")]
        [SerializeField] float pressScale = 0.12f;

        GameManager _game;
        Vector2 _origin;
        float _time;

        void Start()
        {
            if (hand) _origin = hand.anchoredPosition;

            _game = GameManager.Instance;
            _game.StateChanged += OnStateChanged;
            OnStateChanged(_game.State);
        }

        void OnDestroy()
        {
            if (_game != null) _game.StateChanged -= OnStateChanged;
        }

        void OnStateChanged(GameState state)
        {
            bool show = state == GameState.Ready;
            if (root) root.SetActive(show);

            // Событие приходит независимо от enabled, так что выключаться безопасно.
            enabled = show;
            if (show) _time = 0f;
        }

        void Update()
        {
            if (!hand) return;

            _time += Time.deltaTime;
            float swing = Mathf.Sin(_time * Mathf.PI * 2f / period);   // -1..1

            var position = _origin;
            position.x += swing * travel;
            hand.anchoredPosition = position;
            hand.localRotation = Quaternion.Euler(0f, 0f, -swing * tilt);

            // В крайних точках |swing| = 1 — там рука слегка придавливается к экрану.
            float press = 1f - pressScale * Mathf.Abs(swing);
            hand.localScale = new Vector3(press, press, 1f);
        }
    }
}
