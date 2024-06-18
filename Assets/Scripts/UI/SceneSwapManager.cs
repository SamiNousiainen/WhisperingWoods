using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwapManager : MonoBehaviour {
   
	public static SceneSwapManager instance;

	private static bool _loadFromSpawnPoint;

	private LevelChangeInteractionTrigger.SpawnPoint _spawnPoint;

	private void Awake() {
		if (instance == null) {
			instance = this;
		}
	}

	private void OnEnable() {
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void OnDisable() {
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}
	public static void SwapSceneFromSpawnPoint(SceneField myScene, LevelChangeInteractionTrigger.SpawnPoint spawnPoint) {

		_loadFromSpawnPoint = true;
		instance.StartCoroutine(instance.FadeOutThenChangeScene(myScene, spawnPoint));
	}

	private IEnumerator FadeOutThenChangeScene(SceneField myScene, LevelChangeInteractionTrigger.SpawnPoint spawnPoint = LevelChangeInteractionTrigger.SpawnPoint.None) {

		// start fade to black
		SceneFadeManager.instance.StartFadeOut();

		while(SceneFadeManager.instance.IsFadingOut) {
			// keep fading out 
			yield return null;
		}
		_spawnPoint = spawnPoint;
		SceneManager.LoadScene(myScene);
	}

	// Called whenever a new scene is loaded
	private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
		SceneFadeManager.instance.StartFadeIn();

		if (_loadFromSpawnPoint) {

			//warp player to correct spawn point

			_loadFromSpawnPoint = false;
		}
	}

}
