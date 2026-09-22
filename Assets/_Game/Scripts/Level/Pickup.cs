using UnityEngine;

namespace RichRun
{
    /// <summary>
    /// Пачка денег (Value &gt; 0) или бутылка (Value &lt; 0). Один скрипт на оба случая:
    /// поведение отличается только знаком и эффектом.
    /// </summary>
    public class Pickup : MonoBehaviour, ITrackTrigger
    {
        [SerializeField] int value = 2;
        [SerializeField] GameObject visual;
        [SerializeField] ParticleSystem vfx;
        [SerializeField] AudioClip sfx;

        public void OnPlayerEnter(PlayerController player)
        {
            GameManager.Instance.AddWealth(value);

            if (vfx) Vfx.Play(vfx, transform.position);
            Sfx.Play(sfx);
            if (value < 0) player.Visual.PlayHit();

            // Гасим только визуал и коллайдер — объект остаётся частью статичного уровня.
            if (visual) visual.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}
