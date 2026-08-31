using System;
using TMPro;
using UnityEngine;

namespace Unity2DTemplate.Features
{
    public sealed class StatisticsPanelController : MonoBehaviour
    {
        [SerializeField] private StatisticsCatalog catalog;
        [SerializeField] private StatisticRowView[] rows;
        [SerializeField] private TMP_Text capacityText;

        private bool hasWarnedAboutCapacity;

        private void OnEnable()
        {
            if (GameProfile.Instance != null)
            {
                GameProfile.Instance.StatisticChanged += HandleStatisticChanged;
            }

            Refresh();
        }

        private void OnDisable()
        {
            if (GameProfile.Instance != null)
            {
                GameProfile.Instance.StatisticChanged -= HandleStatisticChanged;
            }
        }

        public void Refresh()
        {
            if (catalog == null || rows == null)
            {
                return;
            }

            int visibleCount = Mathf.Min(catalog.Definitions.Count, rows.Length);
            for (int index = 0; index < visibleCount; index++)
            {
                StatisticDefinition definition = catalog.Definitions[index];
                double value = GameProfile.Instance?.GetStatistic(definition.id) ?? 0d;
                rows[index].Bind(
                    definition.label,
                    FormatValue(value, definition),
                    index % 2 != 0);
            }

            for (int index = visibleCount; index < rows.Length; index++)
            {
                rows[index].Hide();
            }

            if (capacityText != null)
            {
                capacityText.text = $"{visibleCount} / {catalog.Definitions.Count}";
            }

            if (!hasWarnedAboutCapacity && catalog.Definitions.Count > rows.Length)
            {
                hasWarnedAboutCapacity = true;
                Debug.LogWarning(
                    $"Statistics panel has {rows.Length} rows for {catalog.Definitions.Count} definitions. " +
                    "Duplicate a row in the scene and assign it to the array.",
                    this);
            }
        }

        private void HandleStatisticChanged(string _)
        {
            Refresh();
        }

        private static string FormatValue(double value, StatisticDefinition definition)
        {
            switch (definition.format)
            {
                case StatisticDisplayFormat.Duration:
                    TimeSpan elapsed = TimeSpan.FromSeconds(Math.Max(0d, value));
                    long hours = (long)elapsed.TotalHours;
                    return $"{hours:D2}:{elapsed.Minutes:D2}:{elapsed.Seconds:D2}";

                case StatisticDisplayFormat.Decimal:
                    return Math.Max(0d, value).ToString($"N{definition.decimalPlaces}");

                default:
                    return Math.Max(0d, value).ToString("N0");
            }
        }
    }
}
