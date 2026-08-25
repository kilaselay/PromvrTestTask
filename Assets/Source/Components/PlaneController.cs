using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace PromvrTestTask
{
    public class PlaneController : MonoBehaviour
    {
        [SerializeField]
        private ComputeShader _planeGeneratorShader;

        [SerializeField, Min(1)]
        private float _planeSideSize = 10f;

        [SerializeField, Min(2)]
        private int _verticesPerSide = 10;

        [SerializeField]
        private Material _planeMaterial;

        [SerializeField]
        private LayerMask _layerMask;

#if UNITY_EDITOR
        [SerializeField, Space, Header("Editor Settings")]
        private bool _isDrawGizmo = true;

        [SerializeField]
        private bool _isDebuggingMode = false;
#endif

        private Mesh _planeMesh;

        private MaterialPropertyBlock _materialProperties;

        private Matrix4x4 _meshMatrix;

        public float PlaneSideSize => _planeSideSize;

        public void Initialize()
        {
            Validate();

            _planeGeneratorShader = Instantiate(_planeGeneratorShader);

            var planeGenerator = new PlaneGenerator(_planeGeneratorShader);

            _planeMesh = planeGenerator.Generate(_planeSideSize, _verticesPerSide);

            _materialProperties = new MaterialPropertyBlock();

            _meshMatrix = Matrix4x4.TRS(transform.position, Quaternion.identity, Vector3.one);

#if UNITY_EDITOR
            if (!_isDebuggingMode)
                return;

            var meshFilter = gameObject.AddComponent<MeshFilter>();
            meshFilter.mesh = _planeMesh;

            var meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer.material = _planeMaterial;
#endif
        }

        public void SetData(RenderTexture heightMap, float heightWeight)
        {
            _materialProperties.SetTexture("_HeightMap", heightMap);
            _materialProperties.SetFloat("_HeightWeight", heightWeight);

#if UNITY_EDITOR
            if (!_isDebuggingMode)
                return;

            _planeMaterial.SetTexture("_HeightMap", heightMap);
            _planeMaterial.SetFloat("_HeightWeight", heightWeight);
#endif
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (_isDebuggingMode)
                return;
#endif
            Graphics.DrawMesh(_planeMesh, _meshMatrix, _planeMaterial, _layerMask, Camera.main, 0, _materialProperties);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Validate()
        {
            if (_planeGeneratorShader == null)
                throw new NullReferenceException("Plane Generator Compute Shader is null");

            if (_planeMaterial == null)
                throw new NullReferenceException("Plaane material is null");
        }

        private void OnDestroy()
        {
            _planeMesh?.Clear();
            _planeMesh = null;

            _materialProperties?.Clear();
            _materialProperties = null;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if(!_isDrawGizmo)
                return;

            Gizmos.DrawWireCube(transform.position, new Vector3(_planeSideSize, 0.01f, _planeSideSize));
        }
#endif
    }
}
