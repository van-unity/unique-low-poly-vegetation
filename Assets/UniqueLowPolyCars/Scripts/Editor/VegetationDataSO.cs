using UnityEngine;

namespace UniqueLowPolyCars.Scripts.Editor {
    public enum VegetationType {
        Pot = 0,
        Composition = 1,
        Tree = 2,
        Pine = 3,
        Palm = 4,
        Sculpt = 5,
        Parts = 6
    }

    [System.Serializable]
    public class VegetationData {
        public VegetationType vegetationType;
        public GameObject[] prefabs;
        public Texture iconTexture;
        
        public int PrefabCount => prefabs.Length;
    }

    [CreateAssetMenu(fileName = "VegetationData", menuName = "BabyCheese/VegetationData", order = 0)]
    public class VegetationDataSO : ScriptableObject {
        public VegetationData[] data;
    }
}