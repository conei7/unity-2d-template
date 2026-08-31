using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Unity2DTemplate.Features
{
    // Subscribe before gameplay components record achievements in Start().
    [DefaultExecutionOrder(-50)]
    public sealed class AchievementToastView : MonoBehaviour
    {
        [SerializeField] private AchievementCatalog catalog;
        [SerializeField] private GameObject toastRoot;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private Image iconImage;
        [SerializeField, Min(0.1f)] private float visibleSeconds = 3f;

        private Coroutine activeRoutine;

        private void Awake()
        {
            toastRoot?.SetActive(false);
        }

        private void Start()
        {
            if (GameProfile.Instance != null)
            {
                GameProfile.Instance.AchievementUnlocked += Show;
            }
        }

        private void OnDestroy()
        {
            if (GameProfile.Instance != null)
            {
                GameProfile.Instance.AchievementUnlocked -= Show;
            }
        }

        private void Show(string id)
        {
            AchievementDefinition definition = catalog?.Find(id);
            if (definition == null || toastRoot == null)
            {
                return;
            }

            titleText.text = definition.title;
            if (iconImage != null)
            {
                iconImage.sprite = definition.icon;
                iconImage.enabled = definition.icon != null;
            }

            if (activeRoutine != null)
            {
                StopCoroutine(activeRoutine);
            }

            activeRoutine = StartCoroutine(ShowRoutine());
        }

        private IEnumerator ShowRoutine()
        {
            toastRoot.SetActive(true);
            yield return new WaitForSecondsRealtime(visibleSeconds);
            toastRoot.SetActive(false);
            activeRoutine = null;
        }
    }
}
