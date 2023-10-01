using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float defMoveSpeed=10f,
        crouchSpeed=5f;
    [SerializeField]
    private float runSpeed=15f;
    [SerializeField]
    private float tiredSpeed=7f;
    [SerializeField]
    private float rotationSpeed=800f;
    [SerializeField]
    private Transform cam;

    [Range(0.0f, 0.3f)]
    public float RotationSmoothTime = 0.12f;
    private float _rotationVelocity;

    public float inputSpeed = 5f;
    public bool isTired=false;
    public bool isMoving=false;
    public bool canRun=false;
    public bool canCrouch = false;
    public bool isRunning = false;
    public bool isCrouching = false;
    public bool shouldMove = false;

    private float horizontal, vertical;
    private float targetRotation = 0.0f;

    public float SpeedChangeRate = 10.0f;

    private float _speed;
    private Vector3 vel;
    private Stamina st;
    private Rigidbody rb;
    private TempInventory inventory;
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        st = GetComponent<Stamina>();
        inventory=GetComponent<TempInventory>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
        SetRotation();
        CheckMovement();

        CheckStamina();
        UpdateAnims();
    }

    private void UpdateAnims()
    {
        anim.SetBool("isMoving", isMoving);
        anim.SetBool("isTired", isTired);
        anim.SetFloat("speed", _speed);
        anim.SetBool("isCrouching", canCrouch);
    }

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

    private void CheckMovement()
    {

        isMoving = rb.velocity.magnitude >= 0.1f;


        if (isTired)
        {
            isRunning = false;
            SetMovement(tiredSpeed);
            st.TiredStaminaIncrease(st.GetTiredIncAmount());

        }
        else if (canCrouch)
        {
           // isCrouching = true;
            isRunning = false;
            SetMovement(crouchSpeed);

        }
        else if (canRun && isMoving)
        {
            isRunning = true;
            
            SetMovement(runSpeed);
            st.DecreaseStamina(st.GetDAmount());

        }
        else if(shouldMove)
        {
            isRunning = false;
            SetMovement(defMoveSpeed);
            st.IncreaseStamina(st.GetIncAmount());

            
        }
        else
        {
            isRunning = false;
            SetMovement(0);
            st.IncreaseStamina(st.GetIncAmount());
        }

        

    }


    private void CheckInput()
    {
        if (inventory.GetInventoryStatus())
        {
            return;
        }
        //horizontal = Mathf.Lerp(horizontal, Input.GetAxis("Horizontal"), Time.deltaTime);
        horizontal = Input.GetAxis("Horizontal"); 
        vertical = Input.GetAxis("Vertical");
        canRun = Input.GetKey(KeyCode.LeftShift);
        if (Input.GetKeyDown(KeyCode.C))
        {
            canCrouch=!canCrouch;
        }
        if (Mathf.Abs(horizontal) >= 0.3f || Mathf.Abs(vertical) >= 0.3f)
        {
            shouldMove = true;
        }
        else
        {
            shouldMove = false;
        }

    }

    private void SetRotation()
    {
        //float targetRotationAngle = cam.eulerAngles.y;
        //if (vertical>=0.1f)
        //{
        //    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, targetRotationAngle, 0), rotationSpeed * Time.deltaTime);
        //}
        //else if (vertical<=-0.1f)
        //{
        //    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, targetRotationAngle+180f, 0), rotationSpeed * Time.deltaTime);
        //}
        //else if (horizontal>=0.1f)
        //{
        //    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, targetRotationAngle+90f, 0), rotationSpeed * Time.deltaTime);
        //}
        //else if (horizontal<=-0.1f)
        //{
        //    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, targetRotationAngle-90f, 0), rotationSpeed * Time.deltaTime);
        //}


        //----------------------------------------

        //Vector3 cameraForward = cam.forward;
        //Vector3 cameraRight = cam.right;

        //// Make sure the direction vectors are horizontal (no vertical component)
        //cameraForward.y = 0;
        //cameraRight.y = 0;

        //// Calculate the target rotation based on input directions
        //Vector3 desiredDirection = cameraForward * vertical + cameraRight * horizontal;

        //// If there is a desired direction (movement), rotate towards it
        //if (desiredDirection != Vector3.zero)
        //{
        //    Quaternion targetRotation = Quaternion.LookRotation(desiredDirection);
        //    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        //}




        //----------------------------------------

        

        //var yRotation = Quaternion.Lerp(transform.rotation, cam.rotation, rotationSpeed * Time.deltaTime).eulerAngles.y;
        //else
        //    transform.rotation = Quaternion.Euler(transform.rotation.x, yRotation, transform.rotation.z);

    }
    private void SetMovement(float speed)
    {
        //if (vertical != 0 && horizontal != 0)
        //{
        //    vel = new Vector3(speed * horizontal * diagonalSpeedMultiplier, rb.velocity.y, speed * vertical * diagonalSpeedMultiplier);
        //}
        //else
        //    vel = new Vector3(horizontal, 0, vertical).normalized * speed;

        //Vector3.

        //---------------------------
        //Vector3 moveDirection = cam.forward.normalized * vertical+horizontal*cam.right.normalized;
        //moveDirection.Normalize();
        //vel = moveDirection * speed;
        //rb.velocity = new Vector3(vel.x,rb.velocity.y,vel.z);

        //---------------------------------------------

        if (!shouldMove)
        {
            speed = 0;
        }

        float currentHorizontalSpeed = new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude;
        float speedOffset = 0.1f;
        // accelerate or decelerate to target speed
        if (currentHorizontalSpeed < speed- speedOffset ||
            currentHorizontalSpeed > speed + speedOffset)
        {


            // creates curved result rather than a linear one giving a more organic speed change
            // note T in Lerp is clamped, so we don't need to clamp our speed
            _speed = Mathf.Lerp(currentHorizontalSpeed, speed*1f, Time.deltaTime * SpeedChangeRate);

            // round speed to 3 decimal places
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = speed;
        }


        if (shouldMove)
        {

            targetRotation = Mathf.Atan2(horizontal, vertical) * Mathf.Rad2Deg +
                                      cam.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref _rotationVelocity,
                RotationSmoothTime);

            // rotate to face input direction relative to camera position
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        Vector3 targetDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;

        vel = _speed * targetDirection.normalized;
        rb.velocity = new Vector3(vel.x, rb.velocity.y, vel.z);



        //TODO: Bu olmadan çalýþýyo buga girmeden yeni rotasyon kodu yaz
        //Vector3 moveDir = rb.velocity.normalized;
        //if (Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f)
        //{
        //    Quaternion targetRotation = Quaternion.LookRotation(moveDir, Vector3.up);

        //    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        //}
    }
}
