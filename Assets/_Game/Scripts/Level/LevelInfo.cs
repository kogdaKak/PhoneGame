using UnityEngine;

namespace RichRun
{
    /// <summary>Корень префаба уровня: откуда стартует игрок и какая ширина дороги.</summary>
    public class LevelInfo : MonoBehaviour
    {
        [SerializeField] Transform playerStart;
        [SerializeField] float roadHalfWidth = 2.5f;

        public Transform PlayerStart => playerStart;
        public float RoadHalfWidth => roadHalfWidth;

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (!playerStart) return;
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(playerStart.position + Vector3.up, new Vector3(roadHalfWidth * 2f, 2f, 0.2f));
        }
#endif
    }
}
