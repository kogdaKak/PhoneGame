using UnityEngine;

namespace RichRun
{
    /// <summary>
    /// Автобег вперёд + рулёжка свайпом. Двигает только transform: физика
    /// нужна лишь для триггеров, поэтому Rigidbody кинематический.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] GameConfig config;
        [SerializeField] PlayerVisual visual;

        [Tooltip("Писать в консоль каждый триггер. Только для отладки уровня.")]
        [SerializeField] bool logTriggers;

        public PlayerVisual Visual => visual;

        GameManager _game;
        float _halfWidth;
        float _targetX;
        float _velocityX;

        void Awake()
        {
            var body = GetComponent<Rigidbody>();
            body.isKinematic = true;
            body.useGravity = false;

            _halfWidth = config.RoadHalfWidth;
            _targetX = transform.position.x;
        }

        void Start() => _game = GameManager.Instance;

        /// <summary>Поставить игрока на старт уровня.</summary>
        public void Place(Transform start, float halfWidth)
        {
            if (start) transform.SetPositionAndRotation(start.position, start.rotation);
            _halfWidth = halfWidth;
            _targetX = transform.position.x;
            _velocityX = 0f;
        }

        public void SetRoadHalfWidth(float halfWidth)
        {
            _halfWidth = halfWidth;
            _targetX = Mathf.Clamp(_targetX, -_halfWidth, _halfWidth);
        }

        void Update()
        {
            if (_game == null || _game.State != GameState.Running) return;

            float dt = Time.deltaTime;

            _targetX = Mathf.Clamp(
                _targetX + SwipeInput.DeltaX * config.SwipeSensitivity,
                -_halfWidth, _halfWidth);

            Vector3 pos = transform.position;
            pos.x = Mathf.SmoothDamp(pos.x, _targetX, ref _velocityX, config.LateralSmoothTime);
            pos.z += config.ForwardSpeed * dt;
            transform.position = pos;

            // Корпус доворачивает в сторону движения — как в оригинале.
            float lean = Mathf.Clamp(_velocityX / config.ForwardSpeed, -1f, 1f);
            transform.rotation = Quaternion.Euler(0f, lean * config.MaxTurnAngle, 0f);
        }

        void OnTriggerEnter(Collider other)
        {
            bool handled = other.TryGetComponent(out ITrackTrigger trigger);

            if (logTriggers)
                Debug.Log($"[RichRun] касание: {other.name}, слой {LayerMask.LayerToName(other.gameObject.layer)}, " +
                          (handled ? "обработчик найден" : "ITrackTrigger НЕ найден на этом объекте"), other);

            if (handled) trigger.OnPlayerEnter(this);
        }
    }
}
