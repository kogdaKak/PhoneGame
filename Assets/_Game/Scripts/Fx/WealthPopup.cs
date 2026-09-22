using UnityEngine;
using UnityEngine.Pool;

namespace RichRun
{
    /// <summary>
    /// Копит подобранное за короткое окно и показывает одно «+12 $» вместо шести «+2 $» —
    /// так же, как в оригинале. Плюсы и минусы не смешиваются.
    /// </summary>
    public class WealthPopup : MonoBehaviour
    {
        [SerializeField] FloatingText prefab;
        [SerializeField] RectTransform container;
        [SerializeField] Camera worldCamera;
        [SerializeField] Transform anchor;
        [SerializeField] Vector3 worldOffset = new Vector3(0f, 1.4f, 0f);
        [SerializeField] Vector2 screenOffset = new Vector2(90f, 0f);
        [SerializeField] float flushDelay = 0.3f;

        GameManager _game;
        ObjectPool<FloatingText> _pool;
        System.Action<FloatingText> _release;
        Camera _uiCamera;
        int _pending;
        float _timer;

        void Awake()
        {
            _pool = new ObjectPool<FloatingText>(
                createFunc: () => Instantiate(prefab, container),
                actionOnGet: t => t.gameObject.SetActive(true),
                actionOnRelease: t => t.gameObject.SetActive(false),
                actionOnDestroy: t => Destroy(t.gameObject),
                defaultCapacity: 6);

            // Делегат создаём один раз: иначе каждая всплывашка — лишняя аллокация.
            _release = _pool.Release;

            var canvas = container.GetComponentInParent<Canvas>();
            _uiCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : worldCamera;
        }

        void Start()
        {
            _game = GameManager.Instance;
            _game.WealthChanged += OnWealthChanged;
        }

        void OnDestroy()
        {
            if (_game != null) _game.WealthChanged -= OnWealthChanged;
        }

        void OnWealthChanged(int wealth, int delta)
        {
            // Смена знака — сразу выкидываем накопленное, чтобы «+4» и «−10» не схлопнулись.
            if (_pending != 0 && (_pending > 0) != (delta > 0)) Flush();

            _pending += delta;
            _timer = flushDelay;
        }

        void Update()
        {
            if (_pending == 0) return;
            _timer -= Time.deltaTime;
            if (_timer <= 0f) Flush();
        }

        void Flush()
        {
            int amount = _pending;
            _pending = 0;
            if (amount == 0) return;

            Vector2 screen = worldCamera.WorldToScreenPoint(anchor.position + worldOffset);
            screen += screenOffset;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                container, screen, _uiCamera, out Vector2 local);

            _pool.Get().Show(amount, local, _release);
        }
    }
}
