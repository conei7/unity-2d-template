using UnityEngine;

namespace Unity2DTemplate.Features
{
    public sealed class AchievementsPanelController : MonoBehaviour
    {
        [SerializeField] private AchievementCatalog catalog;
        [SerializeField] private AchievementRowView[] rows;

        private bool hasWarnedAboutCapacity;

        private void OnEnable()
        {
            if (GameProfile.Instance != null)
            {
                GameProfile.Instance.AchievementUnlocked += HandleAchievementUnlocked;
            }

            Refresh();
        }

        private void OnDisable()
        {
            if (GameProfile.Instance != null)
            {
                GameProfile.Instance.AchievementUnlocked -= HandleAchievementUnlocked;
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
                AchievementDefinition definition = catalog.Definitions[index];
                bool unlocked = GameProfile.Instance?.IsAchievementUnlocked(definition.id) ?? false;
                rows[index].Bind(
                    definition,
                    unlocked,
                    GameProfile.Instance?.GetAchievementUnlockedAtUtc(definition.id));
            }

            for (int index = visibleCount; index < rows.Length; index++)
            {
                rows[index].Hide();
            }

            if (!hasWarnedAboutCapacity && catalog.Definitions.Count > rows.Length)
            {
                hasWarnedAboutCapacity = true;
                Debug.LogWarning(
                    $"Achievements panel has {rows.Length} rows for {catalog.Definitions.Count} definitions. " +
                    "Duplicate a row in the scene and assign it to the array.",
                    this);
            }
        }

        private void HandleAchievementUnlocked(string _)
        {
            Refresh();
        }
    }
}
