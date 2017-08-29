using UnityEngine;
using System.Collections;

namespace ff.utils
{
    public class GenerateMeshFromBounds
    {
        static public Mesh GenerateMesh(Bounds[] bounds, Transform[] transforms, bool[] isLocals, Mesh mesh = null)
        {
            _bounds = bounds;
            _transforms = transforms;
            _isLocals = isLocals;

            if (bounds.Length > MAX_BOUNDS_POSSIBLE)
            {
                Debug.LogWarning("list of bounds too large to convert to mesh");
            }

            var cubeCount = Mathf.Min(bounds.Length, MAX_BOUNDS_POSSIBLE);

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
        static Bounds[] _bounds;
        static Transform[] _transforms;
        static bool[] _isLocals;

        private static void WriteVertices(int index)
        {
            var b = _bounds[index];

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

            if (_isLocals[index])
            {
                var t = _transforms[index];

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

        // private Vector3[] GetNormals()
        // {


        //     Vector3[] normales = new Vector3[]
        //     {
        //     down, down, down, down,// Bottom Side Render
        //     left, left, left, left,// LEFT Side Render
        //     front, front, front, front,// FRONT Side Render
        //     back, back, back, back,// BACK Side Render
        //     right, right, right, right,// RIGTH Side Render
        //     up, up, up, up// UP Side Render
        //     };
        //     return normales;
        // }

        // private Vector2[] GetUVsMap()
        // {

        //     Vector2[] uvs = new Vector2[]
        //             {
        //             _11_CORDINATES, _01_CORDINATES, _00_CORDINATES, _10_CORDINATES,// Bottom
        // 			_11_CORDINATES, _01_CORDINATES, _00_CORDINATES, _10_CORDINATES,// Left
        // 			_11_CORDINATES, _01_CORDINATES, _00_CORDINATES, _10_CORDINATES,// Front
        // 			_11_CORDINATES, _01_CORDINATES, _00_CORDINATES, _10_CORDINATES,// Back
        // 			_11_CORDINATES, _01_CORDINATES, _00_CORDINATES, _10_CORDINATES,// Right
        // 			_11_CORDINATES, _01_CORDINATES, _00_CORDINATES, _10_CORDINATES,// Top
        //             };
        //     return uvs;
        // }

        private static void WriteTriangles(int cubeIndex)
        {
            var triangleIndex = cubeIndex * TRIANGLES_PER_CUBE;
            var vertexIndex = cubeIndex * VERTICES_PER_CUBE;
            for (int i = 0; i < TRIANGE_INDECES.Length; i++)
            {
                _triangles[i + triangleIndex] = TRIANGE_INDECES[i] + vertexIndex;
            }
        }

        // private Mesh GetCubeMesh()
        // {
        //     if (GetComponent<MeshFilter>() == null)
        //     {
        //         Mesh mesh;
        //         MeshFilter filter = gameObject.AddComponent<MeshFilter>();
        //         mesh = filter.mesh;
        //         mesh.Clear();
        //         return mesh;
        //     }
        //     else
        //     {
        //         return gameObject.AddComponent<MeshFilter>().mesh;
        //     }
        // }

        // void MakeCube()
        // {
        //     cubeMesh = GetCubeMesh();
        //     cubeMesh.vertices = GetVerticesForBound();
        //     cubeMesh.normals = GetNormals();
        //     cubeMesh.uv = GetUVsMap();
        //     cubeMesh.triangles = GetTriangles();
        //     cubeMesh.RecalculateBounds();
        //     cubeMesh.RecalculateNormals();
        // }


        // Vector3 up = Vector3.up;
        // Vector3 down = Vector3.down;
        // Vector3 front = Vector3.forward;
        // Vector3 back = Vector3.back;
        // Vector3 left = Vector3.left;
        // Vector3 right = Vector3.right;

        // Vector2 _00_CORDINATES = new Vector2(0f, 0f);
        // Vector2 _10_CORDINATES = new Vector2(1f, 0f);
        // Vector2 _01_CORDINATES = new Vector2(0f, 1f);
        // Vector2 _11_CORDINATES = new Vector2(1f, 1f);

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