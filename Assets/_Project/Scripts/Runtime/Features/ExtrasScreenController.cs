using UnityEngine;

namespace Unity2DTemplate.Features
{
    public sealed class ExtrasScreenController : MonoBehaviour
    {
        [Header("Optional sample achievement")]
        [SerializeField] private string visitAchievementId = "extras_visited";

        private void Start()
        {
            GameProfile.Instance?.UnlockAchievement(visitAchievementId);
        }
    }
}
