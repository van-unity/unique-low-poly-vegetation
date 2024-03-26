using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace UniqueLowPolyCars.Scripts.Editor {
    public class PrefabPreviewWindow : EditorWindow {
        private string _folderPath = "Assets/UniqueLowPolyCars/Prefabs";
        private readonly List<GameObject> _prefabs = new();
        private VisualElement _prefabListView;
        private readonly List<PrefabView> _prefabViews = new();

        [MenuItem("BabyCheese/LowPolyVegetation/Prefab Preview")]
        public static void ShowWindow() {
            var window = GetWindow<PrefabPreviewWindow>("BabyCheese Prefab Preview");
            window.minSize = new Vector2(256, 409);
        }

        private void OnEnable() {
            SceneView.duringSceneGui += OnDuringSceneGUI;
        }

        private void OnDisable() {
            SceneView.duringSceneGui -= OnDuringSceneGUI;
            ClearPrefabViews();
        }

        public void CreateGUI() {
            LoadStylesheet();
            CreateScrollView();
            CreateControlButtons();
            LoadPrefabsFromDefaultPath();
        }

        private void OnDuringSceneGUI(SceneView sceneView) {
            Event currentEvent = Event.current;
            if (currentEvent.type == EventType.DragPerform) {
                DragAndDrop.AcceptDrag();
                GameObject droppedObject =
                    PrefabUtility.InstantiatePrefab(DragAndDrop.objectReferences[0]) as GameObject;
                if (droppedObject != null) {
                    droppedObject.transform.position = ConvertMousePositionToWorldPosition(currentEvent, sceneView);
                }

                currentEvent.Use();
            }
        }

        private Vector3 ConvertMousePositionToWorldPosition(Event eventArgs, SceneView sceneView) {
            Ray mouseRay = HandleUtility.GUIPointToWorldRay(eventArgs.mousePosition);
            bool hitDetected = Physics.Raycast(mouseRay, out RaycastHit hitInfo);
            float targetDistance = hitDetected ? hitInfo.distance : sceneView.camera.nearClipPlane + 10f;
            return mouseRay.GetPoint(targetDistance);
        }

        private void LoadStylesheet() {
            StyleSheet stylesheet = Resources.Load<StyleSheet>("PrefabPreviewWindow");
            if (stylesheet != null) {
                rootVisualElement.styleSheets.Add(stylesheet);
            }
        }

        private void CreateScrollView() {
            _prefabListView = new ScrollView { name = "prefabListView" };
            _prefabListView.contentContainer.AddToClassList("prefab-list");
            rootVisualElement.Add(_prefabListView);
        }

        private void CreateControlButtons() {
            var selectButton = new Button(OnSelectPath) { text = "Select Prefabs Path" };
            selectButton.AddToClassList("select-button");
            rootVisualElement.Add(selectButton);

            var clearButton = new Button(ClearPrefabs) { text = "Clear List" };
            clearButton.AddToClassList("select-button");
            rootVisualElement.Add(clearButton);
        }

        private void OnSelectPath() {
            string path = EditorUtility.OpenFolderPanel("Select Prefabs Path", "Assets", "");
            if (!string.IsNullOrEmpty(path) && path.StartsWith(Application.dataPath)) {
                _folderPath = "Assets" + path.Substring(Application.dataPath.Length);
                LoadPrefabsFromSelectedPath();
            }
        }

        private void LoadPrefabsFromDefaultPath() {
            LoadPrefabs(_folderPath);
        }

        private void LoadPrefabsFromSelectedPath() {
            ClearPrefabViews();
            LoadPrefabs(_folderPath);
        }

        private void LoadPrefabs(string folderPath) {
            var prefabGUIDs = AssetDatabase.FindAssets("t:Prefab", new[] { folderPath });
            foreach (string guid in prefabGUIDs) {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                AddPrefabToList(prefab);
            }
        }

        private void AddPrefabToList(GameObject prefab) {
            if (prefab == null) return;
            _prefabs.Add(prefab);
            var prefabView = new PrefabView(prefab, 128, 128);
            _prefabViews.Add(prefabView);
            _prefabListView.Add(prefabView);
        }

        private void ClearPrefabs() {
            _prefabs.Clear();
            ClearPrefabViews();
        }

        private void ClearPrefabViews() {
            foreach (var view in _prefabViews) {
                view.Clear();
            }

            _prefabViews.Clear();
            _prefabListView?.Clear();
        }
    }
}