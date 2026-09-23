using UnityEngine;
using UnityEngine.UI;

namespace RichRun
{
    /// <summary>
    /// Стартовый экран: карта уровней, боковые кнопки и шестерёнка в углу.
    /// С началом забега всё лишнее прячется, а иконка в углу меняется на выход.
    /// Запуск игры делает GameManager — без этой панели он тоже работает.
    /// </summary>
    public class StartPanel : MonoBehaviour
    {
        [Tooltip("Общий контейнер стартового экрана. Необязателен.")]
        [SerializeField] GameObject root;

        [Tooltip("Что ещё спрятать с началом забега: карта уровней, боковые кнопки.")]
        [SerializeField] GameObject[] hideOnStart;

        [Header("Кнопка в углу")]
        [SerializeField] Image cornerIcon;
        [SerializeField] Sprite readyIcon;
        [SerializeField] Sprite runningIcon;

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
            bool ready = state == GameState.Ready;

            if (root) root.SetActive(ready);

            for (int i = 0; i < hideOnStart.Length; i++)
                if (hideOnStart[i]) hideOnStart[i].SetActive(ready);

            if (!cornerIcon) return;

            // Незаполненный спрайт не трогаем — иначе иконка просто исчезнет.
            var icon = ready ? readyIcon : runningIcon;
            if (icon) cornerIcon.sprite = icon;
        }
    }
}
