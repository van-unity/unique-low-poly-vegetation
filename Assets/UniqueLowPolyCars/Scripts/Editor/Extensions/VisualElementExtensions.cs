using UnityEngine;
using UnityEngine.UIElements;

namespace UniqueLowPolyCars.Scripts.Editor.Extensions {
    public static class VisualElementExtensions {
        public static void AddStyleSheetFromResources(this VisualElement visualElement, string stylePath) {
            var styleSheet = Resources.Load<StyleSheet>(stylePath);
            if (styleSheet != null) {
                visualElement.styleSheets.Add(styleSheet);
            }
        }
    }
}