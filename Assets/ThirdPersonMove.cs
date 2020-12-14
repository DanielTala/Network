using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Cinemachine;

public class ThirdPersonMove : NetworkBehaviour
{
    public CharacterController controller;

    public CinemachineFreeLook cmf;
    public GameObject indicator;
    public float speed = 6f;
    private float run;
    private float walk;
    public Transform cam;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    public float jumpHeight = 3f;
    public float gravity = -9.18f;

    Vector3 velocity;
    bool isGrounded;
    Animator anim;
    private string currentState;
    private bool test;

    const string PlayerJump = "Player_Jump";
    const string PlayerIdle = "Player_Idle";
    const string PlayerWalk = "Player_Walking";
    const string PlayerRun = "Player_Run";
    private void Start()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        anim = GetComponent<Animator>();
        run = speed * 2;
        walk = speed;

    }

    void ChangeAnimationState(string newState)
    {
        if (!isLocalPlayer)
        {
            return;
        }
        if (currentState == newState)
        {
            return;
        }
        anim.Play(newState);

        currentState = newState;
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer)
        {
            cmf.enabled = false;
            indicator.SetActive(false);
            return;

        }

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if(isGrounded && velocity.y<0)
        { 
            velocity.y = -2f; 
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;


        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);


        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        if(!isGrounded)
        {
            ChangeAnimationState(PlayerJump);
        }
        


        if (direction.magnitude >= 0.1f)
        {
            if (Input.GetKey(KeyCode.LeftShift) && isGrounded)
            {
                speed = run;
                ChangeAnimationState(PlayerRun);
            }
            else if (isGrounded)
            {
                speed = walk;
                ChangeAnimationState(PlayerWalk);
            }

            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }
        else if (isGrounded)
        {
            ChangeAnimationState(PlayerIdle);
        }


    }


   
}
