using UnityEngine;

public class Boss : MonoBehaviour {

	public static Boss instance;

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
}
