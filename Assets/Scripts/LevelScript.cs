using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelScript : MonoBehaviour {
	public static LevelScript instance;

	public Transform spawnPoint;
	public Transform spawnPoint2;

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
}
