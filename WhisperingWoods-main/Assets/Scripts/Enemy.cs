using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour {

	private float health = 50f;
	private float damage = 10f;
	public float damageCooldownTimer = 0f;
	private float damageCooldownTime = 0.2f;
	public bool hasTakenDamage;
	private SpriteRenderer spriteRenderer;
	private Color originalColor;

	void Start() {
		spriteRenderer = GetComponent<SpriteRenderer>();
		originalColor = spriteRenderer.color;
	}

	void Update() {
		damageCooldownTimer -= Time.deltaTime;
		if (damageCooldownTimer <= 0) {
			hasTakenDamage = false;
		}
	}

	public void TakeDamage(float damage) {
		hasTakenDamage = true;
		health -= damage;
		Debug.Log(damage);
		damageCooldownTimer = damageCooldownTime;
		//play damage animation
		StartCoroutine(FlashRed());
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

	private IEnumerator FlashRed() {
		spriteRenderer.color = Color.red; // Change to red
		yield return new WaitForSeconds(0.1f); // Duration of the flash
		spriteRenderer.color = originalColor; // Change back to original color
	}
}