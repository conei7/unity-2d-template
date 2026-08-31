using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity2DTemplate.Features
{
    [DisallowMultipleComponent]
    public sealed class GameProfile : MonoBehaviour
    {
        public const int CurrentSaveVersion = 1;
        public const string SaveIntegrityMarker = "unity-2d-template-profile-v1";

        [SerializeField, Min(1f)] private float autoSaveIntervalSeconds = 30f;

        private readonly Dictionary<string, StatisticRecord> statisticIndex =
            new Dictionary<string, StatisticRecord>(StringComparer.Ordinal);
        private readonly Dictionary<string, UnlockRecord> achievementIndex =
            new Dictionary<string, UnlockRecord>(StringComparer.Ordinal);
        private readonly Dictionary<string, UnlockRecord> galleryIndex =
            new Dictionary<string, UnlockRecord>(StringComparer.Ordinal);

        private ProfileData data;
        private bool isDirty;
        private float autoSaveTimer;

        public static GameProfile Instance { get; private set; }

        public event Action<string> StatisticChanged;
        public event Action<string> AchievementUnlocked;
        public event Action<string> GalleryEntryUnlocked;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            Instance = null;
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            data = ProfileStorage.Load();
            RebuildIndexes();
        }

        private void Update()
        {
            if (!isDirty)
            {
                return;
            }

            autoSaveTimer += Time.unscaledDeltaTime;
            if (autoSaveTimer >= autoSaveIntervalSeconds)
            {
                SaveNow();
            }
        }

        public double GetStatistic(string id)
        {
            string normalizedId = NormalizeId(id);
            return normalizedId != null && statisticIndex.TryGetValue(normalizedId, out StatisticRecord record)
                ? record.value
                : 0d;
        }

        public void AddStatistic(string id, double amount)
        {
            if (!IsFinite(amount) || Math.Abs(amount) <= double.Epsilon)
            {
                return;
            }

            StatisticRecord record = GetOrCreateStatistic(id);
            if (record == null)
            {
                return;
            }

            record.value += amount;
            NotifyStatisticChanged(record.id);
        }

        public void SetStatistic(string id, double value)
        {
            if (!IsFinite(value))
            {
                return;
            }

            StatisticRecord record = GetOrCreateStatistic(id);
            if (record == null || Math.Abs(record.value - value) <= double.Epsilon)
            {
                return;
            }

            record.value = value;
            NotifyStatisticChanged(record.id);
        }

        public void SetHighestStatistic(string id, double candidate)
        {
            if (IsFinite(candidate) && candidate > GetStatistic(id))
            {
                SetStatistic(id, candidate);
            }
        }

        public void SetLowestPositiveStatistic(string id, double candidate)
        {
            if (!IsFinite(candidate) || candidate <= 0d)
            {
                return;
            }

            double current = GetStatistic(id);
            if (current <= 0d || candidate < current)
            {
                SetStatistic(id, candidate);
            }
        }

        public bool UnlockAchievement(string id)
        {
            return Unlock(id, achievementIndex, data.achievements, AchievementUnlocked);
        }

        public bool IsAchievementUnlocked(string id)
        {
            string normalizedId = NormalizeId(id);
            return normalizedId != null && achievementIndex.ContainsKey(normalizedId);
        }

        public DateTime? GetAchievementUnlockedAtUtc(string id)
        {
            string normalizedId = NormalizeId(id);
            if (normalizedId == null
                || !achievementIndex.TryGetValue(normalizedId, out UnlockRecord record)
                || record.unlockedUtcTicks <= 0L)
            {
                return null;
            }

            return new DateTime(record.unlockedUtcTicks, DateTimeKind.Utc);
        }

        public bool UnlockGalleryEntry(string id)
        {
            return Unlock(id, galleryIndex, data.galleryEntries, GalleryEntryUnlocked);
        }

        public bool IsGalleryEntryUnlocked(string id)
        {
            string normalizedId = NormalizeId(id);
            return normalizedId != null && galleryIndex.ContainsKey(normalizedId);
        }

        public void SaveNow()
        {
            if (data == null || !isDirty)
            {
                return;
            }

            if (ProfileStorage.Save(data))
            {
                isDirty = false;
                autoSaveTimer = 0f;
            }
        }

        private StatisticRecord GetOrCreateStatistic(string id)
        {
            string normalizedId = NormalizeId(id);
            if (normalizedId == null)
            {
                return null;
            }

            if (statisticIndex.TryGetValue(normalizedId, out StatisticRecord existing))
            {
                return existing;
            }

            var created = new StatisticRecord { id = normalizedId };
            data.statistics.Add(created);
            statisticIndex.Add(normalizedId, created);
            return created;
        }

        private bool Unlock(
            string id,
            Dictionary<string, UnlockRecord> index,
            List<UnlockRecord> records,
            Action<string> unlockedEvent)
        {
            string normalizedId = NormalizeId(id);
            if (normalizedId == null || index.ContainsKey(normalizedId))
            {
                return false;
            }

            var record = new UnlockRecord
            {
                id = normalizedId,
                unlockedUtcTicks = DateTime.UtcNow.Ticks
            };

            records.Add(record);
            index.Add(normalizedId, record);
            MarkDirty();
            unlockedEvent?.Invoke(normalizedId);
            return true;
        }

        private void NotifyStatisticChanged(string id)
        {
            MarkDirty();
            StatisticChanged?.Invoke(id);
        }

        private void MarkDirty()
        {
            isDirty = true;
        }

        private void RebuildIndexes()
        {
            statisticIndex.Clear();
            achievementIndex.Clear();
            galleryIndex.Clear();

            IndexStatistics(data.statistics);
            IndexUnlocks(data.achievements, achievementIndex);
            IndexUnlocks(data.galleryEntries, galleryIndex);
        }

        private void IndexStatistics(List<StatisticRecord> records)
        {
            for (int index = records.Count - 1; index >= 0; index--)
            {
                StatisticRecord record = records[index];
                string id = record == null ? null : NormalizeId(record.id);
                if (id == null || !IsFinite(record.value) || statisticIndex.ContainsKey(id))
                {
                    records.RemoveAt(index);
                    continue;
                }

                record.id = id;
                statisticIndex.Add(id, record);
            }
        }

        private static void IndexUnlocks(
            List<UnlockRecord> records,
            Dictionary<string, UnlockRecord> index)
        {
            for (int recordIndex = records.Count - 1; recordIndex >= 0; recordIndex--)
            {
                UnlockRecord record = records[recordIndex];
                string id = record == null ? null : NormalizeId(record.id);
                if (id == null || index.ContainsKey(id))
                {
                    records.RemoveAt(recordIndex);
                    continue;
                }

                record.id = id;
                index.Add(id, record);
            }
        }

        private static string NormalizeId(string id)
        {
            return string.IsNullOrWhiteSpace(id) ? null : id.Trim();
        }

        private static bool IsFinite(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }

        private void OnApplicationPause(bool isPaused)
        {
            if (isPaused)
            {
                SaveNow();
            }
        }

        private void OnApplicationQuit()
        {
            SaveNow();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                SaveNow();
                Instance = null;
            }
        }
    }
}
