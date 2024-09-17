using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalGhost : Enemy {

	[SerializeField]
	private GameObject lightningPrefab;
	private List<GameObject> attacks;

	void Start() {
		health = 500f;
	}

	void Update() {

	}

	public override void TakeDamage(float damage) {
		base.TakeDamage(damage);
		StartCoroutine(LightningAttack());
	}

	private IEnumerator LightningAttack() {
		GameObject lightning = Instantiate(lightningPrefab, new Vector2(BossArena.instance.transform.position.x, 7), Quaternion.identity);
		yield return new WaitForSeconds(2);
		Destroy(lightning);
	}

	public override void DealDamage() {
		base.DealDamage();
	}

	public override void Die() {
		//Play death anim
		Destroy(gameObject);
	}
}
