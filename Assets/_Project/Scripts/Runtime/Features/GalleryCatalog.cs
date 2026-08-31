using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity2DTemplate.Features
{
    [Serializable]
    public sealed class GalleryEntryDefinition
    {
        public string id;
        public string title;
        public string creator;
        [TextArea] public string description;
        public bool unlockedByDefault = true;
        public Sprite image;
        public AudioClip audioClip;
    }

    [CreateAssetMenu(fileName = "GalleryCatalog", menuName = "Unity 2D Template/Gallery Catalog")]
    public sealed class GalleryCatalog : ScriptableObject
    {
        [SerializeField] private List<GalleryEntryDefinition> entries =
            new List<GalleryEntryDefinition>();

        public IReadOnlyList<GalleryEntryDefinition> Entries => entries;
    }
}
