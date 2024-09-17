using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalGhost : Enemy {

	[SerializeField]
	private GameObject lightningPrefab;
	[SerializeField]
	private GameObject crystalPrefab;
	private List<GameObject> attacks;
	private float burstAmount = 5f;
	private float projectileInterval = 0.7f;

	void Start() {
		health = 500f;
	}

	void Update() {

	}

	public override void TakeDamage(float damage) {
		base.TakeDamage(damage);
		//StartCoroutine(CrystalAttack());
		StartCoroutine(LightningAttack());
	}

	private IEnumerator LightningAttack() {
		if (lightningPrefab != null) {
			GameObject lightning = Instantiate(lightningPrefab, BossArena.instance.transform.position, Quaternion.identity);
			yield return new WaitForSeconds(2);
			Destroy(lightning);
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

		for (int i = 0; i < burstAmount; i++) {
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
