using System.Text;
using TMPro;
using UnityEngine;

namespace Unity2DTemplate.Features
{
    public sealed class CreditsPanelController : MonoBehaviour
    {
        [SerializeField] private CreditsCatalog catalog;
        [SerializeField] private TMP_Text bodyText;

        private void OnEnable()
        {
            Refresh();
        }

        public void Refresh()
        {
            if (bodyText == null)
            {
                return;
            }

            if (catalog == null)
            {
                bodyText.text = "CreditsCatalog is not assigned.";
                return;
            }

            var builder = new StringBuilder();
            for (int sectionIndex = 0; sectionIndex < catalog.Sections.Count; sectionIndex++)
            {
                CreditSection section = catalog.Sections[sectionIndex];
                if (section == null)
                {
                    continue;
                }

                if (builder.Length > 0)
                {
                    builder.AppendLine();
                }

                builder.AppendLine(section.heading);
                for (int lineIndex = 0; lineIndex < section.lines.Count; lineIndex++)
                {
                    builder.Append("  ").AppendLine(section.lines[lineIndex]);
                }
            }

            bodyText.text = builder.ToString().TrimEnd();
        }
    }
}
