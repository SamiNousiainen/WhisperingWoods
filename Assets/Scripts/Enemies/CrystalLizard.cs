using UnityEngine;

public class CrystalLizard : Enemy {
	[Header("Movement Settings")]
	public float moveSpeed = 3f;
	public float detectionRange = 10f;
	public float stoppingDistance = 1f;

	public float attackRange = 2f;


	[Header("Ground Check")]
	public Transform groundCheckPoint;
	public float groundCheckRadius = 0.2f;
	public LayerMask groundLayer;

	[Header("References")]
	private Rigidbody2D rb;

	[Header("Patrol Settings")]
	public Transform[] patrolPoints;
	private int currentPatrolIndex = 0;
	public float waitTime = 1f;
	private float waitCounter;
	private bool isWaiting = false;

	enum EnemyState { Idle, Patrolling, Attacking, Chasing }
	EnemyState currentState = EnemyState.Patrolling;

	private Animator animator;

	void Start() {
		rb = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();

		// Initialize wait counter if patrol points exist
		if (patrolPoints.Length > 0)
			waitCounter = waitTime;


	}

	void Update() {
		if (Player.instance != null) {

			switch (currentState) {
				case EnemyState.Idle:
					currentState = EnemyState.Patrolling;
					break;
				case EnemyState.Patrolling:
					Patrol();
					break;
				case EnemyState.Attacking:
					AttackPlayer();
					break;
				case EnemyState.Chasing:
					ChasePlayer();
					break;
			}
		}
	}

	private void FixedUpdate() {
		if (rb.velocity.x != 0 && (currentState == EnemyState.Patrolling || currentState == EnemyState.Chasing)) {
			animator.Play("lisko_walk");
		}
		if (rb.velocity.x == 0 && currentState == EnemyState.Idle) {
			animator.Play("lisko_idle");
		}
		if (currentState == EnemyState.Attacking) {
			animator.Play("lisko_attack");
		}

	}


	void ChasePlayer() {

		float direction = Mathf.Sign(Player.instance.transform.position.x - transform.position.x);

		// Set the enemy velocity to move towards the player
		rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);

		// Play the walk animation if not already playing

		if (Vector2.Distance(Player.instance.transform.position, transform.position) <= attackRange) {
			currentState = EnemyState.Attacking;
		}

		if (Vector2.Distance(Player.instance.transform.position, transform.position) > detectionRange) {

			currentState = EnemyState.Patrolling;
		}
		RotationCheck();
		Debug.Log("chasing");
	}

	void AttackPlayer() {
		// Stop moving when attacking
		rb.velocity = new Vector2(0, rb.velocity.y);

		// Play attack animation

		Debug.Log("Attacking player");

		if (Vector2.Distance(Player.instance.transform.position, transform.position) > attackRange) {
			currentState = EnemyState.Patrolling;
		}

		RotationCheck();
	}

	void Patrol() {

		// Skip if no patrol points
		if (patrolPoints.Length == 0) return;

		// Waiting state
		if (isWaiting) {
			waitCounter -= Time.deltaTime;
			currentState = EnemyState.Idle;
			if (waitCounter <= 0) {
				isWaiting = false;
				currentState = EnemyState.Patrolling;
				currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
			}
			return;

		}

		// Move towards current patrol point
		Transform targetPoint = patrolPoints[currentPatrolIndex];
		float directionToPoint = targetPoint.position.x > transform.position.x ? 1 : -1;

		// Move
		rb.velocity = new Vector2(directionToPoint * moveSpeed, rb.velocity.y);

		// Check if reached patrol point
		if (Mathf.Abs(transform.position.x - targetPoint.position.x) < 0.5f) {
			rb.velocity = Vector2.zero;
			isWaiting = true;
			waitCounter = waitTime;
		}

		if (Vector2.Distance(Player.instance.transform.position, transform.position) <= detectionRange) {
			currentState = EnemyState.Chasing;
		}

		RotationCheck();
	}

	void RotationCheck() {
		if (currentState == EnemyState.Chasing || currentState == EnemyState.Attacking) {
			if (transform.position.x <= Player.instance.transform.position.x) {
				Vector3 rotator = new Vector3(transform.rotation.x, 0, transform.rotation.z);
				transform.rotation = Quaternion.Euler(rotator);
			}
			if (transform.position.x > Player.instance.transform.position.x) {
				Vector3 rotator = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
				transform.rotation = Quaternion.Euler(rotator);
			}
		}

		if (currentState == EnemyState.Patrolling) {
			if (rb.velocity.x >= 0) {
				Vector3 rotator = new Vector3(transform.rotation.x, 0, transform.rotation.z);
				transform.rotation = Quaternion.Euler(rotator);
			}
			if (rb.velocity.x < 0) {
				Vector3 rotator = new Vector3(transform.rotation.x, 180, transform.rotation.z);
				transform.rotation = Quaternion.Euler(rotator);
			}
		}
	}

	public override void Die() {
		base.Die();
		//death anim

	}

	// Optional ground check (useful for platformers)
	bool IsGrounded() {
		return Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);
	}

	// Visualization in Scene view
	void OnDrawGizmosSelected() {
		// Draw detection range
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, detectionRange);

		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, attackRange);
	}

}