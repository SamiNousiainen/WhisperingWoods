using ProjectEnums;
using System;
using System.Collections;
using System.Collections.Generic;
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
	private float dashSpeed = 40f;
	private float dashDuration = 0.15f;
	private bool isDashing;
	private float dashCooldownTimer = 0f;
	private float dashCooldownTime = 0.5f;

	[Header("Other")]
	[SerializeField]
	private float moveInputX;
	[SerializeField]
	private float moveInputY;
	private float maxFallSpeed = -30f;
	public bool canMove;

	public float jumpBufferTime = 0.1f;
	public float jumpBufferTimer;

	public float coyoteTime = 0.1f;
	public float coyoteTimeTimer;
	private float jumpTimer;

	public Rigidbody2D rb;
	public bool isFacingRight;

	[SerializeField]
	private PhysicsMaterial2D noFriction;
	[SerializeField]
	private PhysicsMaterial2D fullFriction;

	[Header("Ground Check values")]
	public LayerMask groundLayer;
	public Vector2 groundCheckSize;
	public float groundCheckCastDistance;
	public Transform groundCheck;


	[Header("Combat")]
	private float attackBufferTime = 0.1f;
	public float attackBufferTimer;

	private bool isComboQueued = false;
	private bool canCombo = false;
	private int comboStep = 0;

	public float takingDamageTimer = 0f;
	private float takingDamageTime = 0.5f;
	private float immunityTimer = 0f;
	private float immunityTime = 1.5f;
	private float attackRate = 0.3f;
	public float attackCooldownTimer = 0f;
	public Transform attackPoint;
	public Transform attackPointDown;
	public Transform attackPointUp;
	public Collider2D attackCollFwd;
	public Collider2D attackCollUp;
	public Collider2D attackCollDown;
	//private float attackRadius = 2.2f;
	public float attackDamage = 10f;
	public LayerMask enemyLayer;
	private float knockbackForceX = 14f;
	private float knockbackForceY = 24f;
	public bool isAttacking { get; private set; } = false;

	private List<Enemy> damagedEnemies = new List<Enemy>();

	//camera movement
	private CameraFollowObject cameraFollowObject;
	private float fallSpeedYDampingChangeTreshold;

	[System.NonSerialized] public bool interactionEnabled = true;

	Animator animator;

	//player character colliders
	BoxCollider2D characterCollider;
	Vector2 colliderSize;


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
		characterCollider = GetComponent<BoxCollider2D>();
		colliderSize = characterCollider.size;

	}

	void Update() {
		if (WindowManager.instance.escapeableWindowStack.Count == 0) {
			if (CameraFollowObject.instance != null && CameraManager.instance != null) {
				cameraFollowObject = CameraFollowObject.instance.GetComponent<CameraFollowObject>();
				fallSpeedYDampingChangeTreshold = CameraManager.instance.fallSpeedYDampingChangeTreshold;
			}
			if (interactionEnabled == true) {
				if (takingDamageTimer > 0 || isDashing == true) {
					canMove = false;
				}
				else {
					canMove = true;
				}

				if (immunityTimer > 0 || isDashing == true) {
					Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);
				}
				else {
					Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);
				}

				attackCooldownTimer -= Time.deltaTime;
				takingDamageTimer -= Time.deltaTime;
				immunityTimer -= Time.deltaTime;
				jumpBufferTimer -= Time.deltaTime;
				jumpTimer -= Time.deltaTime;
				dashCooldownTimer -= Time.deltaTime;
				attackBufferTimer -= Time.deltaTime;

				if (Grounded() == true) {
					coyoteTimeTimer = coyoteTime;
				}
				else {
					coyoteTimeTimer -= Time.deltaTime;
				}

				if (jumpTimer <= 0f && Input.GetButton("Jump") == false && takingDamageTimer <= 0f) {
					DecreaseYVelocity();
				}

				if (moveInputX > 0.1f || moveInputX < -0.1f) {
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
		if (WindowManager.instance.escapeableWindowStack.Count > 0 && Grounded() == true) {
			rb.velocity = new Vector2(0, 0);
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
		}
		else {
			animator.SetBool("isJumping", false);
		}
		Move();
		//SlopeCheck();
		if (isDashing == true || moveInputX != 0f) {
			characterCollider.sharedMaterial = noFriction;
		}
		else {
			characterCollider.sharedMaterial = fullFriction;
		}
	}

	public void StopPlayer(bool stopPlayer) {
		if (stopPlayer == true) {
			canMove = false;
			rb.velocity = new Vector2(0, 0);
		}
		else {
			canMove = true;
			rb.velocity = new Vector2(moveInputX * moveSpeed, rb.velocity.y);
		}
	}

	void Move() {
		if (canMove == true) {
			moveInputX = Input.GetAxisRaw("Horizontal");
			moveInputY = Input.GetAxis("Vertical");

			if (isAttacking == true && Grounded() == true) {
				rb.velocity = Vector2.zero;
			}
			else {
				if (moveInputX >= 0.1f) {
					rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
				}
				if (moveInputX <= -0.1f) {
					rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
				}
				if (moveInputX == 0f) {
					rb.velocity = new Vector2(0f, rb.velocity.y);
				}
			}
		}
	}

	public void Dash() {
		if (dashCooldownTimer <= 0) {
			StartCoroutine(StartDash());
		}
	}

	private IEnumerator StartDash() {
		isDashing = true;
		dashCooldownTimer = dashCooldownTime;
		if (Grounded() == false) {
			rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
		}
		else {
			rb.constraints = RigidbodyConstraints2D.FreezeRotation;
		}
		if (isFacingRight == true) {
			rb.velocity = new Vector2(dashSpeed, 0f);
		}
		else {
			rb.velocity = new Vector2(-dashSpeed, 0f);
		}
		yield return new WaitForSeconds(dashDuration);
		rb.constraints = RigidbodyConstraints2D.FreezeRotation;
		rb.velocity = Vector2.zero;
		isDashing = false;
	}

	public void Jump() {
		if (canMove == true && coyoteTimeTimer > 0f && isAttacking == false) {
			rb.velocity = new Vector2(rb.velocity.x, jumpForce);
			jumpBufferTimer = 0f;
			coyoteTimeTimer = 0f;
			jumpTimer = 0.1f;
		}
	}

	public void DecreaseYVelocity() {
		//used for variable jump height and pogo jump
		if (rb.velocity.y > 0f && Grounded() == false) {
			rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.4f);
		}
	}

	#region combat

	public void Attack() {
		if (attackCooldownTimer <= 0f) {
			//dealing damage is started and stopped with animation triggers
			animator.SetBool("isAttacking", true);
			if (moveInputY <= -0.5f && Grounded() == false) {
				animator.Play("LongswordDown");
			}
			else if (moveInputY >= 0.5f && Grounded() == false) {
				animator.Play("LongswordUp"); //mid air upward slash
			}
			else if (moveInputY >= 0.5f && Grounded() == true) {
				animator.Play("LongswordUp2"); //upward slash while grounded 
			}
			else {
				if (Grounded() == false) {
					animator.Play("LongswordJump");
				}
				else {
					// Combo logic
					if (comboStep == 0 || canCombo == false) {
						animator.Play("Longsword");
						comboStep = 1;
						isComboQueued = false;
					}
					if (canCombo == true && comboStep == 1) {
						isComboQueued = true;
					}
				}
			}
			attackCooldownTimer = attackRate;
		}
	}

	public void EnableCombo() {
		canCombo = true;
	}

	//trigger the combo if queued at the end of the animation
	public void PerformCombo() {
		if (isComboQueued == true) {
			animator.Play("SwordCombo");
			comboStep = 1;
		}
		else {
			comboStep = 0; // Reset if no combo is queued
		}
		isComboQueued = false;
		canCombo = false;
	}

	public void ResetCombo() {
		comboStep = 0;
		isComboQueued = false;
		canCombo = false;
	}

	public IEnumerator DealDamageDown() {

		isAttacking = true;
		jumpTimer = 0.24f;
		attackCollDown.enabled = true;

		//old method

		//Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPointDown.position, attackRadius, enemyLayer);

		//foreach (Collider2D hitEnemy in hitEnemies) {
		//	if (hitEnemy != null) {
		//		rb.velocity = new Vector2(rb.velocity.x, jumpForce * 1.1f);
		//	}
		//}

		//while (isAttacking == true) {
		//	foreach (Collider2D hitEnemy in hitEnemies) {
		//		if (hitEnemy != null) {
		//			Enemy enemy = hitEnemy.GetComponent<Enemy>();
		//			if (enemy.hasTakenDamage == false) {
		//				StartCoroutine(FreezeFrame());
		//				enemy.TakeDamage(attackDamage);
		//				damagedEnemies.Add(enemy);
		//			}
		//		}
		//	}
		yield return null;
		//}
	}

	public IEnumerator DealDamageForward() {

		isAttacking = true;
		attackCollFwd.enabled = true;
		yield return null;
	}

	public IEnumerator DealDamageUp() {

		isAttacking = true;
		attackCollUp.enabled = true;
		yield return null;

	}

	//stop attacking with animation trigger
	public void StopAttacking() {
		isAttacking = false;
		attackCollFwd.enabled = false;
		attackCollDown.enabled = false;
		attackCollUp.enabled = false;
		animator.SetBool("isAttacking", false);
		DecreaseYVelocity();
		ReturnEnemyToDamageable();
		attackBufferTimer = attackBufferTime;
	}

	public void StopFollowUpAtk() {
		isAttacking = false;
		attackCollFwd.enabled = false;
		animator.SetBool("isAttacking", false);
		DecreaseYVelocity();
		ReturnEnemyToDamageable();
		attackBufferTimer = 0;
	}

	private void ReturnEnemyToDamageable() {
		foreach (Enemy damagedEnemy in damagedEnemies) {
			damagedEnemy.hasTakenDamage = false;
		}
		damagedEnemies.Clear();
	}

	public void TakeDamage(float damage, Transform damageSource) {
		playerCurrentHealth -= damage;
		takingDamageTimer = takingDamageTime;
		attackCooldownTimer = takingDamageTime;
		immunityTimer = immunityTime;
		isAttacking = false;
		Debug.Log("damage taken = " + damage);

		Vector2 knockbackDirection = (transform.position - damageSource.position).normalized;
		knockbackDirection.y = 1;
		rb.velocity = new Vector2(knockbackDirection.x * knockbackForceX, knockbackDirection.y * knockbackForceY);

		StartCoroutine(FreezeFrame());
	}

	public IEnumerator FreezeFrame() {
		Time.timeScale = 0;
		yield return new WaitForSecondsRealtime(0.05f);
		Time.timeScale = 1;
	}

	#endregion

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

	public bool Grounded() {
		if (Physics2D.BoxCast(groundCheck.transform.position, groundCheckSize, 0, -transform.up, groundCheckCastDistance, groundLayer)) {
			return true;
		}
		else {
			return false;
		}
	}

	private void SlopeCheck() {
		//WIP, might not need this feature
		//Vector2 checkPosition = transform.position - new Vector3(0f, colliderSize.y / 2);
		if (moveInputX == 0f && Grounded() == true) {
			characterCollider.sharedMaterial = fullFriction;
		}
		else {
			characterCollider.sharedMaterial = noFriction;
		}
	}

	private void OnDrawGizmos() {
		//Gizmos.color = Color.red;
		//Gizmos.DrawWireCube(groundCheck.transform.position - transform.up * groundCheckCastDistance, groundCheckSize);
		//Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
		//Gizmos.DrawWireSphere(attackPointDown.position, attackRadius);
		//Gizmos.DrawWireSphere(attackPointUp.position, attackRadius);
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		if (((1 << collision.gameObject.layer) & enemyLayer) != 0) {
			Enemy enemy = collision.gameObject.GetComponent<Enemy>();
			if (enemy != null) {
				Debug.Log("hit");
				if (enemy.hasTakenDamage == false) {
					StartCoroutine(FreezeFrame());
					enemy.TakeDamage(attackDamage);
					damagedEnemies.Add(enemy);
				}
				if (attackCollDown.enabled == true) {
					rb.velocity = new Vector2(rb.velocity.x, jumpForce * 1.1f);
				}
			}
		}
	}
}
