using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity2DTemplate.Features
{
    [Serializable]
    public sealed class AchievementDefinition
    {
        public string id;
        public string title;
        [TextArea] public string description;
        public bool isSecret;
        public Sprite icon;
    }

    [CreateAssetMenu(fileName = "AchievementCatalog", menuName = "Unity 2D Template/Achievement Catalog")]
    public sealed class AchievementCatalog : ScriptableObject
    {
        [SerializeField] private List<AchievementDefinition> definitions =
            new List<AchievementDefinition>();

        public IReadOnlyList<AchievementDefinition> Definitions => definitions;

        public AchievementDefinition Find(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return null;
            }

            for (int index = 0; index < definitions.Count; index++)
            {
                AchievementDefinition definition = definitions[index];
                if (definition != null
                    && string.Equals(definition.id, id, StringComparison.Ordinal))
                {
                    return definition;
                }
            }

            return null;
        }
    }
}
