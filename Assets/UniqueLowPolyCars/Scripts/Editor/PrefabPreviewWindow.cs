using System.Linq;
using UniqueLowPolyCars.Scripts.Editor.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace UniqueLowPolyCars.Scripts.Editor {
    public class PrefabPreviewWindow : EditorWindow {
        private PrefabListView _prefabListView;
        private VegetationDataSO _data;
        private Label _totalNumberOfPrefabsLabel;

        [MenuItem("BabyCheese/LowPolyVegetation/Prefab Preview")]
        public static void ShowWindow() {
            var window = GetWindow<PrefabPreviewWindow>();
            window.titleContent = new GUIContent("BabyCheese Prefab Preview");
            window.minSize = new Vector2(412, 640);
        }

        private void OnEnable() {
            _data = Resources.Load<VegetationDataSO>("VegetationData");

            SceneView.duringSceneGui += OnDuringSceneGUI;
            TabView.TabClicked += TabViewOnTabClicked;
        }

        private void TabViewOnTabClicked(VegetationType vegetationType) {
            _prefabListView.ClearPrefabs();
            var prefabs = _data.data.First(data => data.vegetationType == vegetationType).prefabs;
            foreach (var prefab in prefabs) {
                _prefabListView.AddPrefab(prefab);
            }

            _totalNumberOfPrefabsLabel.text = $"Prefab Count: {prefabs.Length}";
        }

        private void OnDisable() {
            SceneView.duringSceneGui -= OnDuringSceneGUI;
            TabView.TabClicked -= TabViewOnTabClicked;
            _prefabListView?.ClearPrefabs();
        }

        public void CreateGUI() {
            var backgroundImage = new Image() {
                image = Resources.Load<Texture>("baby-cheese-logo")
            };
            
            backgroundImage.AddToClassList("background-image");
            rootVisualElement.Add(backgroundImage);
            
            rootVisualElement.AddStyleSheetFromResources("PrefabPreviewWindow");
            rootVisualElement.AddToClassList("prefab-preview-window");
            SetupTabView();
            SetupPrefabListView();
            var labelContainer = new VisualElement();
            labelContainer.AddToClassList("bottom-panel");
            _totalNumberOfPrefabsLabel = new Label("Prefab Count: {0}");
            _totalNumberOfPrefabsLabel.AddToClassList("total-number-label");
            labelContainer.Add(_totalNumberOfPrefabsLabel);
            rootVisualElement.Add(labelContainer);
        }

        private void OnDuringSceneGUI(SceneView sceneView) {
            Event currentEvent = Event.current;
            if (currentEvent.type == EventType.DragPerform) {
                DragAndDrop.AcceptDrag();
                GameObject droppedObject =
                    PrefabUtility.InstantiatePrefab(DragAndDrop.objectReferences[0]) as GameObject;
                if (droppedObject != null) {
                    Vector3 dropPosition = ConvertMousePositionToWorldPosition(currentEvent, sceneView);

                    // Begin undo group
                    Undo.SetCurrentGroupName("Instantiate Prefab from Editor Tool");
                    int groupIndex = Undo.GetCurrentGroup();

                    // Set position and register the undo operation
                    Undo.RecordObject(droppedObject.transform, "Set Position");
                    droppedObject.transform.position = dropPosition;
                    Undo.RegisterCreatedObjectUndo(droppedObject, "Instantiate Prefab");

                    // Collapse all operations into a single undo
                    Undo.CollapseUndoOperations(groupIndex);
                }

                currentEvent.Use(); // Mark the event as used
            }
        }

        private Vector3 ConvertMousePositionToWorldPosition(Event eventArgs, SceneView sceneView) {
            Ray mouseRay = HandleUtility.GUIPointToWorldRay(eventArgs.mousePosition);

            bool hitDetected = Physics.Raycast(mouseRay, out RaycastHit hitInfo);

            float targetDistance = hitDetected ? hitInfo.distance : sceneView.camera.nearClipPlane + 10f;

            return mouseRay.GetPoint(targetDistance);
        }

        private void SetupTabView() {
            var tabView = new TabView(_data.data
                .Select(data => new TabViewButtonData(data.vegetationType, data.iconTexture)).ToArray());
            rootVisualElement.Add(tabView);
        }

        private void SetupPrefabListView() {
            _prefabListView = new PrefabListView();
            rootVisualElement.Add(_prefabListView);
        }
    }
}