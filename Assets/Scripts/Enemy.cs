using UnityEngine;

public class Enemy : MonoBehaviour {

	private float health = 50f;
	private float damage = 10f;

	void Start() {

	}

	void Update() {

	}

	public void TakeDamage(float damage) {
		health -= damage;
		Debug.Log(damage);
		//play damage animation
		if (health <= 0) {
			Debug.Log("enemy dead");
			//Destroy(gameObject);
		}
	}

	public void DealDamage() {
		if (Player.instance != null) {
			Player.instance.TakeDamage(damage);
		}
	}

	private void OnCollisionEnter2D(Collision2D collision) {
		Player player = collision.gameObject.GetComponent<Player>();
		if (player != null && Player.instance.damageCooldownTimer < 0f) {
			DealDamage();
		}
	}
}
