using UnityEngine;

public class BossArena : MonoBehaviour {

	public static BossArena instance;

	public Transform spawnPoint;

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

	}

	void Update() {

	}
}
