using UnityEngine;

namespace RichRun
{
    public class FinishLine : MonoBehaviour, ITrackTrigger
    {
        [SerializeField] ParticleSystem confetti;
        [SerializeField] AudioClip sfx;

        public void OnPlayerEnter(PlayerController player)
        {
            if (confetti) Vfx.Play(confetti, transform.position);
            Sfx.Play(sfx);
            GameManager.Instance.Finish();
        }
    }
}
