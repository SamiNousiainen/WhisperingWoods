using UnityEngine;

public class CrystalGhost : Enemy {

	[SerializeField]
	private GameObject lightningPrefab;

	void Start() {
		health = 500f;
	}

	void Update() {

	}

	public override void TakeDamage(float damage) {
		base.TakeDamage(damage);
		Instantiate(lightningPrefab, BossArena.instance.attackTest1.position, Quaternion.identity);
		Instantiate(lightningPrefab, BossArena.instance.attackTest2.position, Quaternion.identity);

	}

	public override void DealDamage() {
		base.DealDamage();
	}

	public override void Die() {
		//Play death anim
		Destroy(gameObject);
	}
}
