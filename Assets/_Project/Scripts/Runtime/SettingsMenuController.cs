using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Unity2DTemplate
{
    public sealed class SettingsMenuController : MonoBehaviour
    {
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private Slider bgmSlider;
        [SerializeField] private Slider seSlider;
        [SerializeField] private ConfirmationDialog confirmationDialog;
        [SerializeField] private SceneLoader sceneLoader;
        [SerializeField] private string titleSceneName = "Title";

        private bool isOpen;
        private float previousTimeScale = 1f;

        private void Awake()
        {
            if (settingsPanel != null)
            {
                settingsPanel.SetActive(false);
            }
        }

        private void Start()
        {
            RefreshSliders();
        }

        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                if (confirmationDialog != null && confirmationDialog.IsOpen)
                {
                    confirmationDialog.Cancel();
                    return;
                }

                Toggle();
            }
        }

        public void Toggle()
        {
            if (isOpen)
            {
                Close();
            }
            else
            {
                Open();
            }
        }

        public void Open()
        {
            if (isOpen || settingsPanel == null)
            {
                return;
            }

            previousTimeScale = Time.timeScale;
            Time.timeScale = 0f;
            isOpen = true;
            settingsPanel.SetActive(true);
            RefreshSliders();
        }

        public void Close()
        {
            if (!isOpen)
            {
                return;
            }

            isOpen = false;
            settingsPanel.SetActive(false);
            Time.timeScale = previousTimeScale;
            AudioManager.Instance?.SaveVolumeSettings();
        }

        public void SetBgmVolume(float volume)
        {
            AudioManager.Instance?.SetBgmVolume(volume);
        }

        public void SetSeVolume(float volume)
        {
            AudioManager.Instance?.SetSeVolume(volume);
        }

        public void RequestReturnToTitle()
        {
            if (confirmationDialog == null)
            {
                ReturnToTitle();
                return;
            }

            confirmationDialog.Show(
                "タイトルへ戻りますか？",
                "保存されていない進行状況は失われます。",
                ReturnToTitle);
        }

        private void RefreshSliders()
        {
            if (AudioManager.Instance == null)
            {
                return;
            }

            bgmSlider?.SetValueWithoutNotify(AudioManager.Instance.BgmVolume);
            seSlider?.SetValueWithoutNotify(AudioManager.Instance.SeVolume);
        }

        private void OnDestroy()
        {
            if (isOpen)
            {
                Time.timeScale = previousTimeScale;
            }
        }

        private void ReturnToTitle()
        {
            Close();
            sceneLoader?.LoadScene(titleSceneName);
        }
    }
}
