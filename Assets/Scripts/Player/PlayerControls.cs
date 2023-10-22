using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerControls : MonoBehaviour
{
    public Rigidbody planeBody { get; private set; }
    private PlayerInput playerInput;
    public PlayerInputActions playerInputActions { get; private set; }
    [SerializeField] Transform minimapIcon;

    
    [SerializeField] float throttleIncrement = 1f;
    [SerializeField] float maxThrust = 200f;
    [SerializeField] float initialSpeed = 100;
    [SerializeField] float maximumSpeed = 230;
    [SerializeField] float initialThrottle = 0;
    [SerializeField] float gLimit;
    [SerializeField] float gLimitPitch;
    
    [SerializeField] AnimationCurve dragForward;
    [SerializeField] AnimationCurve dragBack;
    [SerializeField] AnimationCurve dragLeft;
    [SerializeField] AnimationCurve dragRight;
    [SerializeField] AnimationCurve dragTop;
    [SerializeField] AnimationCurve dragBottom;
    [SerializeField] Vector3 angularDrag;

    [SerializeField] float liftPower = 150f;
    [SerializeField] AnimationCurve aoaCurve;
    [SerializeField] float inducedDrag = 1;
    [SerializeField] AnimationCurve inducedDragCurve;
    [SerializeField] float rudderPower = 1f;
    [SerializeField] AnimationCurve rudderCurve;
    [SerializeField] AnimationCurve rudderInducedDragCurve;

    [SerializeField] float rotationSpeed = 20f;
    [SerializeField] Vector3 turnAcceleration;
    [SerializeField] Vector3 turnSpeed;
    [SerializeField] AnimationCurve steeringCurve;


    //How speed and acceleration work on airplanes, it is set to be constantly applying a force that can vary
    public float throttle { get; private set; } = 0f;

    public float pitch { get; private set; } = 0f;//Rotates on the X axis, like looking up or down
    public float yaw { get; private set; } = 0f;//Spins on the Y axis, like spinning an office chair
    public float roll { get; private set; } = 0f;//Rolls on the Z axis, like doing cartwheels

    Vector3 velocity;
    Vector3 lastVelocity;
    public Vector3 localGForce { get; private set; }
    Vector3 localVelocity;
    Vector3 localAngularVelocity;
    float angleOfAttack;
    float angleOfAttackYaw;

    AudioSource sound;

    private void Awake()
    {
        planeBody = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        playerInputActions = new PlayerInputActions();

        playerInputActions.UI.Enable();
        sound = GetComponent<AudioSource>();
    }

    private void Start()
    {
        throttle = initialThrottle;
        planeBody.velocity = new Vector3(0, 0, initialSpeed/3.6f);
    }

    private void Update()
    {
        HandleInput();
    }

    private void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;
        CalculateState();
        CalculateAngleOfAttack();
        CalculateGForce(dt);

        UpdateThrust();
        UpdateLift();
        UpdateSteering(dt);
        UpdateDrag();
        UpdateAngularDrag();
        UpdateMinimapView();
        sound.pitch = throttle/100f;
    }

    void UpdateMinimapView()
    {
        minimapIcon.rotation = Quaternion.Euler(90, transform.rotation.y, -transform.rotation.z);
    }
    void UpdateThrust()
    {
        if (planeBody.velocity.magnitude < maximumSpeed / 3.6f)
        {
            planeBody.AddRelativeForce(throttle * maxThrust * Vector3.forward);
        }
        else planeBody.velocity = planeBody.velocity.normalized * (maximumSpeed / 3.6f);
    }
    //The formula for real world drag is: Drag = 1/2 * (airDensity) * (velocity)^2 * (surfaceArea) * (dragCoefficient)
    //Considering a game intending to simulate flight while being not too computationally heavy, the formula used was:
    //Drag = 1/2 * (velocity)^2 * (dragCoefficient)
    void UpdateSteering(float dt)
    {
        Vector3 controlInput = new Vector3(pitch, yaw, roll);
        float speed = Mathf.Max(0, localVelocity.z);
        float steeringPower = steeringCurve.Evaluate(speed);

        float gForceScaling = CalculateGLimiter(controlInput, turnSpeed*Mathf.Deg2Rad * steeringPower);
        
        Vector3 targetAV = Vector3.Scale(controlInput, turnSpeed * steeringPower * gForceScaling);
        Vector3 av = localAngularVelocity * Mathf.Rad2Deg;

        Vector3 correction = new Vector3(
            CalculateSteering(dt, av.x, targetAV.x, turnAcceleration.x * steeringPower),
            CalculateSteering(dt, av.y, targetAV.y, turnAcceleration.y * steeringPower),
            CalculateSteering(dt, av.z, targetAV.z, turnAcceleration.z * steeringPower));
        
        planeBody.AddRelativeTorque(correction * Mathf.Deg2Rad, ForceMode.VelocityChange);
        if(pitch==0 && yaw==0 && roll==0 && planeBody.angularVelocity.magnitude != 0)
        {
            planeBody.angularVelocity = Vector3.Lerp(planeBody.angularVelocity, Vector3.zero, .5f);
        }
    }
    //calculates rotation for a single axis
    float CalculateSteering(float dt, float angularVelocity, float targetVelocity, float acceleration)
    {
        float error = targetVelocity - angularVelocity;
        float accel = acceleration * dt;
        return Mathf.Clamp(error, -accel, accel);
    }
    void UpdateDrag()
    {
        Vector3 lv = localVelocity;
        float lv2 = lv.sqrMagnitude;//velocity squared
        

        Vector3 coefficient = Scale6(
            lv.normalized, 
            dragRight.Evaluate(Mathf.Abs(lv.x)), dragLeft.Evaluate(Mathf.Abs(lv.x)), 
            dragTop.Evaluate(Mathf.Abs(lv.y)), dragBottom.Evaluate(Mathf.Abs(lv.y)), 
            dragForward.Evaluate(Mathf.Abs(lv.z)), dragBack.Evaluate(Mathf.Abs(lv.z)));

        Vector3 drag = coefficient.magnitude * lv2 * -lv.normalized;//drag is always opposite velocity

        planeBody.AddRelativeForce(drag);
    }
    void UpdateAngularDrag()
    {
        Vector3 av = localAngularVelocity;
        Vector3 drag = av.sqrMagnitude * -av.normalized;//squared, opposite direction of angular velocity
        planeBody.AddRelativeTorque(Vector3.Scale(drag, angularDrag), ForceMode.Acceleration);//ignore rigidbody mass
    }
    void UpdateLift()
    {
        if (localVelocity.sqrMagnitude < 1f) return;

        Vector3 liftForce = CalculateLift(
            angleOfAttack + (Mathf.Deg2Rad), Vector3.right,
            liftPower,
            aoaCurve);

        Vector3 yawForce = CalculateLift(angleOfAttackYaw, Vector3.up, rudderPower, rudderCurve);

        planeBody.AddRelativeForce(liftForce);
        planeBody.AddRelativeForce(yawForce);
    }
    //The formula for real world lift is: Lift = 1/2 * (airDensity) * (surfaceArea) * (liftCoefficient) * (velocity)^2
    //Like drag, it was simplified to: Lift = 1/2 * (liftCoefficient) * (velocity)^2 * (liftPower)
    //when Lift > Mass * Gravity, the plane should start taking off
    Vector3 CalculateLift(float angleOfAttack, Vector3 rightAxis, float liftPower, AnimationCurve aoaCurve)
    {
        Vector3 liftVelocity = Vector3.ProjectOnPlane(localVelocity, rightAxis);
        float v2 = liftVelocity.sqrMagnitude;

        //coefficient varies with AoA
        float liftCoefficient = aoaCurve.Evaluate(angleOfAttack * Mathf.Rad2Deg);
        float liftForce = v2 * liftCoefficient * liftPower;

        //lift is perpendicular to velocity
        Vector3 liftDirection = Vector3.Cross(liftVelocity.normalized, rightAxis);
        Vector3 lift = liftDirection * liftForce;

        //induced drag varies with lift coefficient squared
        float dragForce = liftCoefficient * liftCoefficient * inducedDrag;
        Vector3 dragDirection = -liftVelocity.normalized;
        Vector3 finalDrag = dragDirection * v2 * dragForce;

        return lift + finalDrag;
    }


    void CalculateState()
    {
        Quaternion invRotation = Quaternion.Inverse(planeBody.rotation);
        velocity = planeBody.velocity;
        localVelocity = invRotation * velocity;//world velocity into local space
        localAngularVelocity = invRotation * planeBody.angularVelocity;//into local space
    }
    void CalculateAngleOfAttack()
    {
        if (localVelocity.sqrMagnitude < 0.1f)
        {
            angleOfAttack = 0;
            angleOfAttackYaw = 0;
            return;
        }
        angleOfAttack = Mathf.Atan2(-localVelocity.y, localVelocity.z);
        angleOfAttackYaw = Mathf.Atan2(localVelocity.x, localVelocity.z);
    }
    void CalculateGForce(float dt)
    {
        Quaternion invRotation = Quaternion.Inverse(planeBody.rotation);
        Vector3 acceleration = (velocity - lastVelocity) / (dt * Physics.gravity.magnitude);
        localGForce = invRotation * acceleration;
        lastVelocity = velocity;
    }
    //Tangential Velocity = (Angular Velocity) * (Radius)
    //G-Force = (Velocity^2) / (Radius) = (Velocity) * (Velocity) / (Radius) = (Velocity) * (Tangential Velocity) * (Radius) / (Radius)
    //G-Force = (Velocity) * (Tangential Velocity)
    Vector3 CalculateGForce(Vector3 angularVelocity, Vector3 velocity)
    {
        return Vector3.Cross(angularVelocity, velocity);
    }
    Vector3 CalculateGForceLimit(Vector3 input)
    {
        return Scale6(input,
            gLimit, gLimitPitch,
            gLimit, gLimit,
            gLimit, gLimit)*Physics.gravity.magnitude;
    }
    float CalculateGLimiter(Vector3 controlInput, Vector3 maxAngularVelocity)
    {
        Vector3 maxInput = controlInput.normalized;
        Vector3 limit = CalculateGForceLimit(maxInput);
        Vector3 maxGForce = CalculateGForce(Vector3.Scale(maxInput, maxAngularVelocity), localVelocity);

        if(maxGForce.magnitude > limit.magnitude)
        {
            return limit.magnitude / maxGForce.magnitude;
        }
        return 1;
    }
    private void HandleInput()
    {
        float accelerate = playerInputActions.Player.Accelerate.ReadValue<float>();
        bool stopBraking = (accelerate >= 0 && throttle < 0);
        pitch = playerInputActions.Player.Pitch.ReadValue<float>();
        yaw = playerInputActions.Player.Yaw.ReadValue<float>();
        roll = playerInputActions.Player.Roll.ReadValue<float>();


        if (accelerate > .1f)
        {
            throttle += throttleIncrement;
        }
        else if (accelerate < -.1f)
        {
            throttle -= throttleIncrement;
        }

        if(stopBraking)
        {
            throttle = 0;
        }

        throttle = Mathf.Clamp(throttle, -50f, 100f);
    }
    public void SwitchToUIMap()
    {
        playerInput.SwitchCurrentActionMap("UI");
        playerInputActions.UI.Enable();
        playerInputActions.Player.Disable();
    }
    public void SwitchToPlayerMap()
    {
        playerInput.SwitchCurrentActionMap("Player");
        playerInputActions.UI.Disable();
        playerInputActions.Player.Enable();
    }
    public static Vector3 Scale6(
        Vector3 value,
        float posX, float negX,
        float posY, float negY,
        float posZ, float negZ)
    {
        Vector3 result = value;

        if (result.x > 0)
        {
            result.x *= posX;
        }
        else if (result.x < 0)
        {
            result.x *= negX;
        }

        if (result.y > 0)
        {
            result.y *= posY;
        }
        else if (result.y < 0)
        {
            result.y *= negY;
        }

        if (result.z > 0)
        {
            result.z *= posZ;
        }
        else if (result.z < 0)
        {
            result.z *= negZ;
        }

        return result;
    }
}
