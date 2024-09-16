using UnityEngine;

public class LightningStrike : MonoBehaviour {

	float damage = 10f;

	void Start() {

	}

	void Update() {

	}


	private void OnTriggerEnter2D(Collider2D collision) {
		Player player = collision.gameObject.GetComponent<Player>();
		if (player != null && Player.instance.damageCooldownTimer < 0f) {
			Player.instance.TakeDamage(damage, transform);
		}
	}
}
