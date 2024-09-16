using ProjectEnums;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {

	public static SceneLoader instance;
	public bool sceneLoadInProgress;

	private GameObject persistentObject;
	private Checkpoint.CheckpointNumber checkpoint;
	private LevelChangeTrigger.SpawnPoint spawnPoint;

	private void Awake() {
		if (instance == null) {
			instance = this;
			DontDestroyOnLoad(gameObject);
			SceneManager.sceneLoaded += OnSceneLoaded;
		}
		else {
			Destroy(gameObject);
		}
	}

	private void OnDestroy() {
		if (instance == this) {
			instance = null;
			SceneManager.sceneLoaded -= OnSceneLoaded;
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
				if (UserProfile.CurrentProfile.currentCheckpoint != Checkpoint.CheckpointNumber.None) {
					checkpoint = UserProfile.CurrentProfile.currentCheckpoint; // Retrieve saved checkpoint
					SceneSwapManager.instance.FindCheckpoint(checkpoint);
					Debug.Log("checkpoint found");
				}
				else {
					Debug.Log("checkpoint not found");
				}
				Object.DontDestroyOnLoad(persistentObject);
				UserProfile.SaveCurrent();
			}
			else {
				if (UserProfile.CurrentProfile.currentCheckpoint != Checkpoint.CheckpointNumber.None) {
					checkpoint = UserProfile.CurrentProfile.currentCheckpoint; // Retrieve saved checkpoint
					SceneSwapManager.instance.FindCheckpoint(checkpoint);
					Debug.Log("checkpoint found");
				}
				else {
					Debug.Log("checkpoint not found");
				}
				UserProfile.SaveCurrent();
			}
		}
		else if (sceneName == "MainMenu") {
			if (persistentObject != null) {
				Debug.Log("Destroying persistent object in MainMenu");
				Destroy(persistentObject);
				persistentObject = null;
			}
		}
	}
}
