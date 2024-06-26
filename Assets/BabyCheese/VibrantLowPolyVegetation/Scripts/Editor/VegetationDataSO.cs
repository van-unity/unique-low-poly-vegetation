using UnityEngine;

namespace VibrantLowPolyVegetation.Scripts.Editor {
    public enum VegetationType {
        PotWithFlowers = 0,
        Composition = 1,
        Tree = 2,
        Pine = 3,
        Palm = 4,
        Sculpt = 5,
        Parts = 6,
        EmptyPots
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