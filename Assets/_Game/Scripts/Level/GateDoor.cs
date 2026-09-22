using UnityEngine;

namespace RichRun
{
    /// <summary>Одна створка развилки. Бонус может быть и отрицательным.</summary>
    public class GateDoor : MonoBehaviour, ITrackTrigger
    {
        [SerializeField] ChoiceGate gate;
        [SerializeField] int wealthBonus = 20;
        [SerializeField] Collider trigger;

        public int WealthBonus => wealthBonus;

        public void OnPlayerEnter(PlayerController player) => gate.Choose(this);

        public void Close()
        {
            if (trigger) trigger.enabled = false;
        }
    }
}
