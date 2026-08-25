using System.Runtime.CompilerServices;
using UnityEngine;

namespace PromvrTestTask
{
    using variablesID = FillingHeightMapVariables;

    internal class HeightMapUpdater
    {
        private ComputeShader _fillingMapShader;

        private int _updateMapKernelID;
        private int _resetMapKernelID;

        private Vector2Int _threadGroupSize;

        private Vector3 _previousWorldSphere;
        private Vector3 _currentWorldSphere;

        internal HeightMapUpdater(ComputeShader fillingMapShader) => _fillingMapShader = fillingMapShader;

        internal void Initialize(RenderTexture texture, float planeSize)
        {
            FindKernels();
            CalculateThreadGroupSizes(texture.width);
            SetConstantData(texture, planeSize);
        }

        internal void Restart(Vector3 spherePosition, float sphereRadius)
        {
            CalculateSphere(ref _previousWorldSphere, spherePosition, sphereRadius);
            _currentWorldSphere = _previousWorldSphere;
        }

        internal void Update(Vector3 spherePosition, float sphereRadius, float accumulationSpeed, float deltaTime)
        {
            SetSpheres(spherePosition, sphereRadius);

            _fillingMapShader.SetFloat(variablesID.accumulationMaterialSpeed, accumulationSpeed);
            _fillingMapShader.SetFloat(variablesID.deltaTime, deltaTime);

            _fillingMapShader.Dispatch(_updateMapKernelID, _threadGroupSize.x, _threadGroupSize.y, 1);
        }

        internal void Reset() => _fillingMapShader.Dispatch(_resetMapKernelID, _threadGroupSize.x, _threadGroupSize.y, 1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void FindKernels()
        {
            _updateMapKernelID = _fillingMapShader.FindKernel("UpdateMap");
            _resetMapKernelID = _fillingMapShader.FindKernel("ResetMap");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CalculateThreadGroupSizes(int textureSize)
        {
            _fillingMapShader.GetKernelThreadGroupSizes(_updateMapKernelID, out var x, out var y, out _);

            _threadGroupSize = new Vector2Int(
                Mathf.CeilToInt(textureSize / (int)x),
                Mathf.CeilToInt(textureSize / (int)y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetConstantData(RenderTexture texture, float planeSize)
        {
            _fillingMapShader.SetTexture(_updateMapKernelID, variablesID.heightMap, texture);
            _fillingMapShader.SetTexture(_resetMapKernelID, variablesID.heightMap, texture);

            _fillingMapShader.SetInt(variablesID.textureSize, texture.width);
            _fillingMapShader.SetFloat(variablesID.planeSize, planeSize);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetSpheres(Vector3 spherePosition, float sphereRadius)
        {
            _previousWorldSphere = _currentWorldSphere;
            CalculateSphere(ref _currentWorldSphere, spherePosition, sphereRadius);

            _fillingMapShader.SetVector(variablesID.previousWorldSphere, _previousWorldSphere);
            _fillingMapShader.SetVector(variablesID.currentWorldSphere, _currentWorldSphere);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CalculateSphere(ref Vector3 sphere, Vector3 spherePosition, float sphereRadius)
        {
            sphere.x = spherePosition.x;
            sphere.y = spherePosition.z;
            sphere.z = sphereRadius;
        }
    }
}
