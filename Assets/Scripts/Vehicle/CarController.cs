using UnityEngine;

namespace F1Racer
{
    [RequireComponent(typeof(Rigidbody))]
    public class CarController : MonoBehaviour
    {
        [Header("Vehicle Setup")]
        public Rigidbody rb;
        public Transform centerOfMass;
        public WheelCollider frontLeftWheel;
        public WheelCollider frontRightWheel;
        public WheelCollider rearLeftWheel;
        public WheelCollider rearRightWheel;

        public Transform frontLeftMesh;
        public Transform frontRightMesh;
        public Transform rearLeftMesh;
        public Transform rearRightMesh;

        public bool isPlayerControlled = true;
        public bool drsOpen;

        [Header("Drive")]
        public float maxMotorTorque = 1200f;
        public float brakeTorque = 2600f;
        public float maxSteerAngle = 30f;
        public float maxSpeedKmh = 360f;
        public float downforce = 1800f;
        public float dragCoefficient = 0.18f;
        public float tractionGrip = 1.5f;
        public float handbrakeGrip = 0.35f;
        public float[] gearRatios = { 3.6f, 2.8f, 2.2f, 1.8f, 1.45f, 1.2f, 1.0f };
        public float reverseRatio = 3.4f;
        public float maxEngineRPM = 15000f;
        public float idleRPM = 1100f;
        public float steeringAssist = 12f;

        [Header("Input")]
        [SerializeField] private float throttleInput;
        [SerializeField] private float brakeInput;
        [SerializeField] private float steeringInput;
        [SerializeField] private float handbrakeInput;

        [Header("Runtime")]
        public int gear = 1;
        public float engineRPM;
        public float speedKmh;

        private float dragModifier = 1f;

        private void Reset()
        {
            rb = GetComponent<Rigidbody>();
            rb.mass = 1200f;
            rb.drag = 0.1f;
            rb.angularDrag = 0.05f;
        }

        private void Awake()
        {
            if (rb == null)
            {
                rb = GetComponent<Rigidbody>();
            }

            if (centerOfMass != null)
            {
                rb.centerOfMass = centerOfMass.localPosition;
            }
        }

        private void Update()
        {
            if (isPlayerControlled)
            {
                ReadPlayerInput();
            }

            UpdateWheelVisuals();
        }

        private void FixedUpdate()
        {
            speedKmh = Vector3.Dot(rb.velocity, transform.forward) * 3.6f;
            ApplySteering();
            UpdateGearAndRPM();
            ApplyDrive();
            ApplyAero();
            ApplyTraction();
        }

        public void ReadPlayerInput()
        {
            throttleInput = Input.GetAxis("Vertical");
            brakeInput = Mathf.Max(0f, -Input.GetAxis("Vertical"));
            steeringInput = Input.GetAxis("Horizontal");
            handbrakeInput = Input.GetKey(KeyCode.Space) ? 1f : 0f;
        }

        public void SetInputs(float throttle, float brake, float steer, float handbrake)
        {
            throttleInput = Mathf.Clamp(throttle, -1f, 1f);
            brakeInput = Mathf.Clamp(brake, 0f, 1f);
            steeringInput = Mathf.Clamp(steer, -1f, 1f);
            handbrakeInput = Mathf.Clamp(handbrake, 0f, 1f);
        }

        public void SetDRSState(bool open)
        {
            drsOpen = open;
            dragModifier = open ? 0.55f : 1f;
        }

        private void ApplySteering()
        {
            float steerLimit = maxSteerAngle * Mathf.Clamp01(1f - Mathf.Abs(speedKmh) / 220f);
            float steerTarget = steeringInput * steerLimit;

            frontLeftWheel.steerAngle = Mathf.Lerp(frontLeftWheel.steerAngle, steerTarget, Time.fixedDeltaTime * steeringAssist);
            frontRightWheel.steerAngle = Mathf.Lerp(frontRightWheel.steerAngle, steerTarget, Time.fixedDeltaTime * steeringAssist);
        }

        private void UpdateGearAndRPM()
        {
            float absSpeed = Mathf.Abs(speedKmh);
            int gearCandidate = 1;

            if (absSpeed < 40f)
            {
                gearCandidate = 1;
            }
            else if (absSpeed < 90f)
            {
                gearCandidate = 2;
            }
            else if (absSpeed < 140f)
            {
                gearCandidate = 3;
            }
            else if (absSpeed < 190f)
            {
                gearCandidate = 4;
            }
            else if (absSpeed < 240f)
            {
                gearCandidate = 5;
            }
            else
            {
                gearCandidate = 6;
            }

            gear = Mathf.Clamp(gearCandidate, 1, gearRatios.Length);

            float targetRPM = Mathf.Lerp(idleRPM, maxEngineRPM, Mathf.Clamp01(Mathf.Abs(throttleInput)));
            engineRPM = Mathf.Lerp(engineRPM, targetRPM, Time.fixedDeltaTime * 6f);

            if (Mathf.Abs(speedKmh) < 1f && throttleInput <= 0f)
            {
                engineRPM = Mathf.Lerp(engineRPM, idleRPM, Time.fixedDeltaTime * 4f);
            }
        }

        private void ApplyDrive()
        {
            float effectiveTorque = GetEffectiveMotorTorque();
            float rearBrakeTorque = brakeInput * brakeTorque;

            rearLeftWheel.motorTorque = Mathf.Clamp(throttleInput * effectiveTorque, -300f, effectiveTorque);
            rearRightWheel.motorTorque = Mathf.Clamp(throttleInput * effectiveTorque, -300f, effectiveTorque);

            rearLeftWheel.brakeTorque = rearBrakeTorque;
            rearRightWheel.brakeTorque = rearBrakeTorque;

            frontLeftWheel.brakeTorque = brakeInput * brakeTorque * 0.45f;
            frontRightWheel.brakeTorque = brakeInput * brakeTorque * 0.45f;

            if (throttleInput <= 0.05f)
            {
                rearLeftWheel.motorTorque = 0f;
                rearRightWheel.motorTorque = 0f;
            }
        }

        private float GetEffectiveMotorTorque()
        {
            float forwardRatio = 1f;
            if (gear <= gearRatios.Length)
            {
                forwardRatio = gearRatios[gear - 1];
            }

            float torqueScale = Mathf.InverseLerp(idleRPM, maxEngineRPM, engineRPM);
            return maxMotorTorque * forwardRatio * Mathf.Clamp01(torqueScale);
        }

        private void ApplyAero()
        {
            Vector3 downforceVector = -transform.up * downforce * Mathf.Clamp01(1f + Mathf.Abs(speedKmh) / 120f);
            rb.AddForce(downforceVector, ForceMode.Acceleration);

            float speedFactor = Mathf.Clamp01(Mathf.Abs(speedKmh) / maxSpeedKmh);
            rb.drag = dragCoefficient * speedFactor * dragModifier + 0.02f;
        }

        private void ApplyTraction()
        {
            WheelFrictionCurve sidewaysRearLeft = rearLeftWheel.sidewaysFriction;
            WheelFrictionCurve sidewaysRearRight = rearRightWheel.sidewaysFriction;

            float tractionFactor = Mathf.Lerp(tractionGrip, handbrakeGrip, handbrakeInput);
            sidewaysRearLeft.stiffness = tractionFactor;
            sidewaysRearRight.stiffness = tractionFactor;

            rearLeftWheel.sidewaysFriction = sidewaysRearLeft;
            rearRightWheel.sidewaysFriction = sidewaysRearRight;
        }

        private void UpdateWheelVisuals()
        {
            UpdateWheelMesh(frontLeftWheel, frontLeftMesh);
            UpdateWheelMesh(frontRightWheel, frontRightMesh);
            UpdateWheelMesh(rearLeftWheel, rearLeftMesh);
            UpdateWheelMesh(rearRightWheel, rearRightMesh);
        }

        private void UpdateWheelMesh(WheelCollider wheelCollider, Transform wheelMesh)
        {
            if (wheelCollider == null || wheelMesh == null)
            {
                return;
            }

            wheelCollider.GetWorldPose(out var position, out var rotation);
            wheelMesh.position = position;
            wheelMesh.rotation = rotation;
        }
    }
}
