using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity2DTemplate.Features
{
    [Serializable]
    public sealed class CreditSection
    {
        public string heading;
        public List<string> lines = new List<string>();
    }

    [CreateAssetMenu(fileName = "CreditsCatalog", menuName = "Unity 2D Template/Credits Catalog")]
    public sealed class CreditsCatalog : ScriptableObject
    {
        [SerializeField] private List<CreditSection> sections = new List<CreditSection>();

        public IReadOnlyList<CreditSection> Sections => sections;
    }
}
