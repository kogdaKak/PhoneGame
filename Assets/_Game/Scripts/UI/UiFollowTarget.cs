using UnityEngine;

namespace RichRun
{
    /// <summary>Держит UI-элемент над объектом в мире (полоска богатства над головой).</summary>
    [RequireComponent(typeof(RectTransform))]
    public class UiFollowTarget : MonoBehaviour
    {
        [SerializeField] Transform target;
        [SerializeField] Camera worldCamera;
        [SerializeField] Vector3 worldOffset = new Vector3(0f, 2.1f, 0f);

        RectTransform _rect;
        RectTransform _parent;
        Camera _uiCamera;

        void Awake()
        {
            _rect = (RectTransform)transform;
            _parent = (RectTransform)_rect.parent;

            var canvas = GetComponentInParent<Canvas>();
            _uiCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : worldCamera;
        }

        void LateUpdate()
        {
            if (!target) return;

            Vector2 screen = worldCamera.WorldToScreenPoint(target.position + worldOffset);
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_parent, screen, _uiCamera, out Vector2 local))
                _rect.anchoredPosition = local;
        }
    }
}
