using UnityEngine;

namespace RichRun
{
    /// <summary>
    /// Пара ворот вроде «ШКОЛА / ВЕЧЕРИНКА»: проходишь одни — вторые закрываются.
    /// Сами створки — <see cref="GateDoor"/>.
    /// </summary>
    public class ChoiceGate : MonoBehaviour
    {
        [SerializeField] GateDoor[] doors;
        [SerializeField] ParticleSystem passVfx;
        [SerializeField] AudioClip goodSfx;
        [SerializeField] AudioClip badSfx;

        bool _used;

        public void Choose(GateDoor door)
        {
            if (_used) return;
            _used = true;

            GameManager.Instance.AddWealth(door.WealthBonus);

            if (passVfx) Vfx.Play(passVfx, door.transform.position);
            Sfx.Play(door.WealthBonus >= 0 ? goodSfx : badSfx);
            if (door.WealthBonus < 0) GameManager.Instance.Player.Visual.PlayHit();

            for (int i = 0; i < doors.Length; i++)
                if (doors[i]) doors[i].Close();
        }
    }
}
