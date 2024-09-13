using UnityEngine;

public class Boss : MonoBehaviour {

	public static Boss instance;

	[SerializeField]
	private float health;

	private void Awake() {
		if (instance == null) {
			instance = this;
		}
		else {
			Destroy(gameObject);
		}
	}
	private void OnDestroy() {
		if (instance == this) {
			instance = null;
		}
	}

	void Start() {
		//spawn hp bar (gameUI/UI manager?)
	}

	void Update() {

	}

	public virtual void TakeDamage(float damage) {
		//hasTakenDamage = true;
		health -= damage;
		Debug.Log(damage);
		//damageCooldownTimer = damageCooldownTime;
		//play damage animation
		StartCoroutine(Player.instance.FreezeFrame());
		//StartCoroutine(FlashRed());
		if (health <= 0) {
			Debug.Log("enemy dead");
			//Destroy(gameObject);
		}
	}
}
