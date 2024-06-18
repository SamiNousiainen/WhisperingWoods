using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelChangeInteractionTrigger : TriggerInteractionBase {
	public enum SpawnPoint {
		None,
		One,
		Two,
		Three,
		Four,
	}

	[Header("Spawn TO")]
	[SerializeField] private SpawnPoint SpawnToPoint;
	[SerializeField] private SceneField _sceneToLoad;

	[Space(10f)]
	[Header("Spawn Point")]
	public SpawnPoint CurrentSpawnPoint;

	public override void Interact() {
		//base.Interact();
		SceneSwapManager.SwapSceneFromSpawnPoint(_sceneToLoad, SpawnToPoint);
	}
}
