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
	private float attackTime = 2f;
	private float lightningOffset = 2.5f;
	private float dashSpeed = 40f;
	private bool isAttacking = false;

	//movement variables
	private bool isMoving = false;
	private Vector2 targetPos;
	float moveSpeed = 15f;

	void Start() {
		spotLight = GetComponent<Light2D>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		health = 500f;
		animator = GetComponent<Animator>();
		rb = GetComponent<Rigidbody2D>();
		collDashing = GetComponent<CapsuleCollider2D>();
		collFloating = GetComponent<PolygonCollider2D>();
		targetPos = rb.position;
		attackTimer = attackTime;
	}

	void Update() {
		attackTimer -= Time.deltaTime;

		if (attackTimer < 0f) {

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
		float randomValue = Random.value;
		if (attackTimer <= 0f && isAttacking == false) {
			isAttacking = true;
			if (randomValue > 0.66f) {
				animator.Play("range_attack");
				StartCoroutine(CrystalAttack());
			}
			if (randomValue > 0.33f && randomValue <= 0.66f) {
				animator.Play("range_attack");
				StartCoroutine(LightningAttack());
			}
			if (randomValue <= 0.33f) {
				Instantiate(teleportParticles, transform.position, Quaternion.identity);
				StartCoroutine(Teleport());
			}
			Debug.Log(randomValue);
		}
	}

	private IEnumerator Teleport() {
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
			StartCoroutine(Dash());
		}
		if (playerPos.x > center.x) {
			transform.position = new Vector2(BossArena.instance.arenaBoundaries.bounds.min.x, BossArena.instance.spawnPoint.transform.position.y);
			Instantiate(teleportParticles, transform.position, Quaternion.identity);
			StartCoroutine(Dash());
		}

	}

	private IEnumerator Dash() {
		collDashing.enabled = true;
		spriteRenderer.enabled = true;
		spotLight.enabled = true;
		RotationCheck();
		animator.Play("dash"); //change this animation name to dash startup etc.
		yield return new WaitForSeconds(0.28f);
		if (transform.position.x < BossArena.instance.transform.position.x) {
			animator.Play("dashing");
			rb.velocity = new Vector2(dashSpeed, 0f);
		}
		else {
			animator.Play("dashing");
			rb.velocity = new Vector2(-dashSpeed, 0f);
		}


		yield return new WaitForSeconds(1f);
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

		isAttacking = false;
		attackTimer = attackTime;
	}

	private IEnumerator LightningAttack() {
		if (lightningPrefab != null) {
			attackTimer = attackTime;

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
		attackTimer = attackTime;

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
