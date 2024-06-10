using ProjectEnums;
using System;
using UnityEngine;

public class Player : MonoBehaviour {

    public static Player instance;


    public float playerMaxHealth = 100;
    public float playerCurrentHealth = 100;
    public float playerMaxMana = 100;
    public float playerCurrentMana = 100;
    public float moveSpeed = 3f;
    public float jumpForce = 6f;
    public float moveInput;
    public float maxFallSpeed = -15f;

    private Rigidbody2D rb;
    public bool isFacingRight;

    [SerializeField]
    private PhysicsMaterial2D noFriction;
    [SerializeField]
    private PhysicsMaterial2D fullFriction;

    public LayerMask groundLayer;
    public Vector2 groundCheckSize;
    public float groundCheckCastDistance;

    public bool isGrounded;
    public bool canMove;

    private CameraFollowObject cameraFollowObject;
    private float fallSpeedYDampingChangeTreshold;

    [System.NonSerialized] public bool interactionEnabled = true;

    Animator animator;

    CapsuleCollider2D capsuleCollider;
    //BoxCollider2D boxCollider;
    Vector2 colliderSize;

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }
    private void OnDestroy() {
        if (instance == this) {
            instance = null;
        }
    }

    void Start() {
        WindowManager.instance.ShowWindow(WindowPanel.GameUI);
        playerCurrentHealth = 100;
        playerCurrentMana = 100;
        isFacingRight = true;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        //boxCollider = GetComponent<BoxCollider2D>();
        colliderSize = capsuleCollider.size;
        //colliderSize = boxCollider.size;

        if (CameraFollowObject.instance != null) {
            cameraFollowObject = CameraFollowObject.instance.GetComponent<CameraFollowObject>();
        }

        fallSpeedYDampingChangeTreshold = CameraManager.instance.fallSpeedYDampingChangeTreshold;
    }

    void Update() {
        if (WindowManager.instance.escapeableWindowStack.Count == 0) {
            if (interactionEnabled == true) {
                canMove = true;
                Move();
                Jump();
                if (moveInput > 0f || moveInput < 0f) {
                    FlipCheck();
                }

                //if player is falling past a set speed treshold
                if (rb.velocity.y < fallSpeedYDampingChangeTreshold && CameraManager.instance.isLerpingYDamping == false && CameraManager.instance.lerpedFromPlayerFalling == false) {
                    CameraManager.instance.LerpYDamping(true);
                }

                //if player is standing still or moving up
                if (rb.velocity.y >= 0f && CameraManager.instance.isLerpingYDamping == false && CameraManager.instance.lerpedFromPlayerFalling == true) {
                    //reset so it can be called again
                    CameraManager.instance.lerpedFromPlayerFalling = false;

                    CameraManager.instance.LerpYDamping(false);
                }
            }
        } else
        {
            canMove = false;
        }
    }


    private void FixedUpdate() {
        if (rb.velocity.y < maxFallSpeed) {
            rb.velocity = new Vector2(rb.velocity.x, maxFallSpeed);
        }
        animator.SetFloat("xVelocity", Math.Abs(rb.velocity.x));
        animator.SetFloat("yVelocity", rb.velocity.y);

        if (Grounded() == false) {
            animator.SetBool("isJumping", true);
        } else {
            animator.SetBool("isJumping", false);
        }
        SlopeCheck();
    }

    void Move() {
        if (canMove == true) {
            // Get the horizontal input (A/D keys or Left/Right arrow keys)
            moveInput = Input.GetAxis("Horizontal");

            // Set the player's velocity based on input
            rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
            //animator.SetFloat("xVelocity", Math.Abs(rb.velocity.x));
            //animator.SetFloat("yVelocity", rb.velocity.y);
        } else
        {
            //rb.velocity.x = 0f;
        }
    }

    void Jump() {
        if (Input.GetButtonDown("Jump") && Grounded() == true && canMove == true) {
            //rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        //if (Grounded() == true) {
        //    animator.SetBool("isJumping", false);
        //}

    }

    void FlipCheck() {
        if (moveInput > 0f && isFacingRight == false) {
            Flip();
        } else if (moveInput < 0f && isFacingRight == true) {
            Flip();
        }
    }

    public bool Grounded() {
        if (Physics2D.BoxCast(transform.position, groundCheckSize, 0, -transform.up, groundCheckCastDistance, groundLayer)) {
            return true;
        } else {
            return false;
        }
    }

    void Flip() {
        if (isFacingRight == true) {
            Vector3 rotator = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
            isFacingRight = !isFacingRight;
            cameraFollowObject.Turn();
        } else {
            Vector3 rotator = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
            isFacingRight = !isFacingRight;
            cameraFollowObject.Turn();
        }
    }

    private void SlopeCheck() {
        //Vector2 checkPosition = transform.position - new Vector3(0f, colliderSize.y / 2);
        if (moveInput == 0f && Grounded() == true) {
            capsuleCollider.sharedMaterial = fullFriction;
        } else {
            capsuleCollider.sharedMaterial = noFriction;
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position - transform.up * groundCheckCastDistance, groundCheckSize);
    }
}
