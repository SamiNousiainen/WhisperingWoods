using UnityEngine;
public class Mushroom : Enemy {

	private float attackTimer;
	private float attackCooldown = 3f;
	private float attackRadius = 3f;
	public GameObject poisonMist;

	void Start() {
		health = 20;
		damage = 5f;
	}


	void Update() {
		attackTimer -= Time.deltaTime;

		if (Vector2.Distance(Player.instance.transform.position, transform.position) <= attackRadius && attackTimer < 0) {
			Attack();
		}
	}

	private void Attack() {

		Instantiate(poisonMist, transform.position, Quaternion.identity);

		Collider2D player = Physics2D.OverlapCircle(transform.position, attackRadius, LayerMask.NameToLayer("Player"));
		if (player != null) {
			Player.instance.TakeDamage(damage, transform);
		}
		attackTimer = attackCooldown;
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, attackRadius);

	}
}
