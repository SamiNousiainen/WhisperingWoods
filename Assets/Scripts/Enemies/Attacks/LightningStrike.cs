using UnityEngine;

public class LightningStrike : MonoBehaviour {

	float damage = 10f;
	private Collider2D coll;
	private SpriteRenderer spriteRenderer;

	void Start() {
		coll = GetComponent<Collider2D>();
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	void Update() {

	}

	public void ActivateDamageCollider() {
		if (coll != null) {
			coll.enabled = true;
		}
		if (spriteRenderer != null) {
			spriteRenderer.color = Color.red; //temp solution for testing
		}
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
