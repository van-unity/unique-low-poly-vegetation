using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using VibrantLowPolyVegetation.Scripts.Editor.Extensions;

namespace VibrantLowPolyVegetation.Scripts.Editor {
    public class PrefabListView : ScrollView {
        private readonly List<PrefabView> _prefabViews = new();

        public PrefabListView() {
            this.AddStyleSheetFromResources("PrefabListView");
            this.name = "prefabListView";
            this.AddToClassList("prefab-list-view");
            this.contentContainer.AddToClassList("container"); // Ensure this class is defined in your USS for styling
        }

        public void AddPrefab(GameObject prefab) {
            var prefabView = new PrefabView(prefab, 128, 128); // Adjust size as needed
            _prefabViews.Add(prefabView);
            this.Add(prefabView);
        }

        public void ClearPrefabs() {
            foreach (var view in _prefabViews) {
                view.Clear(); // Ensure PrefabView implements Clear properly to clean up resources
            }

            _prefabViews.Clear();
            this.Clear(); // Clears the ScrollView content
        }
    }
}