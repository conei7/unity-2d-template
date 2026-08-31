using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity2DTemplate.Features
{
    public enum StatisticDisplayFormat
    {
        Integer,
        Decimal,
        Duration
    }

    [Serializable]
    public sealed class StatisticDefinition
    {
        public string id;
        public string label;
        public StatisticDisplayFormat format;
        [Range(0, 4)] public int decimalPlaces = 1;
    }

    [CreateAssetMenu(fileName = "StatisticsCatalog", menuName = "Unity 2D Template/Statistics Catalog")]
    public sealed class StatisticsCatalog : ScriptableObject
    {
        [SerializeField] private List<StatisticDefinition> definitions =
            new List<StatisticDefinition>();

        public IReadOnlyList<StatisticDefinition> Definitions => definitions;
    }
}
