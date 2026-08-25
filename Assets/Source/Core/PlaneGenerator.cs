using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace PromvrTestTask
{
    using variablesID = PlaneGeneratorVariables;

    internal class PlaneGenerator
    {
        private const string DefaultMeshName = "PlaneMesh";

        private ComputeShader _planeGeneratorShader;

        private ComputeBuffer _verticesDataBuffer;
        private ComputeBuffer _trianglesBuffer;

        private int _generateVerticesKernelID;
        private int _generateTrianglesKernelID;

        private Vector2Int _threadGroupSizeVerticesKernel;
        private Vector2Int _threadGroupSizeTrianglesKernel;

        internal PlaneGenerator(ComputeShader planeGeneratorShader) => _planeGeneratorShader = planeGeneratorShader;

        internal Mesh Generate(float planeSideSize, int verticesPerSide, bool isRecalculateTangents = true, string meshName = DefaultMeshName)
        {
            var quadsPerSide = verticesPerSide - 1;

            var verticesCount = verticesPerSide * verticesPerSide;
            var trianglesCount = quadsPerSide * quadsPerSide * 6;

            FindKernels();

            CreateBuffers(verticesCount, trianglesCount);

            SetShaderData(planeSideSize, verticesPerSide, quadsPerSide);

            CalculateThreadGroupSizes(verticesPerSide, quadsPerSide);

            var mesh = CreateMesh(verticesCount, trianglesCount);

            ReleaseBuffers();

            mesh.name = meshName;

            mesh.RecalculateBounds();

            if(isRecalculateTangents)
                mesh.RecalculateTangents();

            return mesh;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void FindKernels()
        {
            _generateVerticesKernelID = _planeGeneratorShader.FindKernel("GenerateVertices");
            _generateTrianglesKernelID = _planeGeneratorShader.FindKernel("GenerateTriangles");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CreateBuffers(int verticesCount, int trianglesCount)
        {
            _verticesDataBuffer = new ComputeBuffer(verticesCount, UnsafeUtility.SizeOf<VertexData>());
            _trianglesBuffer = new ComputeBuffer(trianglesCount, sizeof(int));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetShaderData(float planeSideSize, int verticesPerSide, int quadsPerSide)
        {
            _planeGeneratorShader.SetFloat(variablesID.planeSideSize, planeSideSize);

            _planeGeneratorShader.SetInt(variablesID.verticesPerSide, verticesPerSide);
            _planeGeneratorShader.SetInt(variablesID.quadsPerSide, quadsPerSide);

            _planeGeneratorShader.SetBuffer(_generateVerticesKernelID, variablesID.verticesData, _verticesDataBuffer);
            _planeGeneratorShader.SetBuffer(_generateTrianglesKernelID, variablesID.triangles, _trianglesBuffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CalculateThreadGroupSizes(int verticesPerSide, int quadsPerSide)
        {
            _planeGeneratorShader.GetKernelThreadGroupSizes(_generateVerticesKernelID, out var x, out var y, out _);

            _threadGroupSizeVerticesKernel = new Vector2Int(
                Mathf.CeilToInt((float)verticesPerSide / (float)x),
                Mathf.CeilToInt((float)verticesPerSide / (float)y));

            _planeGeneratorShader.GetKernelThreadGroupSizes(_generateTrianglesKernelID, out x, out y, out _);

            _threadGroupSizeTrianglesKernel = new Vector2Int(
                Mathf.CeilToInt((float)quadsPerSide / (float)x),
                Mathf.CeilToInt((float)quadsPerSide / (float)y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Mesh CreateMesh(int verticesCount, int trianglesCount)
        {
            _planeGeneratorShader.Dispatch(_generateVerticesKernelID, _threadGroupSizeVerticesKernel.x, _threadGroupSizeVerticesKernel.y, 1);
            _planeGeneratorShader.Dispatch(_generateTrianglesKernelID, _threadGroupSizeTrianglesKernel.x, _threadGroupSizeTrianglesKernel.y, 1);

            var verticesData = new VertexData[verticesCount];
            var triangles = new int[trianglesCount];

            _verticesDataBuffer.GetData(verticesData);
            _trianglesBuffer.GetData(triangles);

            var vertices = new Vector3[verticesCount];
            var normals = new Vector3[verticesCount];
            var uv = new Vector2[verticesCount];

            for (int i = 0; i < verticesData.Length; i++)
            {
                vertices[i] = verticesData[i].position;
                normals[i] = verticesData[i].normal;
                uv[i] = verticesData[i].uv;
            }

            var mesh = new Mesh();

            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.uv = uv;
            mesh.triangles = triangles;

            return mesh;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ReleaseBuffers()
        {
            _verticesDataBuffer?.Release();
            _trianglesBuffer?.Release();
        }
    }
}
