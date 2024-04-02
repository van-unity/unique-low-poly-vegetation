using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using VibrantLowPolyVegetation.Scripts.Editor.Extensions;
using Object = UnityEngine.Object;

namespace VibrantLowPolyVegetation.Scripts.Editor {
    public class PrefabView : VisualElement {
        private readonly Image _previewImage;

        public PrefabView(GameObject prefab, int width = 256, int height = 256) {
            this.AddStyleSheetFromResources("PrefabView");
            AddToClassList("prefab-view");
            style.width = width;
            style.height = height;
            _previewImage = new Image();
            _previewImage.AddToClassList("imgui-container");
            
            Add(_previewImage);

            RegisterCallback<MouseDownEvent>(e => {
                DragAndDrop.PrepareStartDrag();
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy; // Show a visual cue
                DragAndDrop.objectReferences = new Object[] { prefab };
                DragAndDrop.StartDrag(prefab.name);
                e.StopPropagation();
            });

            var meshInfo = prefab.GetMeshInfo();
            var label = new Label($"Tris: {meshInfo.Item1}, Verts: {meshInfo.Item2}");
            label.AddToClassList("label");
            Add(label);

            RegisterCallback<MouseUpEvent>(e => {
                DragAndDrop.PrepareStartDrag(); // Reset the drag operation
            });

            this.schedule.Execute(() => {
                var texture = AssetPreview.GetAssetPreview(prefab);
                if (texture != null)
                    SetPreviewTexture(texture);
            }).Until(() => !AssetPreview.IsLoadingAssetPreview(prefab.GetInstanceID()));
        }

        public new void Clear() {
            if (_previewImage.image) {
                Object.DestroyImmediate(_previewImage.image);
            }

            base.Clear();
        }

        private void SetPreviewTexture(Texture previewTexture) {
            this.schedule.Execute(() => {
                _previewImage.image = previewTexture;
                var opacityDelay = Random.Range(0.1f, .6f);
                var scaleDelay = opacityDelay + Random.Range(.1f, .2f);
                this.style.transitionDelay = new StyleList<TimeValue>(new List<TimeValue>() {
                    new(opacityDelay, TimeUnit.Second), new(scaleDelay, TimeUnit.Second)
                });
                this.AddToClassList("show-image");
            }).ExecuteLater(20);
        }
    }
}