using System.Runtime.CompilerServices;
using UnityEngine;

namespace PromvrTestTask
{
    public class HeightMapController : MonoBehaviour
    {
        private const string DefaultTextureName = "HeightMap";

        [SerializeField]
        private ComputeShader _fillingMapShader;

        [SerializeField, Min(128)]
        private int _textureSize = 512;

        [SerializeField, Space, Header("Material Settings")]
        private float _accumulationMaterialSpeed = 0.1f;

        [SerializeField]
        private float _additiveSpeedPerSecondFactor = 1f;

        private float _currentAccumulationSpeed;

        private RenderTexture _texture;

        private InputService _inputService;

        private HemiSphereController _sphere;

        private HeightMapUpdater _heightMapUpdater;

        private bool _isFilling = false;

        public RenderTexture HeightMap => _texture;

        public void Initialize()
        {
            _fillingMapShader = Instantiate(_fillingMapShader);

            _heightMapUpdater = new HeightMapUpdater(_fillingMapShader);

            CreateTexture();
        }

        public void SetData(InputService inputService, HemiSphereController sphere, float planeSideSize)
        {
            _inputService = inputService;
            _sphere = sphere;

            _heightMapUpdater.Initialize(_texture, planeSideSize);
            _heightMapUpdater.Reset();

            _inputService.ClickActionButton += OnAction;
            _inputService.ClickResetButton += OnReset;
        }

        private void Update()
        {
            if (!_isFilling)
                return;

            var deltaTime = Time.deltaTime;

            _heightMapUpdater.Update(_sphere.Position, _sphere.Radius, _currentAccumulationSpeed, deltaTime);

            _currentAccumulationSpeed += _additiveSpeedPerSecondFactor * _accumulationMaterialSpeed * deltaTime;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CreateTexture()
        {
            _texture = new RenderTexture(_textureSize, _textureSize, 0, RenderTextureFormat.RFloat);
            _texture.enableRandomWrite = true;
            _texture.filterMode = FilterMode.Point;
            _texture.Create();
            _texture.name = DefaultTextureName;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnAction()
        {
            _isFilling = !_isFilling;

            if (_isFilling)
            {
                _currentAccumulationSpeed = _accumulationMaterialSpeed;

                _heightMapUpdater.Restart(_sphere.Position, _sphere.Radius);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnReset() => _heightMapUpdater.Reset();

        private void OnDestroy()
        {
            _texture?.Release();
            _texture = null;

            _inputService.ClickActionButton -= OnAction;
            _inputService.ClickResetButton -= OnReset;
        }
    }
}
