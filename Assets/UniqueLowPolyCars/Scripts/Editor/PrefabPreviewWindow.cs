using UniqueLowPolyCars.Scripts.Editor.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace UniqueLowPolyCars.Scripts.Editor {
    public class PrefabPreviewWindow : EditorWindow {
        private string _folderPath = "Assets/UniqueLowPolyCars/Prefabs";
        private PrefabListView _prefabListView;

        [MenuItem("BabyCheese/LowPolyVegetation/Prefab Preview")]
        public static void ShowWindow() {
            var window = GetWindow<PrefabPreviewWindow>();
            window.titleContent = new GUIContent("BabyCheese Prefab Preview");
            window.minSize = new Vector2(256, 409);
        }

        private void OnEnable() {
            SceneView.duringSceneGui += OnDuringSceneGUI;
        }

        private void OnDisable() {
            SceneView.duringSceneGui -= OnDuringSceneGUI;
            _prefabListView?.ClearPrefabs();
        }

        public void CreateGUI() {
            rootVisualElement.AddStyleSheetFromResources("PrefabPreviewWindow");
            SetupPrefabListView();
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

        private void SetupPrefabListView() {
            _prefabListView = new PrefabListView();
            rootVisualElement.Add(_prefabListView);
        }

        private void CreateControlButtons() {
            var selectPathButton = new Button(OnSelectPath) {
                text = "Select Prefabs Path"
            };
            selectPathButton.AddToClassList("select-button");
            rootVisualElement.Add(selectPathButton);
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
            LoadPrefabs(_folderPath);
        }

        private void LoadPrefabs(string folderPath) {
            _prefabListView.ClearPrefabs(); // Clear existing views before loading new ones

            var prefabGUIDs = AssetDatabase.FindAssets("t:Prefab", new[] { folderPath });
            foreach (string guid in prefabGUIDs) {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                _prefabListView.AddPrefab(prefab);
            }
        }
    }
}