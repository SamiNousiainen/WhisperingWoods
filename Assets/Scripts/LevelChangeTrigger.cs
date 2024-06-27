using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LevelChangeInteractionTrigger;

public class LevelChangeTrigger : MonoBehaviour {

	public Collider2D triggerCollider;

	public enum SpawnPoint {
		None,
		One,
		Two,
		Three,
		Four,
	}

	[Header("Spawn TO")]
	[SerializeField] private SpawnPoint pointToSpawn;
	[SerializeField] private SceneField sceneToLoad;

	[Space(10f)]
	[Header("Spawn Point")]
	public SpawnPoint currentSpawnPoint;

	private void OnTriggerEnter2D() {
		SceneSwapManager.SwapSceneFromSpawnPoint(sceneToLoad, pointToSpawn);
	}

}
