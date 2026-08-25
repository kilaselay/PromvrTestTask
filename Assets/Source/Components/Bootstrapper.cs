using UnityEngine;

namespace PromvrTestTask
{
    public class Bootstrapper : MonoBehaviour
    {
        [SerializeField]
        private InputService _inputService;

        [SerializeField]
        private PlaneController _planeController;

        [SerializeField]
        private HemiSphereController _hemiSphereController;

        [SerializeField]
        private HeightMapController _heightMapController;

        private void Start()
        {
            _planeController.Initialize();

            var planeSideSize = _planeController.PlaneSideSize;

            _hemiSphereController.Initialize();
            _hemiSphereController.SetData(_inputService, new Vector2(planeSideSize * 0.5f, planeSideSize * 0.5f));

            _heightMapController.Initialize();
            _heightMapController.SetData(_inputService, _hemiSphereController, planeSideSize);

            _planeController.SetData(_heightMapController.HeightMap, _hemiSphereController.HeightWeight);
        }
    }
}
