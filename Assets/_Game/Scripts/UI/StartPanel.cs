using UnityEngine;

namespace RichRun
{
    /// <summary>
    /// Экран до старта: карта уровней и подсказка «проведите по экрану».
    /// Только показывает и прячет — запуск забега делает GameManager,
    /// поэтому без этой панели игра всё равно стартует.
    /// </summary>
    public class StartPanel : MonoBehaviour
    {
        [SerializeField] GameObject root;

        GameManager _game;

        void Start()
        {
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
            if (root) root.SetActive(state == GameState.Ready);
        }
    }
}
