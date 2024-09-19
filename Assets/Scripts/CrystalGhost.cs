using System.Collections;
using UnityEngine;

public class CrystalGhost : Enemy {


	[SerializeField]
	private GameObject lightningPrefab;
	[SerializeField]
	private GameObject crystalPrefab;

	private Rigidbody2D rb;

	[Header("attack values")]
	private float fallingCrystalsAmount = 5f;
	private float projectileInterval = 0.7f;
	private float attackTimer = 0f;
	private float attackTime = 5f;
	private bool attacking = false;

	void Start() {
		health = 500f;
		rb = GetComponent<Rigidbody2D>();
		rb.isKinematic = true;
	}

	void Update() {
		attackTimer -= Time.deltaTime;
	}

	public override void TakeDamage(float damage) {
		base.TakeDamage(damage);
		float randomValue = Random.value;
		if (attackTimer <= 0f && attacking == false) {
			if (randomValue <= 0.5f) {
				StartCoroutine(CrystalAttack());
			}
			if (randomValue > 0.5f) {
				StartCoroutine(LightningAttack());
			}
			Debug.Log(randomValue);
		}
	}

	private IEnumerator LightningAttack() {
		if (lightningPrefab != null) {
			attackTimer = attackTime;
			attacking = true;
			GameObject lightning = Instantiate(lightningPrefab, BossArena.instance.transform.position, Quaternion.identity);
			yield return new WaitForSeconds(1);
			GameObject lightning2 = Instantiate(lightningPrefab, new Vector2(BossArena.instance.transform.position.x + 2.5f, BossArena.instance.transform.position.y), Quaternion.identity);
			Destroy(lightning);
			yield return new WaitForSeconds(1);
			Destroy(lightning2);
			attacking = false;
		}
	}

	public void FallingCrystals() {
		Vector2 crystalSpawnPos = new Vector2(Player.instance.transform.position.x, 23); //set Y pos instead of manual
		if (crystalPrefab != null) {
			Instantiate(crystalPrefab, crystalSpawnPos, Quaternion.identity);
		}
	}

	private IEnumerator CrystalAttack() {
		attackTimer = attackTime;

		for (int i = 0; i < fallingCrystalsAmount; i++) {
			FallingCrystals();

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

	private void OnCollisionEnter2D(Collision2D collision) {
		Player player = collision.gameObject.GetComponent<Player>();
		if (player != null && Player.instance.damageCooldownTimer < 0f) {
			DealDamage();
		}
	}
}
