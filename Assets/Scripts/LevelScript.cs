using ProjectEnums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelScript : MonoBehaviour {
	public static LevelScript instance;

	public LevelID currentLevel;
	//public LevelID overrideStartLevel = LevelID.None;

	private void Awake() {
		if (instance == null) {
			instance = this;
			//if (instance.currentLevel != LevelID.None ) {
			//	UserProfile.CurrentProfile.currentLevel = instance.currentLevel;
			//	UserProfile.SaveCurrent();
			//	Debug.Log(currentLevel);
			//}
		}
		else {
			Destroy(gameObject);
		}
	}

	private void OnDestroy() {
		if (instance == this) {
			instance = null;
		}
	}
}
