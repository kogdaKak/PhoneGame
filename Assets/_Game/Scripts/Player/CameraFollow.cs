using UnityEngine;

namespace RichRun
{
    /// <summary>
    /// Камера идёт за игроком, но вбок — только частично: персонаж ездит
    /// по экрану, и рулёжка читается. Поворот камеры фиксированный.
    /// </summary>
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] Transform target;
        [SerializeField] Vector3 offset = new Vector3(0f, 4.2f, -5.5f);
        [Range(0f, 1f)]
        [SerializeField] float lateralFollow = 0.35f;
        [SerializeField] float smoothTime = 0.12f;

        Vector3 _velocity;

        void LateUpdate()
        {
            if (!target) return;

            Vector3 desired = target.position + offset;
            desired.x = offset.x + target.position.x * lateralFollow;
            transform.position = Vector3.SmoothDamp(transform.position, desired, ref _velocity, smoothTime);
        }
    }
}
