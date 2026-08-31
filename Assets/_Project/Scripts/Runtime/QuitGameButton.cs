using UnityEngine;

namespace Unity2DTemplate
{
    public sealed class QuitGameButton : MonoBehaviour
    {
        [SerializeField] private ConfirmationDialog confirmationDialog;

        public void RequestQuit()
        {
            if (confirmationDialog == null)
            {
                Quit();
                return;
            }

            confirmationDialog.Show(
                "ゲームを終了しますか？",
                "タイトル画面を閉じます。",
                Quit);
        }

        private static void Quit()
        {
            Application.Quit();
        }
    }
}
