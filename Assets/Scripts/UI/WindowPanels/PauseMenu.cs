using ProjectEnums;
using UnityEngine;

public class PauseMenu : WindowBase {
	public static PauseMenu instance;


	//public Transform contentParent;
	//public Transform optionsParent;
	//public Transform inventoryParent;
	//public Transform mapParent;

	//[System.NonSerialized]
	//public PauseMenuMode mode;

	private void Awake() {
	}

	public override void Init(object parameters) {
		if (instance == null) {
			instance = this;
			base.Init();
			if (GameUI.instance != null) {
				WindowManager.instance.CloseWindow(WindowPanel.GameUI);
				Time.timeScale = 0;
			}
		}
		else {
			Destroy(gameObject);
		}
	}

	public override void UpdateUI() {

	}

	protected override void OpeningAnimationFinished() {

	}

	protected override void Closing() {
		WindowManager.instance.ShowWindow(WindowPanel.GameUI);
		Time.timeScale = 1;
	}

	protected override void Destroying() {
	}

}
