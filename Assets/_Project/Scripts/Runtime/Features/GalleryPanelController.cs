using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Unity2DTemplate.Features
{
    public sealed class GalleryPanelController : MonoBehaviour
    {
        [Header("Catalog and slots")]
        [SerializeField] private GalleryCatalog catalog;
        [SerializeField] private GalleryItemView[] slots;

        [Header("Pre-authored detail overlay")]
        [SerializeField] private GameObject detailRoot;
        [SerializeField] private Image detailImage;
        [SerializeField] private TMP_Text detailTitle;
        [SerializeField] private TMP_Text detailCreator;
        [SerializeField] private TMP_Text detailDescription;
        [SerializeField] private TMP_Text audioButtonLabel;
        [SerializeField] private Slider audioProgress;
        [SerializeField] private AudioSource audioSource;

        private GalleryEntryDefinition currentEntry;
        private bool hasWarnedAboutCapacity;

        private void Awake()
        {
            detailRoot?.SetActive(false);
        }

        private void OnEnable()
        {
            if (GameProfile.Instance != null)
            {
                GameProfile.Instance.GalleryEntryUnlocked += HandleGalleryEntryUnlocked;
            }

            EnsureDefaultUnlocks();
            Refresh();
        }

        private void OnDisable()
        {
            if (GameProfile.Instance != null)
            {
                GameProfile.Instance.GalleryEntryUnlocked -= HandleGalleryEntryUnlocked;
            }

            CloseEntry();
        }

        private void Update()
        {
            if (audioSource == null || audioProgress == null || currentEntry?.audioClip == null)
            {
                return;
            }

            float progress = audioSource.clip != null && audioSource.clip.length > 0f
                ? audioSource.time / audioSource.clip.length
                : 0f;
            audioProgress.SetValueWithoutNotify(progress);

            if (!audioSource.isPlaying && progress >= 0.999f)
            {
                audioProgress.SetValueWithoutNotify(0f);
                UpdateAudioLabel();
            }
        }

        public void Refresh()
        {
            if (catalog == null || slots == null)
            {
                return;
            }

            int visibleCount = Mathf.Min(catalog.Entries.Count, slots.Length);
            for (int index = 0; index < visibleCount; index++)
            {
                GalleryEntryDefinition entry = catalog.Entries[index];
                bool unlocked = GameProfile.Instance?.IsGalleryEntryUnlocked(entry.id) ?? entry.unlockedByDefault;
                slots[index].Bind(entry, unlocked, this);
            }

            for (int index = visibleCount; index < slots.Length; index++)
            {
                slots[index].Hide();
            }

            if (!hasWarnedAboutCapacity && catalog.Entries.Count > slots.Length)
            {
                hasWarnedAboutCapacity = true;
                Debug.LogWarning(
                    $"Gallery panel has {slots.Length} slots for {catalog.Entries.Count} entries. " +
                    "Duplicate a slot in the scene and assign it to the array.",
                    this);
            }
        }

        public void OpenEntry(GalleryEntryDefinition entry)
        {
            currentEntry = entry;
            detailTitle.text = entry.title;
            detailCreator.text = $"by {entry.creator}";
            detailDescription.text = entry.description;

            if (detailImage != null)
            {
                detailImage.sprite = entry.image;
                detailImage.enabled = entry.image != null;
                detailImage.preserveAspect = true;
            }

            if (audioProgress != null)
            {
                audioProgress.gameObject.SetActive(entry.audioClip != null);
                audioProgress.SetValueWithoutNotify(0f);
            }

            if (audioButtonLabel != null)
            {
                audioButtonLabel.transform.parent.gameObject.SetActive(entry.audioClip != null);
            }

            detailRoot.SetActive(true);
            UpdateAudioLabel();
        }

        public void CloseEntry()
        {
            if (audioSource != null)
            {
                audioSource.Stop();
                audioSource.clip = null;
            }

            currentEntry = null;
            detailRoot?.SetActive(false);
        }

        public void ToggleAudio()
        {
            if (audioSource == null || currentEntry?.audioClip == null)
            {
                return;
            }

            if (audioSource.isPlaying)
            {
                audioSource.Pause();
            }
            else
            {
                if (audioSource.clip != currentEntry.audioClip)
                {
                    audioSource.clip = currentEntry.audioClip;
                }

                audioSource.Play();
            }

            UpdateAudioLabel();
        }

        public void SeekAudio(float normalizedTime)
        {
            if (audioSource?.clip == null)
            {
                return;
            }

            audioSource.time = Mathf.Clamp01(normalizedTime) * audioSource.clip.length;
        }

        private void EnsureDefaultUnlocks()
        {
            if (catalog == null || GameProfile.Instance == null)
            {
                return;
            }

            for (int index = 0; index < catalog.Entries.Count; index++)
            {
                GalleryEntryDefinition entry = catalog.Entries[index];
                if (entry.unlockedByDefault)
                {
                    GameProfile.Instance.UnlockGalleryEntry(entry.id);
                }
            }
        }

        private void HandleGalleryEntryUnlocked(string _)
        {
            Refresh();
        }

        private void UpdateAudioLabel()
        {
            if (audioButtonLabel != null)
            {
                audioButtonLabel.text = audioSource != null && audioSource.isPlaying
                    ? "停止"
                    : "再生";
            }
        }
    }
}
