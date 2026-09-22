using System;
using TMPro;
using UnityEngine;

namespace RichRun
{
    /// <summary>Всплывающее «+12 $». Живёт в UI-канвасе, возвращает себя в пул сам.</summary>
    [RequireComponent(typeof(RectTransform))]
    public class FloatingText : MonoBehaviour
    {
        [SerializeField] TMP_Text label;
        [SerializeField] CanvasGroup group;
        [SerializeField] float lifeTime = 0.9f;
        [SerializeField] float riseSpeed = 160f;
        [SerializeField] float popTime = 0.15f;
        [SerializeField] Color positiveColor = new Color(0.25f, 0.9f, 0.35f);
        [SerializeField] Color negativeColor = new Color(0.95f, 0.25f, 0.25f);

        RectTransform _rect;
        Action<FloatingText> _onDone;
        Vector2 _position;
        float _time;

        void Awake() => _rect = (RectTransform)transform;

        public void Show(int amount, Vector2 anchoredPosition, Action<FloatingText> onDone)
        {
            _onDone = onDone;
            _position = anchoredPosition;
            _time = 0f;

            label.color = amount >= 0 ? positiveColor : negativeColor;
            label.SetText(amount >= 0 ? "+{0} $" : "{0} $", amount);

            _rect.anchoredPosition = _position;
            _rect.localScale = Vector3.zero;
            group.alpha = 1f;
        }

        void Update()
        {
            _time += Time.deltaTime;
            if (_time >= lifeTime)
            {
                _onDone?.Invoke(this);
                return;
            }

            float t = _time / lifeTime;
            _position.y += riseSpeed * Time.deltaTime;
            _rect.anchoredPosition = _position;
            _rect.localScale = Vector3.one * Mathf.Min(1f, _time / popTime);
            group.alpha = 1f - t * t;
        }
    }
}
