using UnityEngine;

namespace RichRun
{
    /// <summary>Меняет границы рулёжки: узкий мостик, широкая площадь перед особняком.</summary>
    public class RoadWidthZone : MonoBehaviour, ITrackTrigger
    {
        [SerializeField] float halfWidth = 2.5f;

        public void OnPlayerEnter(PlayerController player) => player.SetRoadHalfWidth(halfWidth);

#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0f, 1f, 1f, 0.35f);
            Gizmos.DrawCube(transform.position, new Vector3(halfWidth * 2f, 0.1f, 1f));
        }
#endif
    }
}
