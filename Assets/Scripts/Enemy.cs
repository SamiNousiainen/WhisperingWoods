using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour {

	public float health = 50f;
	private float damage = 10f;
	public bool hasTakenDamage;
	[SerializeField]
	private SpriteRenderer spriteRenderer;
	private Color originalColor;

	void Awake() {
		spriteRenderer = GetComponent<SpriteRenderer>();
		originalColor = spriteRenderer.color;
	}

	void Update() {

	}

	public virtual void TakeDamage(float damage) {
		hasTakenDamage = true;
		health -= damage;
		Debug.Log(health);
		//play damage animation
		StartCoroutine(FlashRed());
		if (health <= 0) {
			Debug.Log("enemy dead");
			Destroy(gameObject);
		}
	}

	public void DealDamage() {
		if (Player.instance != null) {
			Player.instance.TakeDamage(damage, transform);
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
