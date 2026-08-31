using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Unity2DTemplate.Features
{
    public sealed class StatisticRowView : MonoBehaviour
    {
        [SerializeField] private TMP_Text labelText;
        [SerializeField] private TMP_Text valueText;
        [SerializeField] private Image background;

        public void Bind(string label, string value, bool alternate)
        {
            gameObject.SetActive(true);
            labelText.text = label;
            valueText.text = value;

            if (background != null)
            {
                background.color = alternate
                    ? new Color(0.14f, 0.18f, 0.27f, 0.92f)
                    : new Color(0.10f, 0.13f, 0.21f, 0.92f);
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
