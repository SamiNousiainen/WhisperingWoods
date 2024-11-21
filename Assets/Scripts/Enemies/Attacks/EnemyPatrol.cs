using Unity.VisualScripting;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour {
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

	Animator animator;

	[Header("Direction")]
	private bool facingRight = true;

	void Start() {
		rb = GetComponent<Rigidbody2D>();

		// Validate references
		if (rb == null)
			Debug.LogError("Rigidbody2D component is missing!");

		// Initialize wait counter if patrol points exist
		if (patrolPoints.Length > 0)
			waitCounter = waitTime;

		animator = GetComponent<Animator>();
	}

	void Update() {
		// Ensure Player.instance exists before doing any checks
		if (Player.instance != null) {
			float distanceToPlayer = Mathf.Abs(Player.instance.transform.position.x - transform.position.x);

			if (distanceToPlayer <= attackRange) {
				AttackPlayer();
			}
			else if (distanceToPlayer <= detectionRange) {
				ChasePlayer();
			}
			else {
				Patrol();
			}
		}
		else {
			Patrol();
		}
	}


	void ChasePlayer() {
		if (Player.instance == null) return;

		// Determine direction to player
		float directionToPlayer = Player.instance.transform.position.x > transform.position.x ? 1 : -1;

		// Check if we should stop
		if (Mathf.Abs(Player.instance.transform.position.x - transform.position.x) > stoppingDistance) {
			// Move towards player
			rb.velocity = new Vector2(directionToPlayer * moveSpeed, rb.velocity.y);

			// Flip sprite based on movement direction
			Flip(directionToPlayer > 0);
		}
		else {
			// Stop moving when close to player
			rb.velocity = new Vector2(0, rb.velocity.y);
		}
	}

	void AttackPlayer() {
		if (Player.instance == null) return;

		// Stop moving when attacking
		rb.velocity = new Vector2(0, rb.velocity.y);

		// Play attack animation
		animator.Play("lisko_attack");

		Debug.Log("Attacking player");
	}

	void Patrol() {
		// Skip if no patrol points
		if (patrolPoints.Length == 0) return;

		// Waiting state
		if (isWaiting) {
			waitCounter -= Time.deltaTime;
			if (waitCounter <= 0) {
				isWaiting = false;
				currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
			}
			return;

			
		}

		// Move towards current patrol point
		Transform targetPoint = patrolPoints[currentPatrolIndex];
		float directionToPoint = targetPoint.position.x > transform.position.x ? 1 : -1;

		// Move
		rb.velocity = new Vector2(directionToPoint * moveSpeed, rb.velocity.y);
		if(rb.velocity.x != 0) {
			animator.Play("lisko_walk");
		}

		// Flip sprite based on movement direction
		Flip(directionToPoint > 0);

		// Check if reached patrol point
		if (Mathf.Abs(transform.position.x - targetPoint.position.x) < 0.5f) {
			rb.velocity = Vector2.zero;
			isWaiting = true;
			waitCounter = waitTime;
		}
	}

	void Flip(bool lookingRight) {
		// Only flip if facing the wrong way
		if (facingRight != lookingRight) {
			// Flip the sprite
			transform.Rotate(0f, 180f, 0f);
			facingRight = lookingRight;
		}
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

		// Draw stopping distance
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, stoppingDistance);
	}

}