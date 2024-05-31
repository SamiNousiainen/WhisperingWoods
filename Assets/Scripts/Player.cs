using UnityEngine;

public class Player : MonoBehaviour {

    public static Player instance;

    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float moveInput;
    public float maxFallSpeed = -15f;

    private Rigidbody2D rb;
    private bool isGrounded;
    public bool isFacingRight;

    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.3f;

    private CameraFollowObject cameraFollowObject;
    private float fallSpeedYDampingChangeTreshold;

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
        isFacingRight = true;
        rb = GetComponent<Rigidbody2D>();

        if (CameraFollowObject.instance != null) {
            cameraFollowObject = CameraFollowObject.instance.GetComponent<CameraFollowObject>();
        }

        fallSpeedYDampingChangeTreshold = CameraManager.instance.fallSpeedYDampingChangeTreshold;
    }

    void Update() {
        Move();
        Jump();
        CheckGrounded();
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

    private void FixedUpdate() {
        if (rb.velocity.y < maxFallSpeed) {
            rb.velocity = new Vector2(rb.velocity.x, maxFallSpeed);
        }
    }

    void Move() {
        // Get the horizontal input (A/D keys or Left/Right arrow keys)
        moveInput = Input.GetAxis("Horizontal");

        // Set the player's velocity based on input
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
    }

    void Jump() {
        if (Input.GetButtonDown("Jump") && isGrounded == true) {
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        }
    }

    void FlipCheck() {
        if (moveInput > 0f && isFacingRight == false) {
            Flip();
        } else if (moveInput < 0f && isFacingRight == true) {
            Flip();
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

    void CheckGrounded() {
        // Check if the groundCheck position overlaps with any ground layer objects
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
