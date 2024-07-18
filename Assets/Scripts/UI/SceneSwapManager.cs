using ProjectEnums;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;

public class SceneSwapManager : MonoBehaviour {
	public static SceneSwapManager instance;

	private bool loadFromSpawnPoint;
	private bool isLoadingScene;
	public LevelID currentLevel;
	public LevelID overrideStartLevel = LevelID.None;

	private Collider2D playerCollider;
	private Collider2D spawnPointCollider;
	public Vector3 spawnPosition;
	public LevelChangeTrigger.SpawnPoint spawnPoint;
	public Checkpoint.CheckpointNumber currentCheckpoint;

	private void Awake() {
		if (instance == null) {
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else {
			Destroy(gameObject);
			return;
		}
	}

	private void Start() {
		SceneManager.sceneLoaded += OnSceneLoaded;

		// Load the last saved level when the game starts
		if (overrideStartLevel == LevelID.None) {
			if (UserProfile.CurrentProfile != null && UserProfile.CurrentProfile.currentLevel != LevelID.None) {
				StartCoroutine(FadeOutThenChangeScene(UserProfile.CurrentProfile.currentLevel.ToString()));
			}
		}
		else if (overrideStartLevel != LevelID.None) {
			StartCoroutine(FadeOutThenChangeScene(overrideStartLevel.ToString()));
			Debug.Log("Override Start Level: " + overrideStartLevel);
		}
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

	private IEnumerator FadeOutThenChangeScene(string sceneName) {
		Player.instance.enabled = false;
		SceneFadeManager.instance.StartFadeOut();

		yield return new WaitUntil(() => !SceneFadeManager.instance.isFadingOut);

		AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
		asyncOperation.allowSceneActivation = false;

		yield return new WaitUntil(() => asyncOperation.progress >= 0.9f);

		SceneFadeManager.instance.StartFadeIn();
		yield return new WaitUntil(() => !SceneFadeManager.instance.isFadingIn);

		asyncOperation.allowSceneActivation = true;

		yield return new WaitUntil(() => asyncOperation.isDone);

		StartCoroutine(ResetLoadFlagsAfterDelay());
	}

	private IEnumerator ResetLoadFlagsAfterDelay() {
		yield return new WaitForSeconds(0.5f);
		loadFromSpawnPoint = false;
		isLoadingScene = false;
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
		SceneFadeManager.instance.StartFadeIn();

		LevelScript levelScript = FindObjectOfType<LevelScript>();
		if (levelScript != null) {
			currentLevel = levelScript.currentLevel;
			if (UserProfile.CurrentProfile != null) {
				UserProfile.CurrentProfile.currentLevel = currentLevel;
				UserProfile.SaveCurrent();
			}

			if (loadFromSpawnPoint) {
				FindSpawnPoint(spawnPoint);
			}
			else {
				if (UserProfile.CurrentProfile.spawnPoint != LevelChangeTrigger.SpawnPoint.None) {
					FindSpawnPoint(UserProfile.CurrentProfile.spawnPoint);
				}
				else if (UserProfile.CurrentProfile.currentCheckpoint != Checkpoint.CheckpointNumber.None) {
					FindCheckpoint(UserProfile.CurrentProfile.currentCheckpoint);
				}
				else {
					Debug.LogWarning("No spawn point or checkpoint found, defaulting to SpawnPoint One.");
					FindSpawnPoint(LevelChangeTrigger.SpawnPoint.One);
				}
			}
		}
		else {
			//Debug.LogError("LevelScript not found in the loaded scene.");
		}
	}

	public void FindSpawnPoint(LevelChangeTrigger.SpawnPoint spawnPointNumber) {
		LevelChangeTrigger[] spawnPoints = FindObjectsOfType<LevelChangeTrigger>();
		foreach (LevelChangeTrigger spawn in spawnPoints) {
			if (spawn.currentSpawnPoint == spawnPointNumber) {
				Transform childWithCollider = spawn.transform.Find("SpawnPoint");
				if (childWithCollider == null) {
					Debug.LogError($"Child object with BoxCollider not found under spawn point {spawnPointNumber}");
					return;
				}

				BoxCollider2D boxCollider = childWithCollider.GetComponent<BoxCollider2D>();
				if (boxCollider == null) {
					Debug.LogError($"BoxCollider not found on child object under spawn point {spawnPointNumber}");
					return;
				}

				Vector3 spawnPosition = boxCollider.bounds.center;
				Player.instance.transform.position = spawnPosition;

				Player.instance.enabled = true;
				UserProfile.CurrentProfile.spawnPoint = spawnPointNumber;
				Debug.Log("Spawn point found and set: " + spawnPointNumber);
				return;
			}
		}
		Debug.LogError($"Spawn point {spawnPointNumber} not found!");
	}

	public void FindCheckpoint(Checkpoint.CheckpointNumber checkpointNumber) {
		Checkpoint[] checkpoints = FindObjectsOfType<Checkpoint>();
		foreach (Checkpoint checkpoint in checkpoints) {
			if (checkpoint.checkPoint == checkpointNumber) {
				Transform childWithCollider = checkpoint.transform.Find("Checkpoint");
				if (childWithCollider == null) {
					Debug.LogError($"Child object with BoxCollider not found under checkpoint {checkpointNumber}");
					return;
				}

				BoxCollider2D boxCollider = childWithCollider.GetComponent<BoxCollider2D>();
				if (boxCollider == null) {
					Debug.LogError($"BoxCollider not found on child object under checkpoint {checkpointNumber}");
					return;
				}

				Vector3 spawnPosition = boxCollider.bounds.center;
				Player.instance.transform.position = spawnPosition;

				Player.instance.enabled = true;
				Debug.Log("Spawned at checkpoint: " + checkpointNumber);
				return;
			}
		}
		//Debug.LogError($"Checkpoint {checkpointNumber} not found!");
	}
}
