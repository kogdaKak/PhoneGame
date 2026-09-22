using UnityEngine;

namespace RichRun
{
    /// <summary>Ступень богатства: название, порог и цвет полоски.</summary>
    [System.Serializable]
    public class WealthTier
    {
        public string Title = "БЕДНЫЙ";
        public int Threshold;
        public Color BarColor = new Color(1f, 0.45f, 0.1f);
    }

    [CreateAssetMenu(menuName = "RichRun/Game Config", fileName = "GameConfig")]
    public class GameConfig : ScriptableObject
    {
        [Header("Движение")]
        public float ForwardSpeed = 6f;
        [Tooltip("Метров вбок за свайп длиной в ширину экрана.")]
        public float SwipeSensitivity = 14f;
        public float LateralSmoothTime = 0.07f;
        [Tooltip("Максимальный наклон корпуса при повороте, градусы.")]
        public float MaxTurnAngle = 22f;
        public float RoadHalfWidth = 2.5f;

        [Header("Богатство")]
        public int StartWealth = 50;
        public WealthTier[] Tiers =
        {
            new WealthTier { Title = "БЕДНЫЙ",         Threshold = 0,   BarColor = new Color(1f, 0.45f, 0.1f) },
            new WealthTier { Title = "СОСТОЯТЕЛЬНЫЙ",  Threshold = 70,  BarColor = new Color(1f, 0.78f, 0.15f) },
            new WealthTier { Title = "БОГАТЫЙ",        Threshold = 150, BarColor = new Color(0.3f, 0.85f, 0.3f) },
            new WealthTier { Title = "МИЛЛИОНЕР",      Threshold = 260, BarColor = new Color(0.35f, 0.65f, 1f) },
        };

        [Header("Награда")]
        [Tooltip("Монет за единицу богатства. Итог = Wealth * Multiplier * CoinsPerWealth.")]
        public int CoinsPerWealth = 1;
    }
}
