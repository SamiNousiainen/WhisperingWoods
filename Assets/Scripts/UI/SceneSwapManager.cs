using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwapManager : MonoBehaviour {

	public static SceneSwapManager instance;

	private static bool loadFromSpawnPoint;
	private static bool isLoadingScene;

	private GameObject player;
	private Collider2D playerCollider;
	private Collider2D spawnPointCollider;
	private Vector3 playerSpawnPosition;

	private LevelChangeInteractionTrigger.SpawnPoint spawnPoint;

	private void Awake() {
		if (instance == null) {
			instance = this;
		}
		if (Player.instance != null) { 
			player = GameObject.FindGameObjectWithTag("Player");
			playerCollider = player.GetComponent<Collider2D>();
		}
	}

	private void OnEnable() {
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void OnDisable() {
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	public static void SwapSceneFromSpawnPoint(SceneField myScene, LevelChangeInteractionTrigger.SpawnPoint spawnPoint) {
		if (isLoadingScene) return; // Prevent multiple loading

		loadFromSpawnPoint = true;
		isLoadingScene = true;
		instance.StartCoroutine(instance.FadeOutThenChangeScene(myScene, spawnPoint));
	}

	private IEnumerator FadeOutThenChangeScene(SceneField myScene, LevelChangeInteractionTrigger.SpawnPoint spawnPoint = LevelChangeInteractionTrigger.SpawnPoint.None) {
		// start fade to black
		SceneFadeManager.instance.StartFadeOut();

		while (SceneFadeManager.instance.isFadingOut) {
			// keep fading out 
			yield return null;
		}

		// Start loading the scene asynchronously
		AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(myScene);

		// Wait until the scene is loaded
		while (!asyncOperation.isDone) {
			yield return null;
		}

		// Scene loaded, set the spawn point
		this.spawnPoint = spawnPoint;
	}

	// Called whenever a new scene is loaded
	private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
		SceneFadeManager.instance.StartFadeIn();

		if (loadFromSpawnPoint) {
			// Warp player to correct spawn point
			//FindSpawnPoint(_spawnPoint);
			//_player.transform.position = _playerSpawnPosition;
			loadFromSpawnPoint = false;
		}

		isLoadingScene = false; // Reset loading flag
	}

	private void FindSpawnPoint(LevelChangeInteractionTrigger.SpawnPoint spawnPointNumber) {
		LevelChangeInteractionTrigger[] spawnPoints = FindObjectsOfType<LevelChangeInteractionTrigger>();

		for (int i = 0; i < spawnPoints.Length; i++) {
			if (spawnPoints[i].CurrentSpawnPoint == spawnPointNumber) {
				spawnPointCollider = spawnPoints[i].gameObject.GetComponent<Collider2D>();

				// calculate spawn position
				CalculateSpawnPosition();
				return;
			}
		}
	}

	private void CalculateSpawnPosition() {
		float colliderHeight = playerCollider.bounds.extents.y;
		playerSpawnPosition = spawnPointCollider.transform.position - new Vector3(0f, colliderHeight, 0f); 
	}

}


