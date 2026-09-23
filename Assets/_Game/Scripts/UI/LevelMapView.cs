using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RichRun
{
    /// <summary>
    /// Полоска уровней на стартовом экране: пять сегментов между двумя домами.
    /// Пройденные закрашиваются, текущий подсвечивается и слегка увеличен.
    /// </summary>
    public class LevelMapView : MonoBehaviour
    {
        [SerializeField] Image[] segments;
        [SerializeField] Image[] numberCircles;
        [SerializeField] TMP_Text[] numberLabels;

        [Header("Цвета")]
        [SerializeField] Color doneColor = new Color(0.30f, 0.85f, 0.45f);
        [SerializeField] Color todoColor = Color.white;
        [SerializeField] Color currentColor = new Color(1f, 0.82f, 0.20f);
        [SerializeField] Color doneLabelColor = Color.white;
        [SerializeField] Color todoLabelColor = new Color(0.27f, 0.26f, 0.27f);

        [Header("Текущий уровень")]
        [SerializeField] float currentScale = 1.25f;

        void Start()
        {
            // LevelNumber единичный, а сегменты нумеруются с нуля.
            int current = GameManager.Instance.LevelNumber - 1;

            for (int i = 0; i < segments.Length; i++)
                if (segments[i]) segments[i].color = i < current ? doneColor : todoColor;

            for (int i = 0; i < numberCircles.Length; i++)
            {
                bool done = i < current;
                bool isCurrent = i == current;

                if (numberCircles[i])
                {
                    numberCircles[i].color = done ? doneColor : isCurrent ? currentColor : todoColor;
                    float scale = isCurrent ? currentScale : 1f;
                    numberCircles[i].transform.localScale = new Vector3(scale, scale, 1f);
                }

                if (i >= numberLabels.Length || !numberLabels[i]) continue;
                numberLabels[i].SetText("{0}", i + 1);
                numberLabels[i].color = done ? doneLabelColor : todoLabelColor;
            }
        }
    }
}
