using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public float maxSpeed;
    public float forwardSpeed;
    public float sidewaysSpeed;
    public float backwardsSpeed;

    public bool canSprint = true;
    public float sprintVelocity;

    public bool canJump = true;
    public float jumpDelay; // time in seconds after touching the ground until the player can jump again
    public float jumpHeight;

    public float floatiness = 5f; // velocity (in m/s) lost per second

    private Rigidbody rbody;
    private float originalFOV;
    private bool sprinting;
    private bool mayJump = true;
    private bool jumpTimerTicking = false;
    private float jumpForce;
    public bool IsMoving { get; private set; }

	// Use this for initialization
	void Start ()
    {
        rbody = GetComponent<Rigidbody>();
        originalFOV = Camera.main.fieldOfView;

        jumpForce = Mathf.Sqrt(2 * -Physics.gravity.y * jumpHeight);

        // vf2 - vi2 = 2ah
        // -vi2 = 2ah - vf2
        // vi2 = vf2 - 2ah
        // vi2 = 0 - 2ah
        // vi2 = -2ah
        // vi = sqrt(2ah)
	}

    void Update()
    {
        if (!canSprint) return;

        sprinting = Input.GetKey(KeyCode.LeftShift) && IsGrounded() && (Input.GetAxisRaw("Vertical") > 0);
        if (sprinting)
        {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, originalFOV * 1.1f, 0.2f);
        }
        else
        {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, originalFOV, 0.1f);
        }
    }

	// Update is called once per frame
	void FixedUpdate ()
    {
        if (IsGrounded())
        {
            Jumping();
        }
        Movement();
        LimitSpeed();
	}

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -transform.up, 1.05f);
    }

    void LimitSpeed()
    {
        var vel = rbody.velocity;
        vel.y = 0;
        if (vel.magnitude > maxSpeed)
        {
            vel = vel.normalized * maxSpeed;
        }
        vel.y = rbody.velocity.y;
        rbody.velocity = vel;
    }

    void Jumping()
    {
        if (canJump && IsGrounded())
        {
            if (mayJump && Input.GetKey(KeyCode.Space))
            {
                var vel = rbody.velocity;
                vel.y = jumpForce;
                rbody.velocity = vel;

                mayJump = false;
            }
            else if(!mayJump && !jumpTimerTicking)
            {
                jumpTimerTicking = true;
                StartCoroutine(JumpTimer());
            }
        }
    }

    IEnumerator JumpTimer()
    {
        yield return new WaitForSeconds(jumpDelay);
        mayJump = true;
        jumpTimerTicking = false;
    }
    
    void Movement()
    {
        var fwd = Input.GetAxisRaw("Vertical");

        fwd = fwd < 0 ? fwd * backwardsSpeed : (sprinting ? fwd * sprintVelocity : fwd * forwardSpeed);

        var side = Input.GetAxisRaw("Horizontal") * sidewaysSpeed;

        var vel = rbody.velocity;

        var localVel_0 = transform.InverseTransformDirection(vel);

        if(fwd != 0)
        {
            localVel_0.z = 0;
        }

        if(side != 0)
        {
            localVel_0.x = 0;
        }

        IsMoving = fwd != 0 || side != 0;

        vel = transform.TransformDirection(localVel_0);

        var camFwd = transform.forward;
        camFwd.y = 0;

        vel += camFwd * fwd;
        vel += Camera.main.transform.right * side;

        var localVel = transform.InverseTransformDirection(vel);

        if(fwd == 0)
        {
            localVel.z *= Mathf.Pow(floatiness, Time.deltaTime * 60f);
        }

        if(side == 0)
        {
            localVel.x *= Mathf.Pow(floatiness, Time.deltaTime * 60f);
        }

        rbody.velocity = transform.TransformDirection(localVel);
    }

    public bool IsSprinting() {
        return sprinting;
    }

    public Rigidbody GetRigidbody() {
        return rbody;
    }
}
