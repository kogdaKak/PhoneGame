using UnityEngine;
using UnityEngine.InputSystem;

namespace RichRun
{
    /// <summary>
    /// Палец/мышь без аллокаций и без компонентов. Опрашивается лениво:
    /// первое обращение за кадр читает устройство, остальные берут кэш.
    /// </summary>
    public static class SwipeInput
    {
        static int _frame = -1;
        static Vector2 _prev;
        static bool _dragging;
        static float _deltaX;
        static bool _tapped;

        /// <summary>Сдвиг за кадр в долях ширины экрана (+ вправо).</summary>
        public static float DeltaX { get { Sample(); return _deltaX; } }

        /// <summary>Кадр, в котором палец коснулся экрана.</summary>
        public static bool Tapped { get { Sample(); return _tapped; } }

        static void Sample()
        {
            if (_frame == Time.frameCount) return;
            _frame = Time.frameCount;
            _deltaX = 0f;
            _tapped = false;

            if (!TryReadPointer(out Vector2 pos))
            {
                _dragging = false;
                return;
            }

            if (_dragging)
            {
                _deltaX = (pos.x - _prev.x) / Screen.width;
            }
            else
            {
                _dragging = true;
                _tapped = true;
            }
            _prev = pos;
        }

        static bool TryReadPointer(out Vector2 pos)
        {
            var touch = Touchscreen.current;
            if (touch != null && touch.primaryTouch.press.isPressed)
            {
                pos = touch.primaryTouch.position.ReadValue();
                return true;
            }

            var mouse = Mouse.current;
            if (mouse != null && mouse.leftButton.isPressed)
            {
                pos = mouse.position.ReadValue();
                return true;
            }

            pos = default;
            return false;
        }
    }
}
