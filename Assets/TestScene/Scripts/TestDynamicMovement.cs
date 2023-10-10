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


    //INPUT READ VALUES
    public float run,
        dash;


    [Header("Movement Variables")]
    public float baseSpeed = 5f;
    public float walkSpeedMultiplierValue = 0.4f;
    public float crouchWalkSpeedMultiplierValue = .4f;
    public float tiredWalkSpeedMultiplierValue = .4f;

    public float speedChangeModifier = 0.4f;
    public float stepModifier = .05f;
    public float speedChanger;
    private float valueX = 0,
        valueY = 0;

    public float currentSpeedMultiplier = 1f;


    private bool isDashing,
        isRunning,
        isMoving,
        canDash;


    //ForStamina 

    private bool isTired;

    //For Crouch
    private bool isCrouching;

    [Header("Dash")]

    public float dashSpeedMultiplierValue = 3f;
    [Range(0f,5f)] public float dashCooldownTime = 2f;

    

    [Header("Timers")]

    private float currentDashTime = Mathf.Infinity;


    [Header("Slopes")]
    public AnimationCurve slopeSpeedAngles;
    public float slopeSpeedModifier;
    public float slopeOffset;
    public float lerp;
    public float valueForDowningCharacter = 0;
    [Header("For Rotating Player")]

    private Vector3 currentTargetRotation;
    private Vector3 timeToReachTargetRotation;
    private Vector3 dampedTargetRotationCurrentVelocity;
    private Vector3 dampedTargetRotationPassedTime;


    //[Range(0.0f, 0.3f)]
    //public float RotationSmoothTime = 0.12f;
    //private float _rotationVelocity;
    [field: Header("Collisions")]
    [field: SerializeField] public FlyingCapsuleCollider ColliderUtility { get; private set; }
    private SlopeData slopeData;


    [Header("Layer Data")]
    [SerializeField] LayerMask groundLayer;
    public Vector2 movementHolder,
        movementInputs, mouseMove;


    //-------COMPONENTS---------

    PlayerInputTesting inputs;
    Rigidbody rb;
    Animator anim;
    Collider characterCollider;
    Transform cam;
    Stamina st;
    #region Monobehaviour Methods
    private void Awake()
    {
        inputs = new PlayerInputTesting();
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        characterCollider = GetComponent<Collider>();
        st = GetComponent<Stamina>();
        cam = Camera.main.transform;
        ColliderUtility.Initilaze(gameObject);
        ColliderUtility.CalculateCapsuleValues();
        slopeData = ColliderUtility.SlopeData;
    }
    private void Start()
    {
        speedChanger = baseSpeed;
        timeToReachTargetRotation.y = 0.14f;
        InputEventMethod();

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
        HandleInputs();
        CheckDash();
        CheckRun();
        UpdateTimers();
        CheckStamina();
        CheckStaminaValues();
        SetAnimVariables();
    }

    private void SetAnimVariables()
    {
        anim.SetBool("isMoving", isMoving);
        anim.SetBool("isCrouching", isCrouching);
        anim.SetBool("isTired", isTired);
        anim.SetFloat("speed", GetCurrentSpeed());
    }

    private void FixedUpdate()
    {
        FloatCapsule();
        Move();

    }



    #endregion
    #region Inputs

    private void InputEventMethod()
    {

        inputs.CharacterMovement.Move.started += OnMovementStart;
        inputs.CharacterMovement.Move.canceled += OnMovementCancel;

        inputs.CharacterMovement.DashButton.started += OnDashStart;
        inputs.CharacterMovement.DashButton.canceled += OnDashCancel;

        inputs.CharacterMovement.CrouchButton.started += OnCrouchStart;

    }

  

    public void DisableActionFor(InputAction action, float amountOfSeconds)
    {
        StartCoroutine(DisableAction(action, amountOfSeconds));
    }
    IEnumerator DisableAction(InputAction action, float seconds)
    {
        action.Disable();

        yield return new WaitForSecondsRealtime(seconds);

        action.Enable();
    }
    private void HandleInputs()
    {
        movementHolder = inputs.CharacterMovement.Move.ReadValue<Vector2>();

        run = inputs.CharacterMovement.RunButton.ReadValue<float>();
        isRunning = run == 1;
        dash = inputs.CharacterMovement.DashButton.ReadValue<float>();
        movementInputs.x = GetAxsisMovement(movementHolder.x, ref valueX);
        movementInputs.y = GetAxsisMovement(movementHolder.y, ref valueY);

    }

    private void CheckDash()
    {
        canDash = currentDashTime > dashCooldownTime;
    }

    private void CheckRun()
    {
        if (isDashing && isTired && !isRunning) { return; }
        isCrouching = false;
        st.DecreaseStamina(st.GetDAmount());
        currentSpeedMultiplier = isRunning ? 1 : walkSpeedMultiplierValue;
    }
    private void UpdateTimers()
    {
        currentDashTime += Time.deltaTime;
    }
    private void OnMovementStart(InputAction.CallbackContext context)
    {
        if (isDashing && isTired) return;

        currentSpeedMultiplier = isCrouching ? crouchWalkSpeedMultiplierValue : walkSpeedMultiplierValue;
    }

    private void OnMovementCancel(InputAction.CallbackContext context)
    {
        if (isDashing) return;
        print("Cagirildi");
        ResetVelocity();
        valueX = 0;
        valueY = 0;
        currentSpeedMultiplier = 0;
    }
    private void OnDashStart(InputAction.CallbackContext context)
    {
        if (!canDash) return;
        currentDashTime = 0;
        isDashing = true;
        currentSpeedMultiplier = dashSpeedMultiplierValue;
        anim.SetTrigger("Dash");
        DashForce();
    }

    private void DashForce()
    {
        rb.velocity = (GetSpeedValue() * transform.forward);
    }

    private void OnDashCancel(InputAction.CallbackContext context)
    {
    }
    private void OnCrouchStart(InputAction.CallbackContext context)
    {
        isCrouching = !isCrouching;
    }
    private float GetAxsisMovement(float a, ref float value)
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
    #region Stamina Methods
    private void CheckStamina()
    {
        if (st.GetStamina() <= 0.1f)
        {
            isTired = true;
        }

        if (st.IsStaminaFull())
        {
            isTired = false;
        }

    }

    private void CheckStaminaValues()
    {

        isMoving = GetCurrentSpeed() >= 0.2f;

        if (isTired)
        {
           currentSpeedMultiplier = tiredWalkSpeedMultiplierValue;
           st.TiredStaminaIncrease(st.GetTiredIncAmount());

        }
        else if (!isRunning)
        {
            st.IncreaseStamina(st.GetIncAmount());
        }
        else
        {
            st.IncreaseStamina(st.GetIncAmount());
        }
    }
    #endregion

    private void Move()
    {
        if (movementInputs == Vector2.zero || currentSpeedMultiplier == 0 || isDashing)
        {
            return;
        }
        //var dir= SetRotation(moveDir);

        var moveDir = GetMovementDirection();

        float angle = Rotate(moveDir);

        Vector3 targetRot = Quaternion.Euler(0, angle, 0) * Vector3.forward;
        //var dir = Rotate(moveDir); 
        var curVelocity = GetPlayerGroundVelocity();

        // FOR TRANSITION BETWEEN WALK AND RUN SPEED
        speedChanger = isDashing ? GetSpeedValue() : speedChanger > baseSpeed ? GetSpeedValue() : Mathf.MoveTowards(speedChanger, GetSpeedValue(), speedChangeModifier);
        //speedChanger = GetSpeedValue();
        rb.AddForce(speedChanger * targetRot - curVelocity, ForceMode.VelocityChange);
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
            
            if (slopeSpeedModif == 0)
            {
                return;
            }

            var angleFor = Vector3.Angle(transform.forward, -hit.normal);
            if (slopeSpeedModif < 1 && angleFor > 90f)
            {
                print("Asagi");
                //Asagi iniyor rampadan
                valueForDowningCharacter = -slopeOffset;
            }
            else if (slopeSpeedModif < 1 && angleFor < 89f)
            {
                print("Yukari");
                valueForDowningCharacter = slopeOffset;
            }
            else
            {
                valueForDowningCharacter = 0;
            }

            float distanceToFloatPoint = ColliderUtility.ColliderData.ColliderCenterInLocalSpace.y - 
                hit.distance + valueForDowningCharacter;
            if (distanceToFloatPoint == 0)
            {
            print(" Distance 0 oldu");
                return;

            }
            
            
            
            float amountToLift;
            amountToLift = distanceToFloatPoint * slopeData.StepReachForce - GetPlayerVerticalVelocity().y ;
            
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
        Vector3 vel = rb.velocity;
        vel.y = 0;
        return vel;
    }
    private Vector3 GetPlayerVerticalVelocity()
    {
        return new Vector3(0, rb.velocity.y, 0);
    }
    private Vector3 GetMovementDirection()
    {
        return new Vector3(movementInputs.x, 0, movementInputs.y);
    }
    private float GetSpeedValue()
    {
        return baseSpeed * currentSpeedMultiplier * slopeSpeedModifier;
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

    #region Animation Methods

    public void OnAnimationEnterEvent()
    {

    }

    public void OnAnimationTransitionEvent()
    {

    }
    public void OnAnimationExitEvent()
    {
        isDashing = false;
        if (movementInputs == Vector2.zero)
        {
            ResetVelocity();
            currentSpeedMultiplier = 0;
            return;
        }
        else if (run == 1)
        {
            ResetVelocity();
            currentSpeedMultiplier = 1;
        }
        else
        {
            ResetVelocity(); 
            currentSpeedMultiplier = walkSpeedMultiplierValue;
        }

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