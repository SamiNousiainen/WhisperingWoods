using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using ProjectEnums;

public class SceneSwapManager : MonoBehaviour {
	public static SceneSwapManager instance;

	private bool loadFromSpawnPoint;
	private bool isLoadingScene;
	public LevelID currentLevel;
	public LevelID overrideStartLevel = LevelID.None;

	private Collider2D playerCollider;
	private Collider2D spawnPointCollider;
	private Vector3 playerSpawnPosition;
	private LevelChangeTrigger.SpawnPoint spawnPoint;

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
		if (UserProfile.CurrentProfile != null && UserProfile.CurrentProfile.currentLevel != LevelID.None) {
			StartCoroutine(FadeOutThenChangeScene(UserProfile.CurrentProfile.currentLevel.ToString()));
		}
		else if (overrideStartLevel != LevelID.None) {
			StartCoroutine(FadeOutThenChangeScene(overrideStartLevel.ToString()));
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

		// Use FindObjectOfType to find the LevelScript component in the scene
		LevelScript levelScript = FindObjectOfType<LevelScript>();
		Debug.Log("Saved current level: " + currentLevel);
		if (levelScript != null) {
			currentLevel = levelScript.currentLevel;
			if (UserProfile.CurrentProfile != null) {
				UserProfile.CurrentProfile.currentLevel = currentLevel;
				UserProfile.SaveCurrent();
				FindSpawnPoint(spawnPoint);
			}
			if (loadFromSpawnPoint) {
				FindSpawnPoint(spawnPoint);
			}
		}
		else {
			Debug.Log("LevelScript not found");
		}

	}

	private void FindSpawnPoint(LevelChangeTrigger.SpawnPoint spawnPointNumber) {

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
				return;
			}
		}

		//Debug.LogError($"Spawn point {spawnPointNumber} not found!");
	}


	//private void FindSpawnPoint(LevelChangeTrigger.SpawnPoint spawnPointNumber) {
	//	LevelChangeTrigger[] spawnPoints = FindObjectsOfType<LevelChangeTrigger>();
	//	foreach (LevelChangeTrigger spawn in spawnPoints) {
	//		if (spawn.currentSpawnPoint == spawnPointNumber) {
	//			// Find the child object with BoxCollider
	//			Transform childWithCollider = spawn.transform.Find("SpawnPoint"); 

	//			if (childWithCollider == null) {
	//				Debug.LogError($"Child object with BoxCollider not found under spawn point {spawnPointNumber}");
	//				return;
	//			}

	//			// Get the BoxCollider component of the child object
	//			BoxCollider2D boxCollider = childWithCollider.GetComponent<BoxCollider2D>();
	//			if (boxCollider == null) {
	//				Debug.LogError($"BoxCollider not found on child object under spawn point {spawnPointNumber}");
	//				return;
	//			}

	//			// Calculate the spawn position at the center of the BoxCollider
	//			Vector3 spawnPosition = boxCollider.bounds.center;

	//			Player.instance.transform.position = spawnPosition - new Vector3(0f, 0f, 0f); // Adjust as needed


	//			Player.instance.enabled = true;
	//			return;
	//		}
	//	}

	//	//Debug.LogError($"Spawn point {spawnPointNumber} not found!");
	//}
}



//using UnityEngine;
//using UnityEngine.SceneManagement;
//using System.Collections;
//using ProjectEnums;

//public class SceneSwapManager : MonoBehaviour {
//	public static SceneSwapManager instance;

//	private bool loadFromSpawnPoint;
//	private bool isLoadingScene;
//	public LevelID currentLevel;
//	public LevelID overrideStartLevel = LevelID.None;

//	private Collider2D playerCollider;
//	private Collider2D spawnPointCollider;
//	private Vector3 playerSpawnPosition;
//	private LevelChangeTrigger.SpawnPoint spawnPoint;

//	private void Awake() {
//		if (instance == null) {
//			instance = this;
//		}
//		else {
//			Destroy(gameObject);
//			return;
//		}

//		DontDestroyOnLoad(gameObject);
//	}

//	private void Start() {
//		SceneManager.sceneLoaded += OnSceneLoaded;
//		if (Player.instance != null) {
//		}
//	}

//	private void OnDestroy() {
//		SceneManager.sceneLoaded -= OnSceneLoaded;
//		if (Player.instance != null) {
//		}
//	}

//	public static void SwapSceneFromSpawnPoint(SceneField myScene, LevelChangeTrigger.SpawnPoint spawnPoint) {
//		if (instance.isLoadingScene)
//			return;

//		instance.loadFromSpawnPoint = true;
//		instance.isLoadingScene = true;
//		instance.spawnPoint = spawnPoint;
//		instance.StartCoroutine(instance.FadeOutThenChangeScene(myScene));
//	}

//	private IEnumerator FadeOutThenChangeScene(SceneField myScene) {
//		Player.instance.enabled = false;
//		SceneFadeManager.instance.StartFadeOut();

//		yield return new WaitUntil(() => !SceneFadeManager.instance.isFadingOut);

//		AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(myScene);

//		yield return new WaitUntil(() => asyncOperation.isDone);

//		yield return null; // Yield to ensure OnSceneLoaded is called after setting sceneLoaded to true

//		StartCoroutine(ResetLoadFlagsAfterDelay());
//	}

//	private IEnumerator ResetLoadFlagsAfterDelay() {
//		yield return new WaitForSeconds(0.5f); // Adjust delay as needed
//		loadFromSpawnPoint = false;
//		isLoadingScene = false;
//	}

//	private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
//		SceneFadeManager.instance.StartFadeIn();
//		LevelScript levelScript = GetComponent<LevelScript>();
//		if (UserProfile.CurrentProfile != null && LevelScript.instance != null) {
//			if (LevelScript.instance.currentLevel != LevelID.None) {
//				UserProfile.CurrentProfile.currentLevel = instance.currentLevel;
//				UserProfile.SaveCurrent();
//				Debug.Log(currentLevel);
//			}
//		}


//		if (loadFromSpawnPoint) {
//			//Player.instance.transform.position = LevelScript.instance.spawnPoint.position;
//			FindSpawnPoint(spawnPoint);

//			//if (player != null && playerCollider != null) {
//			//	player.transform.position = playerSpawnPosition;
//			//}
//		}
//	}



//	private void FindSpawnPoint(LevelChangeTrigger.SpawnPoint spawnPointNumber) {
//		LevelChangeTrigger[] spawnPoints = FindObjectsOfType<LevelChangeTrigger>();
//		foreach (LevelChangeTrigger spawn in spawnPoints) {
//			if (spawn.currentSpawnPoint == spawnPointNumber) {
//				spawnPointCollider = spawn.triggerCollider;
//				//float colliderHeight = playerCollider.bounds.extents.y;
//				//playerSpawnPosition =
//				if (Player.instance.isFacingRight == true) {
//					Player.instance.transform.position = spawnPointCollider.bounds.center - new Vector3(-4f, 0f, 0f);
//				}
//				else {
//					Player.instance.transform.position = spawnPointCollider.bounds.center - new Vector3(4f, 0f, 0f);
//				}
//				Player.instance.enabled = true;
//				return;
//			}
//		}

//		Debug.LogError($"Spawn point {spawnPointNumber} not found!");
//	}
//}
