using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour {

	public float health = 50f;
	public float damage = 10f;
	public bool hasTakenDamage;
	[SerializeField]
	private SpriteRenderer spriteRenderer;
	private Color originalColor;

	void Awake() {
		spriteRenderer = GetComponent<SpriteRenderer>();
		originalColor = spriteRenderer.color;
	}

	public virtual void TakeDamage(float damage) {
		hasTakenDamage = true;
		health -= damage;
		Debug.Log(health);
		StartCoroutine(FlashRed());
		if (health <= 0) {
			Debug.Log("enemy dead");
			Die();
		}
	}

	public virtual void Die() {
		Destroy(gameObject);
	}

	public virtual void DealDamage() {
		if (Player.instance != null) {
			Player.instance.TakeDamage(damage, transform);
		}
	}

	private void OnCollisionEnter2D(Collision2D collision) {
		Player player = collision.gameObject.GetComponent<Player>();
		if (player != null && Player.instance.takingDamageTimer < 0f) {
			DealDamage();
		}
	}

	private IEnumerator FlashRed() {
		spriteRenderer.color = Color.red; // Change to red
		yield return new WaitForSeconds(0.1f); // Duration of the flash
		spriteRenderer.color = originalColor; // Change back to original color
	}
}
