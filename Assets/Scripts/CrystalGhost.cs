using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalGhost : Enemy {

	[SerializeField]
	private GameObject lightningPrefab;
	[SerializeField]
	private GameObject crystalPrefab;
	private List<GameObject> attacks;
	private float fallingCrystalsAmount = 5f;
	private float projectileInterval = 0.7f;
	private float attackTimer = 0f;
	private float attackTime = 5f;

	void Start() {
		health = 500f;
	}

	void Update() {
		attackTimer -= Time.deltaTime;
	}

	public override void TakeDamage(float damage) {
		base.TakeDamage(damage);
		if (attackTimer <= 0f) {
			//StartCoroutine(CrystalAttack());
			StartCoroutine(LightningAttack());
		}
	}

	private IEnumerator LightningAttack() {
		if (lightningPrefab != null) {
			attackTimer = attackTime;
			GameObject lightning = Instantiate(lightningPrefab, BossArena.instance.transform.position, Quaternion.identity);
			yield return new WaitForSeconds(1);
			GameObject lightning2 = Instantiate(lightningPrefab, new Vector2(BossArena.instance.transform.position.x + 2.5f, BossArena.instance.transform.position.y), Quaternion.identity);
			Destroy(lightning);
			yield return new WaitForSeconds(1);
			Destroy(lightning2);
		}
	}

	public void FallingCrystals() {
		Vector2 crystalSpawnPos = new Vector2(Player.instance.transform.position.x, 23); //set Y pos instead of manual
		if (crystalPrefab != null) {
			Instantiate(crystalPrefab, crystalSpawnPos, Quaternion.identity);
		}
	}

	private IEnumerator CrystalAttack() {
		//attackTimer = attackTime;

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
}
