using UnityEngine;

public class Projectile : MonoBehaviour {

	float damage = 10f;
	private Collider2D coll;
	void Start() {
		coll = GetComponent<Collider2D>();
	}

	void Update() {

	}

	void OnCollisionEnter2D(Collision2D collision) {
		Player player = collision.gameObject.GetComponent<Player>();
		if (player != null && Player.instance.damageCooldownTimer < 0f) {
			Player.instance.TakeDamage(damage, transform);
		}
		Destroy(gameObject);
	}
}
