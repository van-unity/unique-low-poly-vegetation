using System;
using UnityEngine;

namespace UniqueLowPolyCars.Scripts.Editor.Extensions {
    public static class GameObjectExtensions {
        public static Tuple<int, int> GetMeshInfo(this GameObject selectedGameObject) {
            var meshFilters = selectedGameObject.GetComponentsInChildren<MeshFilter>();
            var totalTriangles = 0;
            var totalVertices = 0;

            foreach (MeshFilter mf in meshFilters) {
                if (mf.sharedMesh != null) {
                    var sharedMesh = mf.sharedMesh;
                    totalTriangles += sharedMesh.triangles.Length / 3; // Each triangle is 3 indices
                    totalVertices += sharedMesh.vertexCount;
                }
            }

            return new Tuple<int, int>(totalTriangles, totalVertices);
        }
    }
}