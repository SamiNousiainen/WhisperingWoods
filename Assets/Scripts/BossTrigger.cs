using UnityEngine;

public class BossTrigger : MonoBehaviour {
	[SerializeField]
	private GameObject bossPrefab;

	void Start() {

	}


	void Update() {

	}

	private void OnTriggerEnter2D() {
		if (BossArena.instance != null) {
			Instantiate(bossPrefab, BossArena.instance.spawnPoint.position, Quaternion.identity);
		}
		Destroy(gameObject);
	}
}
