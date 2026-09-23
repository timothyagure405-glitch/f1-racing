using UnityEngine;

namespace F1Racer
{
    [RequireComponent(typeof(CarController))]
    public class DRSSystem : MonoBehaviour
    {
        public float activationSpeedKmh = 200f;
        public float drsOpenDragMultiplier = 0.55f;
        public KeyCode activateKey = KeyCode.LeftControl;

        private CarController car;
        private bool drsEnabled;

        private void Awake()
        {
            car = GetComponent<CarController>();
        }

        private void Update()
        {
            if (car == null)
            {
                return;
            }

            bool canActivate = Mathf.Abs(car.speedKmh) >= activationSpeedKmh;
            if (Input.GetKeyDown(activateKey) && canActivate)
            {
                drsEnabled = !drsEnabled;
                car.SetDRSState(drsEnabled);
            }

            if (!canActivate)
            {
                drsEnabled = false;
                car.SetDRSState(false);
            }
        }
    }
}
