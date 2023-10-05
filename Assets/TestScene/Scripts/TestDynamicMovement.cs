using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class TestDynamicMovement : MonoBehaviour
{
    public Transform referencedPoint;


    
    public float run;
    

    [Header("Movement Variables")]
    public float baseSpeed = 5f;
    public float speedMultiplier = 1f;
    public float stepModifier = .05f;
    public float speedChangeModifier = 0.4f;
    public float walkSpeedMultiplierValue = 0.4f;
    public float speedChanger;
    private float valueX = 0,
        valueY = 0;
    private bool isRunning;

    [Header("Slopes")]
    public AnimationCurve slopeSpeedAngles;
    public float slopeSpeedModifier;


    [Header("For Rotating Player")]

    private Vector3 currentTargetRotation;
    private Vector3 timeToReachTargetRotation;
    private Vector3 dampedTargetRotationCurrentVelocity;
    private Vector3 dampedTargetRotationPassedTime   ;


    //[Range(0.0f, 0.3f)]
    //public float RotationSmoothTime = 0.12f;
    //private float _rotationVelocity;
    [field:Header("Collisions")]
    [field: SerializeField] public FlyingCapsuleCollider ColliderUtility { get; private set; }
    private SlopeData slopeData;


    [Header("Layer Data")]
    [SerializeField] LayerMask groundLayer;
    public Vector2 movementHolder,
        movement, mouseMove;


    //-------COMPONENTS---------

    PlayerInputTesting inputs;
    Rigidbody rb;
    Animator anim;
    Collider characterCollider;
    Transform cam;
    private void Awake()
    {
        inputs=new PlayerInputTesting();
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        characterCollider = GetComponent<Collider>();
        cam = Camera.main.transform;
        ColliderUtility.Initilaze(gameObject);
        ColliderUtility.CalculateCapsuleValues();
        slopeData = ColliderUtility.SlopeData;
    }
    private void Start()
    {
        speedChanger = baseSpeed;
        timeToReachTargetRotation.y = 0.14f;
        inputs.CharacterMovement.Move.started += OnMovementStart;
        inputs.CharacterMovement.Move.canceled += OnMovementCancel;

    }

   
    private void OnValidate()
    {
        ColliderUtility.Initilaze(gameObject);
        ColliderUtility.CalculateCapsuleValues();
    }
    private void OnEnable()
    {
        inputs.Enable();
    }
    private void OnDisable()
    {
        inputs.Disable();
    }
    private void Update()
    {
        anim.SetFloat("speed",GetCurrentSpeed());
        HandleInputs();
    }

    private void FixedUpdate()
    {
        FloatCapsule();
        Move();

    }

    #region Inputs
    private void HandleInputs()
    {
        movementHolder = inputs.CharacterMovement.Move.ReadValue<Vector2>();

        run = inputs.CharacterMovement.RunButton.ReadValue<float>();
        isRunning = run == 1;

        movement.x = GetAxsisMovement(movementHolder.x, ref valueX);
        movement.y = GetAxsisMovement(movementHolder.y, ref valueY);
        
    }

    private void OnMovementStart(InputAction.CallbackContext context)
    {

        speedMultiplier = walkSpeedMultiplierValue;
    }

    private void OnMovementCancel(InputAction.CallbackContext context)
    {
        print("Cagirildi");
        ResetVelocity();
        valueX = 0;
        valueY = 0;
        speedMultiplier = 0;

    }
  

    private float GetAxsisMovement(float a,ref float value)
    {
        value = Mathf.MoveTowards(value, a, stepModifier);
        return value;

        //if (movementHolder.x < 0)
        //{
        //    value = Mathf.MoveTowards(value, a, tModifier);
        //}
        //else if (movementHolder.x > 0)
        //{
        //    value = Mathf.MoveTowards(value, a, tModifier);
        //}
        //else if (movementHolder.x == 0)
        //{
        //    if (value < 0.1f)
        //    {
        //        value = 0;
        //        return;
        //    }
        //    value = Mathf.MoveTowards(value, a, tModifier);
        //}
    }
    #endregion

    private void Move()
    {
        if (movement == Vector2.zero || speedMultiplier == 0) 
        {
            return;
        }
        //var dir= SetRotation(moveDir);

        var moveDir = GetMovementDirection();

        float angle = Rotate(moveDir);

        Vector3 targetRot = Quaternion.Euler(0, angle, 0) * Vector3.forward;
        //var dir = Rotate(moveDir); 
        var curVelocity = GetPlayerGroundVelocity();

        speedChanger = Mathf.MoveTowards(speedChanger, GetSpeedValue(), speedChangeModifier);

        rb.AddForce(speedChanger * targetRot-curVelocity, ForceMode.VelocityChange);
    }

    #region Rotation
    private float Rotate(Vector3 direction)
    {
        float directionAngle = UpdateTargetRotation(direction);

        RotateTowardsTargetRotation();

        return directionAngle;
    }

    private float UpdateTargetRotation(Vector3 direction, bool shouldConsiderCameraRotation = true)
    {
        float directionAngle = GetDirectionAngle(direction);

        if (shouldConsiderCameraRotation)
        {
            directionAngle = AddCameraRotationToAngle(directionAngle);
        }

        if (directionAngle != currentTargetRotation.y)
        {
            UpdateTargetRotationData(directionAngle);
        }

        return directionAngle;
    }

    private void UpdateTargetRotationData(float targetAngle)
    {
        currentTargetRotation.y = targetAngle;
        dampedTargetRotationPassedTime.y = 0f;
    }

    private float AddCameraRotationToAngle(float angle)
    {
        angle += cam.transform.eulerAngles.y;

        if (angle > 360f)
        {
            angle -= 360f;
        }

        return angle;
    }

    void RotateTowardsTargetRotation()
    {
        float currentYAngle = transform.rotation.eulerAngles.y;

        if (currentYAngle == currentTargetRotation.y)
        {
            return;
        }


        float smoothedYAngle = Mathf.SmoothDampAngle(currentYAngle, currentTargetRotation.y, ref dampedTargetRotationCurrentVelocity.y,
            timeToReachTargetRotation.y - dampedTargetRotationPassedTime.y);

        dampedTargetRotationPassedTime.y += Time.deltaTime;

        Quaternion targetRotation = Quaternion.Euler(0, smoothedYAngle, 0);

        rb.MoveRotation(targetRotation);
    }
    private static float GetDirectionAngle(Vector3 direction)
    {
        float directionAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

        if (directionAngle < 0)
        {
            directionAngle += 360f;
        }

        return directionAngle;
    }

    #endregion

    #region Floating Capsule

    private void FloatCapsule()
    {
        Vector3 center = ColliderUtility.ColliderData.Collider.bounds.center;
        Ray capsuleCenterToDownRay = new(center, Vector3.down);
        Debug.DrawRay(center, Vector3.down * slopeData.RayDistance, Color.blue);
        if (Physics.Raycast(capsuleCenterToDownRay, out RaycastHit hit, slopeData.RayDistance, groundLayer, QueryTriggerInteraction.Ignore))
        {
            float angle = Vector3.Angle(hit.normal, -capsuleCenterToDownRay.direction);

            float slopeSpeedModif = SetSlopeSpeedModifierOnAngle(angle);

            if (slopeSpeedModif==0)
            {
                return;
            }

            float distanceToFloatPoint = ColliderUtility.ColliderData.ColliderCenterInLocalSpace.y - hit.distance;

            if (distanceToFloatPoint == 0)
            {
                return;

            }
            float amountToLift = distanceToFloatPoint * slopeData.StepReachForce - GetPlayerVerticalVelocity().y;

            Vector3 liftForce = new(0, amountToLift, 0);

            rb.AddForce(liftForce, ForceMode.VelocityChange);
        }
    }

    private float SetSlopeSpeedModifierOnAngle(float angle)
    {
        float slopeSpeed = slopeSpeedAngles.Evaluate(angle);
        slopeSpeedModifier = slopeSpeed;
        return slopeSpeedModifier;
    }
    #endregion

    #region Getter Metods
    private Vector3 GetPlayerGroundVelocity()
    {
        Vector3 vel=rb.velocity;
        vel.y = 0;
        return vel;
    }
    private Vector3 GetPlayerVerticalVelocity()
    {
        return new Vector3(0, rb.velocity.y, 0);
    }
    private Vector3 GetMovementDirection()
    {
        return new Vector3(movement.x, 0, movement.y);
    }
    private float GetSpeedValue()
    {
        speedMultiplier = isRunning ? 1 : walkSpeedMultiplierValue;
        return baseSpeed * speedMultiplier * slopeSpeedModifier;
    }
    private float GetCurrentSpeed()
    {
        var speed = rb.velocity;
        speed.y = 0;

        return speed.magnitude;
    }
    private void ResetVelocity()
    {
        rb.velocity = Vector3.zero;
    }
    #endregion
}
//private Vector3 SetRotation(Vector3 moveDir)
//{
//    var targetRotation = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg +
//                                  cam.transform.eulerAngles.y;
//    float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref _rotationVelocity,
//        RotationSmoothTime);

//    transform.rotation= Quaternion.Euler(0.0f, rotation, 0.0f);
//    // rotate to face input direction relative to camera position
//    Vector3 targetDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;
//    return targetDirection.normalized;

//}