using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using ProjectEnums;
using static LevelChangeTrigger;

public class SceneLoader : MonoBehaviour {

	public static SceneLoader instance;
	public bool sceneLoadInProgress;

	private GameObject persistentObject;
	private SpawnPoint spawnPoint;

	private void Awake() {
		if (instance == null) {
			instance = this;
			DontDestroyOnLoad(gameObject);
			SceneManager.sceneLoaded += OnSceneLoaded; // Subscribe to sceneLoaded event
		}
		else {
			Destroy(gameObject);
		}
	}

	private void OnDestroy() {
		if (instance == this) {
			instance = null;
			SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe from sceneLoaded event
		}
	}

	public void LoadScene(int sceneBuildIndex, object parameters = null) {
		StartCoroutine(LoadSceneInternal(SceneManager.GetSceneAt(sceneBuildIndex).name, parameters));
	}

	public void LoadScene(string sceneName, object parameters = null) {
		StartCoroutine(LoadSceneInternal(sceneName, parameters));
	}

	private IEnumerator LoadSceneInternal(string sceneName, object parameters) {
		WindowManager.instance.ShowWindow(WindowPanel.LoadingScreen);
		sceneLoadInProgress = true;
		yield return new WaitForSecondsRealtime(Gval.panelAnimationDuration);

		WindowManager.instance.CloseWindowsOnSceneLoad();
		AsyncOperation asyncOp = SceneManager.LoadSceneAsync(sceneName);
		asyncOp.allowSceneActivation = false;
		yield return new WaitForSecondsRealtime(Gval.mininumLoadingScreenDisplayTime - Gval.panelAnimationDuration);
		while (!asyncOp.isDone) {
			if (asyncOp.progress >= 0.9F) {
				sceneLoadInProgress = false;
				asyncOp.allowSceneActivation = true;
				yield return null;
			}
		}

		WindowManager.instance.escapeableWindowStack.Clear();
		if (sceneName != "LoadingScreen") {
			WindowManager.instance.CloseWindow(WindowPanel.LoadingScreen);
		}
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
		string sceneName = scene.name;

		if (sceneName != "LoadingScreen" && sceneName != "MainMenu") {
			Debug.Log("Loaded player");
			if (persistentObject == null) {
				persistentObject = Object.Instantiate(Resources.Load<GameObject>("PersistenceObjects"));
				if (UserProfile.CurrentProfile.spawnPoint != LevelChangeTrigger.SpawnPoint.None) {
					spawnPoint = UserProfile.CurrentProfile.spawnPoint; // Retrieve saved spawn point
				} else {
					UserProfile.CurrentProfile.spawnPoint = LevelChangeTrigger.SpawnPoint.One;
					spawnPoint = UserProfile.CurrentProfile.spawnPoint;
				}
				SceneSwapManager.instance.FindSpawnPoint(spawnPoint);
				Object.DontDestroyOnLoad(persistentObject);
				UserProfile.SaveCurrent();
			}
		}
		else if (sceneName == "MainMenu") {
			if (persistentObject != null) {
				Debug.Log("Destroying persistent object in MainMenu");
				Destroy(persistentObject);
				persistentObject = null; // Reset the reference
			}
		}
	}
}
