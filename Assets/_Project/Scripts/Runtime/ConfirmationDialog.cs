using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Unity2DTemplate
{
    public sealed class ConfirmationDialog : MonoBehaviour
    {
        [SerializeField] private GameObject dialogPanel;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text messageText;
        [SerializeField] private Button defaultButton;

        private Action confirmed;
        private Action cancelled;

        public bool IsOpen { get; private set; }

        private void Awake()
        {
            HideWithoutCallback();
        }

        public void Show(
            string title,
            string message,
            Action onConfirmed,
            Action onCancelled = null)
        {
            confirmed = onConfirmed;
            cancelled = onCancelled;
            titleText.text = title;
            messageText.text = message;
            IsOpen = true;
            dialogPanel.SetActive(true);

            if (defaultButton != null && EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(defaultButton.gameObject);
            }
        }

        public void Confirm()
        {
            Action callback = confirmed;
            HideWithoutCallback();
            callback?.Invoke();
        }

        public void Cancel()
        {
            Action callback = cancelled;
            HideWithoutCallback();
            callback?.Invoke();
        }

        public void HideWithoutCallback()
        {
            IsOpen = false;
            confirmed = null;
            cancelled = null;
            dialogPanel?.SetActive(false);
        }
    }
}
