using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneFadeManager : MonoBehaviour {

	public static SceneFadeManager instance;

	[SerializeField] public Image fadeOutImage;
	[Range(0.1f, 20f), SerializeField] private float fadeOutSpeed = 10f;
	[Range(0.1f, 20f), SerializeField] private float fadeInSpeed = 10f;

	[SerializeField] public Color fadeOutStartColor;

	public bool isFadingOut { get; private set; }
	
	public bool isFadingIn { get; private set; }

	private void Awake() {
        
		if (instance == null) {
			instance = this;
		}

		fadeOutStartColor.a = 0f;
    }

	private void Update() {
		if (isFadingOut) {
			if (fadeOutImage.color.a < 1f) {
				fadeOutStartColor.a += Time.deltaTime * fadeOutSpeed;
				fadeOutImage.color = fadeOutStartColor;
			}
			else {
				isFadingOut = false;
			}
		}

		if (isFadingIn) {
			if (fadeOutImage.color.a > 0f) {
				fadeOutStartColor.a -= Time.deltaTime * fadeInSpeed;
				fadeOutImage.color = fadeOutStartColor;
			}
			else {
				isFadingIn = false;
				Player.instance.enabled = true;
			}
		}
	}

	public void StartFadeOut() {
		fadeOutImage.color = fadeOutStartColor;
		WindowManager.instance.CloseWindow(ProjectEnums.WindowPanel.GameUI);
		isFadingOut = true;
		Debug.Log("fading out");
	}

	public void StartFadeIn() {
		if (fadeOutImage.color.a >= 1f) {
			fadeOutImage.color = fadeOutStartColor;
			isFadingIn = true;
			Debug.Log("fading in");
		}
	}

}
