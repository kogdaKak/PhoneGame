using UnityEngine;

namespace RichRun
{
    /// <summary>Прогресс игрока. Весь PlayerPrefs — только здесь.</summary>
    public static class Save
    {
        const string CoinsKey = "rr.coins";
        const string LevelKey = "rr.level";

        public static int Coins
        {
            get => PlayerPrefs.GetInt(CoinsKey, 0);
            set { PlayerPrefs.SetInt(CoinsKey, value); PlayerPrefs.Save(); }
        }

        /// <summary>Индекс пройденных уровней (0 = первый уровень).</summary>
        public static int Level
        {
            get => PlayerPrefs.GetInt(LevelKey, 0);
            set { PlayerPrefs.SetInt(LevelKey, value); PlayerPrefs.Save(); }
        }
    }
}
