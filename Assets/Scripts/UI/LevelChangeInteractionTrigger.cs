using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelChangeInteractionTrigger : TriggerInteractionBase {

	public Collider2D triggerCollider;
	public enum SpawnPoint {
		None,
		One,
		Two,
		Three,
		Four,
	}

	[Header("Spawn TO")]
	[SerializeField] private SpawnPoint SpawnToPoint;
	[SerializeField] private SceneField sceneToLoad;

	[Space(10f)]
	[Header("Spawn Point")]
	public SpawnPoint CurrentSpawnPoint;

	//private void ontriggerenter() {
	//	sceneswapmanager.swapscenefromspawnpoint(scenetoload, spawntopoint);
	//} 

	//public override void interact() {
	//	//base.interact();
	//	sceneswapmanager.swapscenefromspawnpoint(scenetoload, spawntopoint);
	//}
}
