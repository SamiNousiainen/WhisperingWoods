using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneSwapManager : MonoBehaviour {
	public static SceneSwapManager instance;

	private bool loadFromSpawnPoint;
	private bool isLoadingScene;

	private Player player;
	private Collider2D playerCollider;
	private Collider2D spawnPointCollider;
	private Vector3 playerSpawnPosition;
	private LevelChangeTrigger.SpawnPoint spawnPoint;

	private void Awake() {
		if (instance == null) {
			instance = this;
		}
		else {
			Destroy(gameObject);
			return;
		}

		DontDestroyOnLoad(gameObject);
	}

	private void Start() {
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void OnDestroy() {
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	public static void SwapSceneFromSpawnPoint(SceneField myScene, LevelChangeTrigger.SpawnPoint spawnPoint) {
		if (instance.isLoadingScene)
			return;

		instance.loadFromSpawnPoint = true;
		instance.isLoadingScene = true;
		instance.spawnPoint = spawnPoint;
		instance.StartCoroutine(instance.FadeOutThenChangeScene(myScene));
	}

	private IEnumerator FadeOutThenChangeScene(SceneField myScene) {
		SceneFadeManager.instance.StartFadeOut();

		yield return new WaitUntil(() => !SceneFadeManager.instance.isFadingOut);

		AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(myScene);

		yield return new WaitUntil(() => asyncOperation.isDone);

		yield return null; // Yield to ensure OnSceneLoaded is called after setting sceneLoaded to true

		StartCoroutine(ResetLoadFlagsAfterDelay());
	}

	private IEnumerator ResetLoadFlagsAfterDelay() {
		yield return new WaitForSeconds(0.5f); // Adjust delay as needed
		loadFromSpawnPoint = false;
		isLoadingScene = false;
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
		SceneFadeManager.instance.StartFadeIn();

		if (loadFromSpawnPoint) {
			FindSpawnPoint(spawnPoint);

			if (player != null && playerCollider != null) {
				player.transform.position = playerSpawnPosition;
			}
			//else {
			//	Debug.LogError("Player or Player Collider is null in OnSceneLoaded");
			//}
		}
	}

	private void FindSpawnPoint(LevelChangeTrigger.SpawnPoint spawnPointNumber) {
		LevelChangeTrigger[] spawnPoints = FindObjectsOfType<LevelChangeTrigger>();

		foreach (LevelChangeTrigger spawn in spawnPoints) {
			if (spawn.currentSpawnPoint == spawnPointNumber) {
				spawnPointCollider = spawn.triggerCollider;
				CalculateSpawnPosition();
				return;
			}
		}

		Debug.LogError($"Spawn point {spawnPointNumber} not found!");
	}

	private void CalculateSpawnPosition() {
		if (playerCollider == null || spawnPointCollider == null) {
			//Debug.LogError("Player Collider or SpawnPoint Collider is null in CalculateSpawnPosition");
			return;
		}

		float colliderHeight = playerCollider.bounds.extents.y;
		playerSpawnPosition = spawnPointCollider.bounds.center - new Vector3(0f, colliderHeight, 0f);
	}
}
