using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Unity2DTemplate.Features
{
    public sealed class GalleryItemView : MonoBehaviour
    {
        [SerializeField] private Image thumbnailImage;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text creatorText;
        [SerializeField] private GameObject lockedOverlay;

        private GalleryEntryDefinition entry;
        private GalleryPanelController owner;
        private bool isUnlocked;

        public void Bind(
            GalleryEntryDefinition definition,
            bool unlocked,
            GalleryPanelController controller)
        {
            gameObject.SetActive(true);
            entry = definition;
            owner = controller;
            isUnlocked = unlocked;

            titleText.text = unlocked ? definition.title : "???";
            creatorText.text = unlocked ? $"by {definition.creator}" : "LOCKED";

            if (thumbnailImage != null)
            {
                thumbnailImage.sprite = unlocked ? definition.image : null;
                thumbnailImage.enabled = unlocked && definition.image != null;
            }

            if (lockedOverlay != null)
            {
                lockedOverlay.SetActive(!unlocked);
            }
        }

        public void Select()
        {
            if (isUnlocked && entry != null)
            {
                owner?.OpenEntry(entry);
            }
        }

        public void Hide()
        {
            entry = null;
            owner = null;
            gameObject.SetActive(false);
        }
    }
}
