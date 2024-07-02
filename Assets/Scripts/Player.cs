using ProjectEnums;
using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour {

	public static Player instance;

	[Header("Stats")]
	public float playerMaxHealth = 100;
	public float playerCurrentHealth = 100;
	public float playerMaxMana = 100;
	public float playerCurrentMana = 100;
	public float moveSpeed = 3f;
	public float jumpForce = 6f;

	[Header("Other")]
	public float moveInputX;
	public float moveInputY;
	public float maxFallSpeed = -15f;
	public bool canMove;

	private Rigidbody2D rb;
	public bool isFacingRight;

	[Header("Ground Check values")]
	public LayerMask groundLayer;
	public Vector2 groundCheckSize;
	public float groundCheckCastDistance;
	public Transform groundCheck;


	[Header("Combat")]
	//combat
	public float damageCooldownTimer = 0f;
	private float damageCooldownTime = 1f;
	private float attackRate = 0.35f;
	public float attackCooldownTimer = 0f;
	public Transform attackPoint;
	public Transform attackPointDown;
	public Transform attackPointUp;
	private float attackRange = 0.9f;
	private float attackDamage = 10f;
	public LayerMask enemyLayer;
	public bool isAttacking;

	//camera movement
	private CameraFollowObject cameraFollowObject;
	private float fallSpeedYDampingChangeTreshold;

	[System.NonSerialized] public bool interactionEnabled = true;

	Animator animator;

	//player character colliders
	BoxCollider2D boxCollider;
	//CapsuleCollider2D capsuleCollider;
	Vector2 colliderSize;

	[SerializeField]
	private PhysicsMaterial2D noFriction;
	[SerializeField]
	private PhysicsMaterial2D fullFriction;

	private void Awake() {
		if (instance == null) {
			instance = this;
		}
		else {
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
		playerCurrentHealth = playerMaxHealth;
		playerCurrentMana = playerMaxMana;
		isFacingRight = true;
		rb = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		//capsuleCollider = GetComponent<CapsuleCollider2D>();
		boxCollider = GetComponent<BoxCollider2D>();
		//colliderSize = capsuleCollider.size;
		colliderSize = boxCollider.size;

		//if (CameraFollowObject.instance != null) {
		//	cameraFollowObject = CameraFollowObject.instance.GetComponent<CameraFollowObject>();
		//}

		//fallSpeedYDampingChangeTreshold = CameraManager.instance.fallSpeedYDampingChangeTreshold;
	}

	void Update() {
		if (WindowManager.instance.escapeableWindowStack.Count == 0) {
			if (CameraFollowObject.instance != null && CameraManager.instance != null) {
				cameraFollowObject = CameraFollowObject.instance.GetComponent<CameraFollowObject>();
				fallSpeedYDampingChangeTreshold = CameraManager.instance.fallSpeedYDampingChangeTreshold;
			}
			if (interactionEnabled == true) {
				attackCooldownTimer -= Time.deltaTime;
				damageCooldownTimer -= Time.deltaTime;
				canMove = true;
				Move();
				//Jump();
				//Attack();
				if (moveInputX > 0f || moveInputX < 0f) {
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
		}
		//else {
		//	canMove = false;
		//	rb.velocity = new Vector2(0f, rb.velocity.y); //purkkafix
		//}
	}


	private void FixedUpdate() {
		if (rb.velocity.y < maxFallSpeed) {
			rb.velocity = new Vector2(rb.velocity.x, maxFallSpeed);
		}
		animator.SetFloat("xVelocity", Math.Abs(rb.velocity.x));
		animator.SetFloat("yVelocity", rb.velocity.y);


		if (Grounded() == false) {
			animator.SetBool("isJumping", true);
		}
		else {
			animator.SetBool("isJumping", false);
		}

		SlopeCheck();
	}

	void Move() {
		if (canMove == true) {
			// Get the horizontal input (A/D keys or Left/Right arrow keys)
			moveInputX = Input.GetAxis("Horizontal");
			moveInputY = Input.GetAxis("Vertical");

			if (isAttacking == true && Grounded() == true) {
				rb.velocity = Vector2.zero;
			}
			else {
				// Set the player's velocity based on input
				rb.velocity = new Vector2(moveInputX * moveSpeed, rb.velocity.y);
			}
		}
	}

	public void Jump() {
		if (Grounded() == true && canMove == true) {
			rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
			//rb.velocity = new Vector2(rb.velocity.x, jumpForce);
		}
	}

	public void Attack() {
		if (attackCooldownTimer <= 0f) {
			isAttacking = true;
			animator.SetBool("isAttacking", true);
			if (moveInputY <= -0.1f && Grounded() == false) {
				StartCoroutine(DealDamageDown());
				animator.Play("LongswordDown");
			}
			else if (moveInputY >= 0.1f) {
				StartCoroutine(DealDamageUp());
				animator.Play("LongswordJump"); //placeholder
			}
			else {
				StartCoroutine(DealDamageForward());
				if (Grounded() == false) {
					animator.Play("LongswordJump");
				}
				else {
					animator.Play("Longsword");
				}
			}
			attackCooldownTimer = attackRate;
		}
	}

	private IEnumerator DealDamageDown() {
		Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPointDown.position, attackRange, enemyLayer);

		foreach (Collider2D enemy in hitEnemies) {
			enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
			if (enemy.gameObject.layer == LayerMask.NameToLayer("Enemy")) {
				rb.velocity = new Vector2(rb.velocity.x, jumpForce);
			}
		}

		yield return new WaitForSeconds(0.24f);
		isAttacking = false;
		animator.SetBool("isAttacking", false);
	}


	private IEnumerator DealDamageForward() {
		Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

		yield return new WaitForSeconds(0.1f);
		foreach (Collider2D enemy in hitEnemies) {
			Debug.Log(enemy.name);
			enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
		}
		yield return new WaitForSeconds(0.24f);
		isAttacking = false;
		animator.SetBool("isAttacking", false);
	}

	private IEnumerator DealDamageUp() {
		Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPointUp.position, attackRange, enemyLayer);

		yield return new WaitForSeconds(0.1f);
		foreach (Collider2D enemy in hitEnemies) {
			Debug.Log(enemy.name);
			enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
		}
		yield return new WaitForSeconds(0.24f);
		isAttacking = false;
		animator.SetBool("isAttacking", false);
	}

	public void TakeDamage(float damage) {
		playerCurrentHealth -= damage;
		damageCooldownTimer = damageCooldownTime;
		Debug.Log("damage taken = " + damage);
	}

	void FlipCheck() {
		if (canMove == true) {
			if (moveInputX > 0f && isFacingRight == false) {
				Flip();
			}
			else if (moveInputX < 0f && isFacingRight == true) {
				Flip();
			}
		}
	}

	public bool Grounded() {
		if (Physics2D.BoxCast(groundCheck.transform.position, groundCheckSize, 0, -transform.up, groundCheckCastDistance, groundLayer)) {
			return true;
		}
		else {
			return false;
		}
	}

	void Flip() {
		if (isAttacking == false) {
			if (isFacingRight == true) {
				Vector3 rotator = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
				transform.rotation = Quaternion.Euler(rotator);
				isFacingRight = !isFacingRight;
				cameraFollowObject.Turn();
			}
			else {
				Vector3 rotator = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
				transform.rotation = Quaternion.Euler(rotator);
				isFacingRight = !isFacingRight;
				cameraFollowObject.Turn();
			}
		}
	}

	private void SlopeCheck() {
		//Vector2 checkPosition = transform.position - new Vector3(0f, colliderSize.y / 2);
		if (moveInputX == 0f && Grounded() == true) {
			boxCollider.sharedMaterial = fullFriction;
			//capsuleCollider.sharedMaterial = fullFriction;
		}
		else {
			boxCollider.sharedMaterial = noFriction;
			//capsuleCollider.sharedMaterial = noFriction;
		}
	}

	private void OnDrawGizmos() {
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(groundCheck.transform.position - transform.up * groundCheckCastDistance, groundCheckSize);
		Gizmos.DrawWireSphere(attackPoint.position, attackRange);
		Gizmos.DrawWireSphere(attackPointDown.position, attackRange);
		Gizmos.DrawWireSphere(attackPointUp.position, attackRange);
	}
}
