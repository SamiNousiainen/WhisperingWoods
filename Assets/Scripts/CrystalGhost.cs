public class CrystalGhost : Enemy {

	void Start() {
		health = 100f;
	}

	void Update() {

	}

	public override void TakeDamage(float damage) {
		base.TakeDamage(damage);
	}
}
