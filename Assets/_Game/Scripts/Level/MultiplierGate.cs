using TMPro;
using UnityEngine;

namespace RichRun
{
    /// <summary>
    /// Дом-чекпоинт с табличкой «×N». Хватает богатства — заходишь и множитель растёт,
    /// не хватает — охрана не пускает и забег заканчивается на этом множителе.
    /// </summary>
    public class MultiplierGate : MonoBehaviour, ITrackTrigger
    {
        [SerializeField] int requiredWealth = 80;
        [SerializeField] int multiplier = 2;

        [Header("Вид")]
        [SerializeField] TMP_Text label;
        [SerializeField] GameObject openedDoor;
        [SerializeField] GameObject closedDoor;
        [SerializeField] ParticleSystem passVfx;
        [SerializeField] AudioClip passSfx;
        [SerializeField] AudioClip blockSfx;
        [SerializeField] Collider trigger;

        void Awake()
        {
            if (label) label.SetText("×{0}", multiplier);
            if (openedDoor) openedDoor.SetActive(false);
            if (closedDoor) closedDoor.SetActive(true);
        }

        public void OnPlayerEnter(PlayerController player)
        {
            if (trigger) trigger.enabled = false;

            var game = GameManager.Instance;
            if (game.Wealth >= requiredWealth)
            {
                game.SetMultiplier(multiplier);
                if (openedDoor) openedDoor.SetActive(true);
                if (closedDoor) closedDoor.SetActive(false);
                if (passVfx) Vfx.Play(passVfx, transform.position);
                Sfx.Play(passSfx);
            }
            else
            {
                Sfx.Play(blockSfx);
                player.Visual.PlayHit();
                game.Finish();
            }
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            if (label) label.SetText("×{0}", multiplier);
        }
#endif
    }
}
