using UnityEngine;

namespace RichRun
{
    /// <summary>Экран до старта: карта уровней и подсказка «проведите по экрану».</summary>
    public class StartPanel : MonoBehaviour
    {
        [SerializeField] GameObject root;

        void Start()
        {
            root.SetActive(GameManager.Instance.State == GameState.Ready);
        }

        void Update()
        {
            if (!SwipeInput.Tapped) return;

            GameManager.Instance.StartRun();
            root.SetActive(false);
            enabled = false;
        }
    }
}
