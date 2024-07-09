using ProjectEnums;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KeyInputManager : MonoBehaviour {
	// Start is called before the first frame update
	void Start() {

	}

	// Update is called once per frame
	void Update() {

		if (SceneManager.GetActiveScene().name != "MainMenu" && SceneManager.GetActiveScene().name != "LoadingScreen") {

			if (Player.instance != null && WindowManager.instance.escapeableWindowStack.Count == 0) {
				Time.timeScale = 1.0f;
				if (Input.GetKeyDown(KeyCode.K) == true || Input.GetButtonDown("Fire1")) {
					GameInputLogic.PlayerAttack();
				}
				else if (Input.GetButtonDown("Jump")) {
					Player.instance.jumpBufferTimer = Player.instance.jumpBufferTime;
				}
				if (Player.instance.jumpBufferTimer > 0 && Player.instance.Grounded() == true) {
					GameInputLogic.PlayerJump();
				}
				if (!Input.GetButton("Jump") && Player.instance.isAttacking == false) {
					GameInputLogic.PlayerDecreaseYVelocity();
				}

			}
			else if (Player.instance != null && WindowManager.instance.escapeableWindowStack.Count > 0) {
				Time.timeScale = 0f;
			}

			if (Input.GetKeyDown(KeyCode.Escape) && WindowManager.instance.escapeableWindowStack.Count == 0 && Player.instance.enabled == true) {
				GameInputLogic.PlayerShowWindow(WindowPanel.PauseMenu);
			}
			else if (Input.GetKeyDown(KeyCode.Escape) && WindowManager.instance.escapeableWindowStack.Count > 0 && Player.instance.enabled == true) {
				// Call your method to close a specific window, e.g., WindowPanel.Inventory
				GameInputLogic.PlayerCloseWindow(WindowManager.instance.escapeableWindowStack.Pop());
			}
			else if (Input.GetKeyDown(KeyCode.Tab) && Player.instance.enabled == true) {
				GameInputLogic.TabPressed();
			}
		}
		else if (SceneManager.GetActiveScene().name == "MainMenu") {
			if (Input.GetKeyDown(KeyCode.Escape) && WindowManager.instance.escapeableWindowStack.Count > 0) {
				GameInputLogic.PlayerCloseWindow(WindowManager.instance.escapeableWindowStack.Pop());
			}
		}
	}
}
