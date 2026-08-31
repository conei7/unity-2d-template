using System;
using System.Collections.Generic;

namespace Unity2DTemplate.Features
{
    [Serializable]
    public sealed class StatisticRecord
    {
        public string id;
        public double value;
    }

    [Serializable]
    public sealed class UnlockRecord
    {
        public string id;
        public long unlockedUtcTicks;
    }

    [Serializable]
    public sealed class ProfileData
    {
        public int version = 1;
        public long revision;
        public string integrityMarker;
        public List<StatisticRecord> statistics = new List<StatisticRecord>();
        public List<UnlockRecord> achievements = new List<UnlockRecord>();
        public List<UnlockRecord> galleryEntries = new List<UnlockRecord>();
    }
}
