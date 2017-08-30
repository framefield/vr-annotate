using UnityEngine;
using System.Collections;
using ff.nodegraph;

namespace ff.utils
{
    public class GenerateMeshFromBounds
    {

        static public Mesh GenerateMesh(Node.BoundsWithContextStruct[] boundsWithContext, Mesh mesh = null)
        {
            _boundsWithContext = boundsWithContext;

            if (boundsWithContext.Length > MAX_BOUNDS_POSSIBLE)
            {
                Debug.LogWarning("list of bounds too large to convert to mesh");
            }

            var cubeCount = Mathf.Min(boundsWithContext.Length, MAX_BOUNDS_POSSIBLE);

            _triangles = new int[cubeCount * TRIANGLES_PER_CUBE];
            _vertices = new Vector3[cubeCount * VERTICES_PER_CUBE];

            for (int cubeIndex = 0; cubeIndex < cubeCount; cubeIndex++)
            {
                WriteVertices(cubeIndex);
                WriteTriangles(cubeIndex);
            }

            if (mesh == null)
            {
                mesh = new Mesh();
                mesh.Clear();
            }

            mesh.vertices = _vertices;
            //mesh.normals = GetNormals();
            //mesh.uv = GetUVsMap();
            mesh.triangles = _triangles;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            return mesh;
        }

        static int MAX_BOUNDS_POSSIBLE = 64000 / 2 / 12;
        static int VERTICES_PER_CUBE = 6 * 4;
        static int TRIANGLES_PER_CUBE = 6 * 2 * 3;
        static Vector3[] _vertices;
        static int[] _triangles;
        static Node.BoundsWithContextStruct[] _boundsWithContext;

        private static void WriteVertices(int index)
        {
            var bwc = _boundsWithContext[index];

            var b = bwc.HasLocalBounds ? bwc.LocalBounds : bwc.Bounds;

            var sx = b.extents.x;
            var sy = b.extents.y;
            var sz = b.extents.z;

            var x = b.center.x;
            var y = b.center.y;
            var z = b.center.z;

            var vertice_0 = new Vector3(x - sx, y - sy, z + sz);
            var vertice_1 = new Vector3(x + sx, y - sy, z + sz);
            var vertice_2 = new Vector3(x + sx, y - sy, z - sz);
            var vertice_3 = new Vector3(x - sx, y - sy, z - sz);
            var vertice_4 = new Vector3(x - sx, y + sy, z + sz);
            var vertice_5 = new Vector3(x + sx, y + sy, z + sz);
            var vertice_6 = new Vector3(x + sx, y + sy, z - sz);
            var vertice_7 = new Vector3(x - sx, y + sy, z - sz);

            if (bwc.HasLocalBounds)
            {
                var t = bwc.LocalTransform;

                vertice_0 = t.TransformPoint(vertice_0);
                vertice_1 = t.TransformPoint(vertice_1);
                vertice_2 = t.TransformPoint(vertice_2);
                vertice_3 = t.TransformPoint(vertice_3);
                vertice_4 = t.TransformPoint(vertice_4);
                vertice_5 = t.TransformPoint(vertice_5);
                vertice_6 = t.TransformPoint(vertice_6);
                vertice_7 = t.TransformPoint(vertice_7);
            }

            int i = index * VERTICES_PER_CUBE;
            // Bottom Polygon
            _vertices[i + 0] = vertice_0;
            _vertices[i + 1] = vertice_1;
            _vertices[i + 2] = vertice_2;
            _vertices[i + 3] = vertice_3;
            // Left Polygon
            _vertices[i + 4] = vertice_7;
            _vertices[i + 5] = vertice_4;
            _vertices[i + 6] = vertice_0;
            _vertices[i + 7] = vertice_3;
            // Front Polygon
            _vertices[i + 8] = vertice_4;
            _vertices[i + 9] = vertice_5;
            _vertices[i + 10] = vertice_1;
            _vertices[i + 11] = vertice_0;
            // Back Polygon
            _vertices[i + 12] = vertice_6;
            _vertices[i + 13] = vertice_7;
            _vertices[i + 14] = vertice_3;
            _vertices[i + 15] = vertice_2;
            // Right Polygon
            _vertices[i + 16] = vertice_5;
            _vertices[i + 17] = vertice_6;
            _vertices[i + 18] = vertice_2;
            _vertices[i + 19] = vertice_1;
            // Top Polygon
            _vertices[i + 20] = vertice_7;
            _vertices[i + 21] = vertice_6;
            _vertices[i + 22] = vertice_5;
            _vertices[i + 23] = vertice_4;
        }


        private static void WriteTriangles(int cubeIndex)
        {
            var triangleIndex = cubeIndex * TRIANGLES_PER_CUBE;
            var vertexIndex = cubeIndex * VERTICES_PER_CUBE;
            for (int i = 0; i < TRIANGE_INDECES.Length; i++)
            {
                _triangles[i + triangleIndex] = TRIANGE_INDECES[i] + vertexIndex;
            }
        }


        static int[] TRIANGE_INDECES = new int[]
        {
            // Cube Bottom Side Triangles
            3, 1, 0,
            3, 2, 1,			

            // Cube Left Side Triangles
            3 + 4 * 1, 1 + 4 * 1, 0 + 4 * 1,
            3 + 4 * 1, 2 + 4 * 1, 1 + 4 * 1,

            // Cube Front Side Triangles
            3 + 4 * 2, 1 + 4 * 2, 0 + 4 * 2,
            3 + 4 * 2, 2 + 4 * 2, 1 + 4 * 2,

            // Cube Back Side Triangles
            3 + 4 * 3, 1 + 4 * 3, 0 + 4 * 3,
            3 + 4 * 3, 2 + 4 * 3, 1 + 4 * 3,

            // Cube Rigth Side Triangles
            3 + 4 * 4, 1 + 4 * 4, 0 + 4 * 4,
            3 + 4 * 4, 2 + 4 * 4, 1 + 4 * 4,

            // Cube Top Side Triangles
            3 + 4 * 5, 1 + 4 * 5, 0 + 4 * 5,
            3 + 4 * 5, 2 + 4 * 5, 1 + 4 * 5,
        };
    }
}