using UnityEngine;

namespace Unity2DTemplate.Features
{
    public sealed class GameSessionStatistics : MonoBehaviour
    {
        [SerializeField] private string startedStatisticId = "games_started";
        [SerializeField] private string playTimeStatisticId = "play_time_seconds";
        [SerializeField] private string clearsStatisticId = "games_cleared";
        [SerializeField] private string bestClearTimeStatisticId = "best_clear_time_seconds";
        [SerializeField] private string firstStartAchievementId = "first_steps";
        [SerializeField] private string firstClearAchievementId = "first_clear";

        private float elapsedSeconds;
        private float committedSeconds;
        private bool completed;

        private void Start()
        {
            GameProfile.Instance?.AddStatistic(startedStatisticId, 1d);
            GameProfile.Instance?.UnlockAchievement(firstStartAchievementId);
        }

        private void Update()
        {
            elapsedSeconds += Time.deltaTime;
        }

        public void CompleteSession()
        {
            if (completed || GameProfile.Instance == null)
            {
                return;
            }

            completed = true;
            CommitPlayTime();
            GameProfile.Instance.AddStatistic(clearsStatisticId, 1d);
            GameProfile.Instance.SetLowestPositiveStatistic(bestClearTimeStatisticId, elapsedSeconds);
            GameProfile.Instance.UnlockAchievement(firstClearAchievementId);
            GameProfile.Instance.SaveNow();
        }

        private void CommitPlayTime()
        {
            float uncommittedSeconds = elapsedSeconds - committedSeconds;
            if (uncommittedSeconds <= 0f || GameProfile.Instance == null)
            {
                return;
            }

            GameProfile.Instance.AddStatistic(playTimeStatisticId, uncommittedSeconds);
            committedSeconds = elapsedSeconds;
        }

        private void OnDisable()
        {
            CommitPlayTime();
        }
    }
}
