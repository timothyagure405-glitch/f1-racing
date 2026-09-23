using UnityEngine;

namespace F1Racer
{
    [RequireComponent(typeof(CarController))]
    public class AICarController : MonoBehaviour
    {
        public WaypointPath waypointPath;
        public float targetSpeedKmh = 210f;
        public float brakeDistance = 15f;
        public float accelerationBias = 1.2f;
        public float steeringSensitivity = 1.6f;

        private CarController car;

        private void Awake()
        {
            car = GetComponent<CarController>();
            if (car != null)
            {
                car.isPlayerControlled = false;
            }
        }

        private void FixedUpdate()
        {
            if (car == null || waypointPath == null)
            {
                return;
            }

            Transform waypoint = waypointPath.GetClosestWaypoint(transform.position);
            if (waypoint == null)
            {
                return;
            }

            Vector3 localWaypoint = transform.InverseTransformPoint(waypoint.position);
            float steer = Mathf.Clamp(localWaypoint.x * steeringSensitivity, -1f, 1f);

            float speedError = targetSpeedKmh - Mathf.Abs(car.speedKmh);
            float throttle = Mathf.Clamp01(speedError / 120f) * accelerationBias;
            float brake = 0f;

            if (Mathf.Abs(localWaypoint.z) < brakeDistance)
            {
                brake = Mathf.Clamp01((brakeDistance - Mathf.Abs(localWaypoint.z)) / brakeDistance);
                throttle = 0f;
            }

            float handbrake = Mathf.Abs(localWaypoint.x) > 4f ? 0.15f : 0f;
            car.SetInputs(throttle, brake, steer, handbrake);
        }
    }
}
