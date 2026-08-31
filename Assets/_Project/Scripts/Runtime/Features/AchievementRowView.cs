using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Unity2DTemplate.Features
{
    public sealed class AchievementRowView : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private TMP_Text statusText;
        [SerializeField] private GameObject lockedOverlay;

        public void Bind(AchievementDefinition definition, bool unlocked, System.DateTime? unlockedAtUtc)
        {
            gameObject.SetActive(true);

            bool hideDetails = definition.isSecret && !unlocked;
            titleText.text = hideDetails ? "???" : definition.title;
            descriptionText.text = hideDetails ? "条件は秘密です。" : definition.description;
            statusText.text = unlocked
                ? unlockedAtUtc.HasValue
                    ? $"達成 {unlockedAtUtc.Value.ToLocalTime():yyyy/MM/dd}"
                    : "達成済み"
                : "未達成";

            if (iconImage != null)
            {
                iconImage.sprite = hideDetails ? null : definition.icon;
                iconImage.enabled = !hideDetails && definition.icon != null;
            }

            if (lockedOverlay != null)
            {
                lockedOverlay.SetActive(!unlocked);
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
