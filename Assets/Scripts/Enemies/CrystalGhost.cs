using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CrystalGhost : Enemy {

	[SerializeField]
	private GameObject lightningPrefab;
	[SerializeField]
	private GameObject crystalPrefab;
	[SerializeField]
	private ParticleSystem teleportParticles;

	private PolygonCollider2D collFloating;
	private CapsuleCollider2D collDashing;

	private Rigidbody2D rb;
	private Animator animator;
	[System.NonSerialized]
	private SpriteRenderer spriteRenderer;
	private Light2D spotLight;

	//attack variables
	private float fallingCrystalsAmount = 5f;
	private float projectileInterval = 0.7f;
	private float attackTimer = 0f;
	private float attackCooldown = 3f;
	private float lightningOffset = 2.5f;
	private float dashSpeed = 40f;
	private bool isAttacking = false;

	//movement variables
	private bool isMoving = false;
	private Vector2 targetPos;
	float moveSpeed = 15f;

	enum BossState { Idle, Moving, Attacking, Teleporting }

	BossState currentState = BossState.Idle;

	private void Awake() {
		health = 300f;
		targetPos = rb.position;
		attackTimer = attackCooldown;

		spotLight = GetComponent<Light2D>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();
		rb = GetComponent<Rigidbody2D>();
		collDashing = GetComponent<CapsuleCollider2D>();
		collFloating = GetComponent<PolygonCollider2D>(); //korjaa t‰‰ getcomponent sp‰mmi
	}

	void Start() {

	}

	private void OnValidate() {
		//this.ValidateRefs();
	}

	void Update() {

		if (currentState == BossState.Idle) {
			attackTimer -= Time.deltaTime;
		}

		switch (currentState) {
			case BossState.Idle:
				//Trigger a transition to an attack or movement after a delay
				if (attackTimer < 0f && isAttacking == false) {
					currentState = BossState.Attacking;
				}
				break;
			case BossState.Moving:
				MoveToTarget();
				//if (isAtTargetPosition) currentState = BossState.Attacking;
				break;
			case BossState.Attacking:
				Attack();
				currentState = BossState.Idle;
				break;
				//Add other states with similar transitions
		}

	}

	private void FixedUpdate() {
		if (isMoving == true) {
			MoveToTarget();
		}
	}

	public override void TakeDamage(float damage) {
		base.TakeDamage(damage);
		//Attack();
	}

	public void SetTargetPos(Vector2 target) {
		targetPos = target;

		//example:
		//SetTargetPos(BossArena.instance.transform.position);
		//isMoving = true;
	}

	private void MoveToTarget() {
		Vector2 newPosition = Vector2.MoveTowards(rb.position, targetPos, moveSpeed * Time.fixedDeltaTime);
		rb.MovePosition(newPosition);

		if (Vector2.Distance(rb.position, targetPos) < 0.1f) {
			isMoving = false;
		}
	}

	private void Attack() {
		isAttacking = true;
		int attackRoll = Random.Range(0, 3);
		if (attackRoll == 0) {
			animator.Play("range_attack");
			StartCoroutine(CrystalAttack());
		}
		if (attackRoll == 1) {
			animator.Play("range_attack");
			StartCoroutine(LightningAttack());
		}
		if (attackRoll == 2) {
			Instantiate(teleportParticles, transform.position, Quaternion.identity);
			StartCoroutine(TeleportToDash());
		}
		Debug.Log(attackRoll);
	}

	private IEnumerator TeleportToDash() {
		Vector2 center = BossArena.instance.transform.position;
		Vector2 playerPos = Player.instance.transform.position;
		collFloating.enabled = false;
		spriteRenderer.enabled = false;
		spotLight.enabled = false;
		Instantiate(teleportParticles, transform.position, Quaternion.identity);
		yield return new WaitForSeconds(1f);
		if (playerPos.x <= center.x) {
			transform.position = new Vector2(BossArena.instance.arenaBoundaries.bounds.max.x, BossArena.instance.spawnPoint.transform.position.y);
			Instantiate(teleportParticles, transform.position, Quaternion.identity);
			Dash();
		}
		if (playerPos.x > center.x) {
			transform.position = new Vector2(BossArena.instance.arenaBoundaries.bounds.min.x, BossArena.instance.spawnPoint.transform.position.y);
			Instantiate(teleportParticles, transform.position, Quaternion.identity);
			Dash();
		}
	}

	private void Dash() {
		collDashing.enabled = true;
		spriteRenderer.enabled = true;
		spotLight.enabled = true;
		RotationCheck();
		animator.Play("dash"); //change this animation name to dash startup etc.	
	}

	private IEnumerator DashAttack() {

		if (transform.position.x < BossArena.instance.transform.position.x) {
			animator.Play("dashing");
			rb.velocity = new Vector2(dashSpeed, 0f);
		}
		else {
			animator.Play("dashing");
			rb.velocity = new Vector2(-dashSpeed, 0f);
		}

		attackTimer = attackCooldown;
		yield return new WaitForSeconds(1f);
		StartCoroutine(TeleportToCenter());

		isAttacking = false;
	}

	private IEnumerator TeleportToCenter() {
		rb.velocity = Vector2.zero;
		spriteRenderer.enabled = false;
		spotLight.enabled = false;
		collDashing.enabled = false;
		Instantiate(teleportParticles, transform.position, Quaternion.identity);

		yield return new WaitForSeconds(1f);
		transform.position = BossArena.instance.transform.position;
		spriteRenderer.enabled = true;
		collFloating.enabled = true;
		spotLight.enabled = true;
		animator.Play("idle_1");
		Instantiate(teleportParticles, transform.position, Quaternion.identity);
	}

	private IEnumerator LightningAttack() {
		if (lightningPrefab != null) {
			attackTimer = attackCooldown;

			GameObject lightning = Instantiate(lightningPrefab, BossArena.instance.transform.position, Quaternion.identity);

			yield return new WaitForSeconds(1);

			GameObject lightning2 = Instantiate(lightningPrefab, new Vector2(BossArena.instance.transform.position.x + lightningOffset, BossArena.instance.transform.position.y), Quaternion.identity);

			Destroy(lightning);

			yield return new WaitForSeconds(1);

			Destroy(lightning2);
			animator.Play("idle_1");
			isAttacking = false;
		}
	}

	public void FallingCrystals() {
		Vector2 crystalSpawnPos = new Vector2(Player.instance.transform.position.x, BossArena.instance.arenaBoundaries.bounds.max.y - 1);
		if (crystalPrefab != null) {
			Instantiate(crystalPrefab, crystalSpawnPos, Quaternion.identity);
		}
	}

	private IEnumerator CrystalAttack() {
		attackTimer = attackCooldown;

		for (int i = 0; i < fallingCrystalsAmount; i++) {
			FallingCrystals();
			animator.Play("idle_1");
			isAttacking = false;

			yield return new WaitForSeconds(projectileInterval);
		}
	}

	public override void DealDamage() {
		base.DealDamage();
	}

	public override void Die() {
		//Play death anim
		Destroy(gameObject);
	}

	void RotationCheck() {
		if (transform.position.x <= Player.instance.transform.position.x) {
			Vector3 rotator = new Vector3(transform.rotation.x, 0, transform.rotation.z);
			transform.rotation = Quaternion.Euler(rotator);

		}
		if (transform.position.x > Player.instance.transform.position.x) {
			Vector3 rotator = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
			transform.rotation = Quaternion.Euler(rotator);
		}
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		Player player = collision.gameObject.GetComponent<Player>();
		if (player != null && Player.instance.takingDamageTimer < 0f) {
			DealDamage();
		}
	}
}
