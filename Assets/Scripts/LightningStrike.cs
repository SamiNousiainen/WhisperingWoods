using UnityEngine;

public class LightningStrike : MonoBehaviour {

	float damage = 10f;
	private Collider2D coll;

	void Start() {
		coll = GetComponent<Collider2D>();
	}

	void Update() {

	}

	public void ActivateDamageCollider() {
		coll.enabled = true;
	}


	private void OnTriggerEnter2D(Collider2D collision) {
		Player player = collision.gameObject.GetComponent<Player>();
		if (player != null && Player.instance.damageCooldownTimer < 0f) {
			Player.instance.TakeDamage(damage, transform);
		}
	}

	private void Despawn() {
		Destroy(gameObject);
	}
}
