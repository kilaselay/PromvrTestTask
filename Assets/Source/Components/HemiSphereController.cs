using System.Runtime.CompilerServices;
using UnityEngine;

namespace PromvrTestTask
{
    public class HemiSphereController : MonoBehaviour
    {
        private const float MinRadius = 0.1f;

        [SerializeField]
        private float _velocity = 5f;

        [SerializeField]
        private float _baseRadius = 1.5f;

        [SerializeField]
        private float _radiusAmplitude = 1.2f;

        [SerializeField]
        private float _radiusFrequency = 1f;

        [SerializeField]
        private AnimationCurve _radiusCurve = AnimationCurve.Linear(0, 1, 1, 1);

        private float _currentRadius = 1.5f;

        private float _elapsedTime;

        private InputService _inputService;

        private Vector2 _borders;

#if UNITY_EDITOR
        [SerializeField, Space, Header("Editor Settings")]
        private bool _isDrawGizmo = true;

        private bool _isActive = false;
#endif

        public Vector3 Position => transform.position;

        public float Radius => _currentRadius;

        public float HeightWeight => (_baseRadius + _radiusAmplitude) / _baseRadius;

        public void Initialize()
        {
            _currentRadius = _baseRadius;
            UpdateSphereSize();
        }

        public void SetData(InputService inputService, Vector2 borders)
        {
            _inputService = inputService;

            _borders = borders;
#if UNITY_EDITOR
            _inputService.ClickActionButton += OnClickActionButton;
#endif
        }

        private void Update()
        {
            var deltaTime = Time.deltaTime;

            _elapsedTime += deltaTime;

            UpdateSpherePosition(deltaTime);
            UpdateSphereRadius();
            UpdateSphereSize();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdateSpherePosition(float deltaTime)
        {
            var position = transform.position;

            var movement = _inputService.Movement;

            position.x = Mathf.Clamp(position.x + movement.x * _velocity * deltaTime, -_borders.x, _borders.x);
            position.z = Mathf.Clamp(position.z + movement.y * _velocity * deltaTime, -_borders.y, _borders.y);

            transform.position = new Vector3(position.x, 0, position.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdateSphereRadius()
        {
            float cycleProgress = (_elapsedTime * _radiusFrequency) % 1f;

            float curveValue = _radiusCurve.Evaluate(cycleProgress);

            _currentRadius = Mathf.Max(MinRadius, _baseRadius + _radiusAmplitude * curveValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdateSphereSize() => transform.localScale = Vector3.one * _currentRadius * 2f;

#if UNITY_EDITOR
        private void OnClickActionButton() => _isActive = !_isActive;

        private void OnDrawGizmos()
        {
            if (!_isDrawGizmo)
                return;

            if (_isActive)
                Gizmos.color = Color.cyan;
            else
                Gizmos.color = Color.red;

            Gizmos.DrawWireSphere(transform.position, _currentRadius + 0.01f);
        }
#endif

        private void OnDestroy()
        {
#if UNITY_EDITOR
            _inputService.ClickActionButton -= OnClickActionButton;
#endif
        }
    }
}
