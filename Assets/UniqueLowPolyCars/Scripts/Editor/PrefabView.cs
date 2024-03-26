using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace UniqueLowPolyCars.Scripts.Editor {
    public class PrefabView : VisualElement {
        private Texture _texture;

        public PrefabView(GameObject prefab, int width = 256, int height = 256) {
            styleSheets.Add(Resources.Load<StyleSheet>("PrefabView"));
            style.width = width;
            style.height = height;
            AddToClassList("prefab-view");
            // var path = Path.Combine(Application.dataPath, "texturetest.png");
            // File.WriteAllBytes(path, texture.EncodeToPNG());
            var image = new Image();

            image.AddToClassList("imgui-container");
            Add(image);
            var label = new Label(prefab.name);
            label.AddToClassList("label");
            Add(label);

            RegisterCallback<MouseDownEvent>(e => {
                DragAndDrop.PrepareStartDrag();
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy; // Show a visual cue
                DragAndDrop.objectReferences = new Object[] { prefab };
                DragAndDrop.StartDrag(prefab.name);
                e.StopPropagation();
            });

            RegisterCallback<MouseUpEvent>(e => {
                DragAndDrop.PrepareStartDrag(); // Reset the drag operation
            });

            this.schedule.Execute(() => {
                _texture = AssetPreview.GetAssetPreview(prefab);
                if (_texture != null)
                    image.image = _texture;
            }).Until(() => !AssetPreview.IsLoadingAssetPreview(prefab.GetInstanceID()));
        }

        public new void Clear() {
            if (_texture) {
                Object.DestroyImmediate(_texture);
            }

            base.Clear();
        }
    }
}